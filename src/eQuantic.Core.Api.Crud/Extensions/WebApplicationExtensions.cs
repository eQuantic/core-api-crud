using System.ComponentModel.DataAnnotations;
using System.Reflection;
using eQuantic.Core.Api.Crud.Filters;
using eQuantic.Core.Application.Crud.Services;
using eQuantic.Core.Api.Extensions;
using eQuantic.Core.Api.Crud.Handlers;
using eQuantic.Core.Api.Crud.Options;
using eQuantic.Core.Application.Crud.Attributes;
using eQuantic.Core.Application.Crud.Enums;
using eQuantic.Core.Application.Entities.Results;
using eQuantic.Core.Domain.Attributes;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace eQuantic.Core.Api.Crud.Extensions;

/// <summary>
/// The wep application extensions class
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Map all CRUD endpoints
    /// </summary>
    /// <param name="app"></param>
    /// <param name="options">Map all CRUD options</param>
    /// <returns></returns>
    public static WebApplication MapAllCrud(this WebApplication app, Action<AllCrudOptions>? options = null)
    {
        var allCrudOptions = new AllCrudOptions();
        options?.Invoke(allCrudOptions);

        var extensionType = typeof(WebApplicationExtensions);

        var assembly = allCrudOptions.GetAssembly() ?? Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes()
            .Where(o =>
                o is { IsAbstract: false, IsInterface: false } &&
                o.GetInterfaces()
                    .Any(i => i == typeof(IReaderService)) &&
                o.GetCustomAttribute<MapCrudEndpointsAttribute>() != null);

        foreach (var type in types)
        {
            var crudEndpoints = type.GetCustomAttribute<MapCrudEndpointsAttribute>()!;
            var interfaces = type.GetInterfaces();
            var serviceType = interfaces.FirstOrDefault(o => o.Name == $"I{type.Name}");

            if (serviceType == null)
            {
                continue;
            }

            var crudInterface = GetCrudServiceInterface(interfaces);

            if (crudInterface == null)
            {
                var readerInterface = GetReaderServiceInterface(interfaces);

                if (readerInterface == null)
                {
                    continue;
                }

                InvokeMapReaders(app, readerInterface, allCrudOptions, extensionType, serviceType, crudEndpoints);
                continue;
            }

            InvokeMapCrud(app, crudInterface, allCrudOptions, extensionType, serviceType, crudEndpoints);
        }

        return app;
    }

    /// <summary>
    /// Map Readers endpoints
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="options">The options</param>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapReaders<TEntity, TService, TKey>(this WebApplication app,
        Action<CrudOptions<TEntity>>? options = null)
        where TEntity : class, new()
        where TService : IReaderService<TEntity, TKey>
    {
        var crudOptions = new CrudOptions<TEntity>();
        options?.Invoke(crudOptions);

        var routeBuilder = !string.IsNullOrEmpty(crudOptions.Prefix)
            ? (IEndpointRouteBuilder)app.MapGroup(crudOptions.Prefix)
            : app;

        if ((crudOptions.Verbs & CrudEndpointVerbs.OnlyGetById) == CrudEndpointVerbs.OnlyGetById)
        {
            routeBuilder.MapGetById<TEntity, TService, TKey>(crudOptions);
        }

        if ((crudOptions.Verbs & CrudEndpointVerbs.OnlyGetPaged) == CrudEndpointVerbs.OnlyGetPaged)
        {
            routeBuilder.MapGetPagedList<TEntity, TService, TKey>(crudOptions);
        }

        return routeBuilder;
    }

    /// <summary>
    /// Map CRUD endpoints
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="options">The options</param>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapCrud<TEntity, TRequest, TService, TKey>(this WebApplication app,
        Action<CrudOptions<TEntity>>? options = null)
        where TEntity : class, new()
        where TService : ICrudService<TEntity, TRequest, TKey>
        where TRequest : class
    {
        var crudOptions = new CrudOptions<TEntity>();
        options?.Invoke(crudOptions);

        var routeBuilder = app.MapReaders<TEntity, TService, TKey>(options);

        if ((crudOptions.Verbs & CrudEndpointVerbs.OnlyCreate) == CrudEndpointVerbs.OnlyCreate)
        {
            routeBuilder.MapCreate<TEntity, TRequest, TService, TKey>(crudOptions);
        }

        if ((crudOptions.Verbs & CrudEndpointVerbs.OnlyUpdate) == CrudEndpointVerbs.OnlyUpdate)
        {
            routeBuilder.MapUpdate<TEntity, TRequest, TService, TKey>(crudOptions);
        }

        if ((crudOptions.Verbs & CrudEndpointVerbs.OnlyDelete) == CrudEndpointVerbs.OnlyDelete)
        {
            routeBuilder.MapDelete<TEntity, TRequest, TService, TKey>(crudOptions);
        }

        return routeBuilder;
    }

    private static Type? GetCrudServiceInterface(IEnumerable<Type> interfaces)
    {
        return interfaces
            .FirstOrDefault(o =>
                o.GenericTypeArguments.Length > 0 && o.GetGenericTypeDefinition() == typeof(ICrudService<,,>));
    }

    private static Type? GetReaderServiceInterface(IEnumerable<Type> interfaces)
    {
        return interfaces
            .FirstOrDefault(o =>
                o.GenericTypeArguments.Length > 0 && o.GetGenericTypeDefinition() == typeof(IReaderService<,>));
    }

    private static Action<ICrudOptions>? GetCrudOptions(AllCrudOptions allCrudOptions, Type entityType)
    {
        return allCrudOptions.GetOptions().ContainsKey(entityType)
            ? allCrudOptions.GetOptions()[entityType]
            : null;
    }

    private static void InvokeMapCrud(WebApplication app, Type crudInterface, AllCrudOptions allCrudOptions,
        Type extensionType, Type serviceType, MapCrudEndpointsAttribute crudEndpoints)
    {
        var entityType = crudInterface.GenericTypeArguments[0];
        var requestType = crudInterface.GenericTypeArguments[1];
        var keyType = crudInterface.GenericTypeArguments[2];

        var crudOptions = GetCrudOptions(allCrudOptions, entityType);
        var method = extensionType.GetMethod(nameof(MapCrud))
            ?.MakeGenericMethod(entityType, requestType, serviceType, keyType);

        InvokeMethod(app, crudEndpoints, method, allCrudOptions, crudOptions);
    }

    private static void InvokeMapReaders(WebApplication app, Type crudInterface, AllCrudOptions allCrudOptions,
        Type extensionType, Type serviceType, MapCrudEndpointsAttribute crudEndpoints)
    {
        var entityType = crudInterface.GenericTypeArguments[0];
        var keyType = crudInterface.GenericTypeArguments[1];

        var crudOptions = GetCrudOptions(allCrudOptions, entityType);
        var method = extensionType.GetMethod(nameof(MapReaders))
            ?.MakeGenericMethod(entityType, serviceType, keyType);

        InvokeMethod(app, crudEndpoints, method, allCrudOptions, crudOptions);
    }

    private static void InvokeMethod(
        WebApplication app,
        MapCrudEndpointsAttribute crudEndpoints,
        MethodBase? method,
        AllCrudOptions allCrudOptions,
        Action<ICrudOptions>? crudOptions)
    {
        method?.Invoke(null, new object?[]
        {
            app, (Action<ICrudOptions>?)(opt =>
            {
                opt.WithVerbs(crudEndpoints.EndpointVerbs);

                if (crudEndpoints.ReferenceType != null)
                {
                    var referenceKeyType = crudEndpoints.ReferenceKeyType ?? crudEndpoints.ReferenceType
                        .GetProperties()
                        .FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null)?
                        .PropertyType;
                    opt.WithReference(crudEndpoints.ReferenceType, referenceKeyType ?? typeof(int));
                }

                if (allCrudOptions.GetRequireAuth() == true)
                {
                    opt.RequireAuthorization();
                }

                var routeFormat = allCrudOptions.GetRouteFormat();
                if (routeFormat.HasValue)
                {
                    opt.WithRouteFormat(routeFormat.Value);
                }

                if (allCrudOptions.GetValidation() == true)
                {
                    opt.WithValidation();
                }

                crudOptions?.Invoke(opt);
            })
        });
    }

    private static string GetPattern<TEntity, TKey>(
        RouteFormat format,
        bool withId = false, 
        Type? referenceType = null)
    {
        var entityType = typeof(TEntity);
        var entityName = GetEntityName(entityType);
        var prefix = entityName.ChangeCase(format);
        var pattern = $"/{prefix}";
        if (referenceType != null)
        {
            pattern = $"/{GetEntityName(referenceType).ChangeCase(format)}/{{referenceId}}{pattern}";
        }

        if (!withId)
            return pattern;

        pattern = IsPrimitiveKey<TKey>()
            ? $"{pattern}/{{id}}"
            : $"{pattern}/{{{string.Join("}/{", GetRoutesFromComplexKey<TKey>())}}}";

        return pattern;
    }

    private static string GetEntityName(MemberInfo entityType)
    {
        var entityAttr = entityType.GetCustomAttribute<EntityAttribute>();
        return entityAttr != null ? entityAttr.Name : entityType.Name;
    }
    
    private static string ChangeCase(this string name, RouteFormat format)
    {
        var route = name.Pluralize();
        return format switch
        {
            RouteFormat.CamelCase => route.Camelize(),
            RouteFormat.PascalCase => route.Pascalize(),
            RouteFormat.SnakeCase => route.Kebaberize(),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    private static bool IsPrimitiveKey<TKey>()
    {
        var keyType = typeof(TKey);
        return keyType == typeof(string) || keyType == typeof(Guid) || keyType.IsPrimitive;
    }

    private static string[] GetRoutesFromComplexKey<TKey>()
    {
        var keyType = typeof(TKey);
        return keyType.GetProperties().Select(o => o.Name.Camelize()!).ToArray();
    }

    private static IEndpointRouteBuilder MapGetById<TEntity, TService, TKey>(this IEndpointRouteBuilder app,
        CrudOptions<TEntity> options)
        where TEntity : class, new()
        where TService : IReaderService<TEntity, TKey>
    {
        var pattern = GetPattern<TEntity, TKey>(options.RouteFormat, true, options.Get.ReferenceType);
        var handlers = new ReaderEndpointHandlers<TEntity, TService, TKey>(options);
        Delegate handler = options.Get.ReferenceType != null
            ? (
                IsPrimitiveKey<TKey>() ? 
                    handlers.GetReferencedHandler(options.Get.ReferenceKeyType!, nameof(handlers.GetReferencedByIdDelegate)) :
                    handlers.GetReferencedHandler(options.Get.ReferenceKeyType!, nameof(handlers.GetReferencedByComplexIdDelegate))
              )
            : (IsPrimitiveKey<TKey>() ? handlers.GetById : handlers.GetByComplexId);

        app
            .MapGet(pattern, handler)
            .SetOptions<TEntity>(options.Get)
            .Produces<TEntity?>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }

    private static IEndpointRouteBuilder MapGetPagedList<TEntity, TService, TKey>(this IEndpointRouteBuilder app,
        CrudOptions<TEntity> options)
        where TEntity : class, new()
        where TService : IReaderService<TEntity, TKey>
    {
        var pattern = GetPattern<TEntity, TKey>(options.RouteFormat, false, options.List.ReferenceType);
        var handlers = new ReaderEndpointHandlers<TEntity, TService, TKey>(options);
        Delegate handler = options.List.ReferenceType != null
            ? handlers.GetReferencedHandler(options.Get.ReferenceKeyType!, nameof(handlers.GetReferencedPagedListDelegate))
            : handlers.GetPagedList;

        app
            .MapGet(pattern, handler)
            .SetOptions<TEntity>(options.List)
            .Produces<PagedListResult<TEntity>>(StatusCodes.Status200OK);
        return app;
    }

    private static IEndpointRouteBuilder MapCreate<TEntity, TRequest, TService, TKey>(this IEndpointRouteBuilder app,
        CrudOptions<TEntity> options)
        where TEntity : class, new()
        where TService : ICrudService<TEntity, TRequest, TKey>
        where TRequest : class
    {
        var pattern = GetPattern<TEntity, TKey>(options.RouteFormat, false, options.Create.ReferenceType);
        var handlers = new CrudEndpointHandlers<TEntity, TRequest, TService, TKey>(options);
        
        
        Delegate handler = options.Create.ReferenceType != null
            ? handlers.GetReferencedHandler(options.Get.ReferenceKeyType!, nameof(handlers.GetReferencedCreateDelegate))
            : handlers.Create;
        app
            .MapPost(pattern, handler)
            .SetOptions<TEntity, TRequest>(options.Create)
            .Produces<TKey>(StatusCodes.Status201Created);
        return app;
    }
    
    private static IEndpointRouteBuilder MapUpdate<TEntity, TRequest, TService, TKey>(this IEndpointRouteBuilder app,
        CrudOptions<TEntity> options)
        where TEntity : class, new()
        where TService : ICrudService<TEntity, TRequest, TKey>
        where TRequest : class
    {
        var pattern = GetPattern<TEntity, TKey>(options.RouteFormat, true, options.Update.ReferenceType);
        var handlers = new CrudEndpointHandlers<TEntity, TRequest, TService, TKey>(options);
        Delegate handler = options.Update.ReferenceType != null
            ? (
                IsPrimitiveKey<TKey>() ? 
                    handlers.GetReferencedHandler(options.Get.ReferenceKeyType!, nameof(handlers.GetReferencedUpdateDelegate)) : 
                    handlers.GetReferencedHandler(options.Get.ReferenceKeyType!, nameof(handlers.GetReferencedUpdateByComplexIdDelegate))
              )
            : (IsPrimitiveKey<TKey>() ? handlers.Update : handlers.UpdateByComplexId);
        app
            .MapPut(pattern, handler)
            .SetOptions<TEntity, TRequest>(options.Update)
            .Produces(StatusCodes.Status200OK);

        return app;
    }

    private static IEndpointRouteBuilder MapDelete<TEntity, TRequest, TService, TKey>(this IEndpointRouteBuilder app,
        CrudOptions<TEntity> options)
        where TEntity : class, new()
        where TService : ICrudService<TEntity, TRequest, TKey>
        where TRequest : class
    {
        var pattern = GetPattern<TEntity, TKey>(options.RouteFormat, true, options.Delete.ReferenceType);
        var handlers = new CrudEndpointHandlers<TEntity, TRequest, TService, TKey>(options);
        Delegate handler = options.Delete.ReferenceType != null
            ? (
                IsPrimitiveKey<TKey>() ? 
                    handlers.GetReferencedHandler(options.Get.ReferenceKeyType!, nameof(handlers.GetReferencedDeleteDelegate)) : 
                    handlers.GetReferencedHandler(options.Get.ReferenceKeyType!, nameof(handlers.GetReferencedDeleteByComplexIdDelegate))
               )
            : (IsPrimitiveKey<TKey>() ? handlers.Delete : handlers.DeleteByComplexId);
        app
            .MapDelete(pattern, handler)
            .SetOptions<TEntity, TRequest>(options.Delete)
            .Produces(StatusCodes.Status200OK);
        return app;
    }

    private static RouteHandlerBuilder SetOptions<TEntity>(this RouteHandlerBuilder endpoint, EndpointOptions options)
    {
        endpoint.WithName(options.Name);

        if (!string.IsNullOrEmpty(options.Summary))
        {
            endpoint.WithSummary(options.Summary);
        }

        if (!string.IsNullOrEmpty(options.Description))
        {
            endpoint.WithDescription(options.Description);
        }

        if (options.Tags.Any())
        {
            endpoint.WithTags(options.Tags);
        }

        endpoint.WithOpenApi(o =>
        {
            if (options.Parameters.Count == 0)
                return o;

            for (var i = options.Parameters.Count - 1; i >= 0; i--)
            {
                o.Parameters.Insert(0, options.Parameters[i]);
            }

            return o;
        });


        if (options.RequireAuth == true)
        {
            endpoint.RequireAuthorization();
        }

        if (options.FilterType != null)
        {
            endpoint.AddEndpointFilter(options.FilterType);
        }
        
        return endpoint;
    }

    private static RouteHandlerBuilder SetOptions<TEntity, TRequest>(this RouteHandlerBuilder endpoint,
        EndpointOptions options) where TRequest : class
    {
        SetOptions<TEntity>(endpoint, options);
        
        if (options.HasValidation == true)
        {
            endpoint.AddEndpointFilter<ValidationFilter<TRequest>>();
        }
        
        return endpoint;
    }
}