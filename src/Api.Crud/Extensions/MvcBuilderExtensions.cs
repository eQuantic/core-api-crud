using System.ComponentModel.DataAnnotations;
using System.Reflection;
using eQuantic.Core.Api.Crud.Controllers;
using eQuantic.Core.Api.Crud.Controllers.Conventions;
using eQuantic.Core.Api.Crud.Options;
using eQuantic.Core.Application.Crud.Attributes;
using eQuantic.Core.Application.Crud.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace eQuantic.Core.Api.Crud.Extensions;

/// <summary>
/// Registers the "traditional" MVC controllers counterpart of <c>MapAllCrud</c>: it auto-discovers the
/// services marked with <see cref="MapCrudEndpointsAttribute"/> and materializes a generic CRUD/Reader
/// controller per entity, with the same routes, references, complex keys, verbs and authorization.
/// </summary>
public static class MvcBuilderExtensions
{
    /// <summary>
    /// Add CRUD controllers (auto-discovered) to an existing MVC builder.
    /// </summary>
    /// <param name="builder">The MVC builder (from <c>AddControllers()</c>)</param>
    /// <param name="options">Map all CRUD options</param>
    public static IMvcBuilder AddCrudControllers(this IMvcBuilder builder, Action<AllCrudOptions>? options = null)
    {
        var allCrudOptions = new AllCrudOptions();
        options?.Invoke(allCrudOptions);

        var assembly = allCrudOptions.GetAssembly()
                       ?? Assembly.GetEntryAssembly()
                       ?? Assembly.GetCallingAssembly();

        var descriptors = new List<CrudControllerDescriptor>();

        foreach (var serviceType in GetCrudServiceTypes(assembly))
        {
            var descriptor = TryCreateDescriptor(serviceType, allCrudOptions, builder.Services);
            if (descriptor != null)
            {
                descriptors.Add(descriptor);
            }
        }

        if (descriptors.Count == 0)
        {
            return builder;
        }

        builder.ConfigureApplicationPartManager(apm =>
            apm.FeatureProviders.Add(new CrudControllerFeatureProvider(descriptors.Select(d => d.ControllerType))));

        builder.Services.Configure<MvcOptions>(mvc =>
            mvc.Conventions.Add(new CrudControllerModelConvention(descriptors)));

        return builder;
    }

    /// <summary>
    /// Add controllers and CRUD controllers (auto-discovered) in one call.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="options">Map all CRUD options</param>
    public static IMvcBuilder AddCrudControllers(this IServiceCollection services, Action<AllCrudOptions>? options = null)
        => services.AddControllers().AddCrudControllers(options);

    private static IEnumerable<Type> GetCrudServiceTypes(Assembly assembly)
        => assembly.GetTypes()
            .Where(o =>
                o is { IsAbstract: false, IsInterface: false } &&
                o.GetInterfaces().Any(i => i == typeof(IReaderService)) &&
                o.GetCustomAttribute<MapCrudEndpointsAttribute>() != null);

    private static CrudControllerDescriptor? TryCreateDescriptor(
        Type serviceType,
        AllCrudOptions allCrudOptions,
        IServiceCollection services)
    {
        var attribute = serviceType.GetCustomAttribute<MapCrudEndpointsAttribute>()!;
        var interfaces = serviceType.GetInterfaces();

        // The registered service interface follows the I{ServiceName} convention (e.g. IExampleService).
        var serviceInterfaceType = interfaces.FirstOrDefault(o => o.Name == $"I{serviceType.Name}");
        if (serviceInterfaceType == null)
        {
            return null;
        }

        var referenceType = attribute.ReferenceType;
        var referenceKeyType = referenceType != null ? ResolveReferenceKeyType(attribute) : null;

        var crudInterface = interfaces.FirstOrDefault(o =>
            o.IsGenericType && o.GetGenericTypeDefinition() == typeof(ICrudService<,,>));

        Type controllerType;
        Type entityType;
        Type keyType;
        Type ctorServiceType;

        if (crudInterface != null)
        {
            entityType = crudInterface.GenericTypeArguments[0];
            var requestType = crudInterface.GenericTypeArguments[1];
            keyType = crudInterface.GenericTypeArguments[2];
            ctorServiceType = typeof(ICrudService<,,>).MakeGenericType(entityType, requestType, keyType);
            controllerType = referenceType != null
                ? typeof(GenericReferencedCrudController<,,,,>).MakeGenericType(entityType, requestType, keyType, referenceType, referenceKeyType!)
                : typeof(GenericCrudController<,,>).MakeGenericType(entityType, requestType, keyType);
        }
        else
        {
            var readerInterface = interfaces.FirstOrDefault(o =>
                o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IReaderService<,>));
            if (readerInterface == null)
            {
                return null;
            }

            entityType = readerInterface.GenericTypeArguments[0];
            keyType = readerInterface.GenericTypeArguments[1];
            ctorServiceType = typeof(IReaderService<,>).MakeGenericType(entityType, keyType);
            controllerType = referenceType != null
                ? typeof(GenericReferencedReaderController<,,,>).MakeGenericType(entityType, keyType, referenceType, referenceKeyType!)
                : typeof(GenericReaderController<,>).MakeGenericType(entityType, keyType);
        }

        // The controller depends on the closed ICrudService/IReaderService; forward it to the
        // concrete service interface the consumer registered (e.g. IExampleService).
        if (ctorServiceType != serviceInterfaceType)
        {
            services.TryAddTransient(ctorServiceType, sp => sp.GetRequiredService(serviceInterfaceType));
        }

        return new CrudControllerDescriptor
        {
            ControllerType = controllerType,
            EntityType = entityType,
            KeyType = keyType,
            Options = BuildOptions(entityType, attribute, allCrudOptions)
        };
    }

    private static Type ResolveReferenceKeyType(MapCrudEndpointsAttribute attribute)
        => attribute.ReferenceKeyType
           ?? attribute.ReferenceType!
               .GetProperties()
               .FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null)?
               .PropertyType
           ?? typeof(int);

    private static ICrudOptions BuildOptions(Type entityType, MapCrudEndpointsAttribute attribute, AllCrudOptions allCrudOptions)
    {
        var options = (ICrudOptions)Activator.CreateInstance(typeof(CrudOptions<>).MakeGenericType(entityType))!;

        options.WithVerbs(attribute.EndpointVerbs);

        if (attribute.ReferenceType != null)
        {
            // Controllers derive the reference route segment from the parent entity name, so always use
            // the default name to keep the generated route and the controller's reader in sync.
            options.WithReference(attribute.ReferenceType, ResolveReferenceKeyType(attribute));
        }

        if (allCrudOptions.GetRequireAuth() == true)
        {
            options.RequireAuthorization();
        }

        var routeFormat = allCrudOptions.GetRouteFormat();
        if (routeFormat.HasValue)
        {
            options.WithRouteFormat(routeFormat.Value);
        }

        if (allCrudOptions.GetValidation() == true)
        {
            options.WithValidation();
        }

        var group = allCrudOptions.GetGroup();
        if (!string.IsNullOrEmpty(group))
        {
            options.WithGroup(group);
        }

        if (allCrudOptions.GetOptions().TryGetValue(entityType, out var perEntityOptions))
        {
            perEntityOptions(options);
        }

        return options;
    }
}
