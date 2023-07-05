using System.Reflection;
using eQuantic.Core.Application.Crud.Services;
using eQuantic.Core.Api.Crud.Handlers;
using eQuantic.Core.Api.Crud.Options;
using eQuantic.Core.Application.Entities.Results;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace eQuantic.Core.Api.Crud.Extensions;

/// <summary>
/// The wep application extensions class
/// </summary>
public static class WepApplicationExtensions
{
    
    
    /// <summary>
    /// Map all CRUD endpoints
    /// </summary>
    /// <param name="app"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static WebApplication MapAllCrud(this WebApplication app, Assembly assembly)
    {
        var extensionType = typeof(WepApplicationExtensions);
        
        var types = assembly.GetTypes()
            .Where(o => o is { IsAbstract: false, IsInterface: false } && o.GetInterfaces().Any(i => i == typeof(ICrudServiceBase)));
        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces();
            var serviceType = interfaces.FirstOrDefault(o => o.Name == $"I{type.Name}");

            if (serviceType == null)
            {
                continue;
            }
            var crudInterface = interfaces
                .FirstOrDefault(o => o.GenericTypeArguments.Length > 0 && o.GetGenericTypeDefinition() == typeof(ICrudServiceBase<,>));
            
            if(crudInterface == null)
            {
                continue;
            }

            var entityType = crudInterface.GenericTypeArguments[0];
            var requestType = crudInterface.GenericTypeArguments[1];

            var method = extensionType.GetMethod(nameof(MapCrud))?.MakeGenericMethod(entityType, requestType, serviceType);
            method?.Invoke(null, new object?[]{ app, null });
        }
        return app;
    }
    
    /// <summary>
    /// Map CRUD endpoints
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="options">The options</param>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    public static WebApplication MapCrud<TEntity, TRequest, TService>(this WebApplication app,
        Action<CrudOptions<TEntity>>? options = null)
        where TEntity : class, new()
        where TService : ICrudServiceBase<TEntity, TRequest>
    {
        var crudOptions = new CrudOptions<TEntity>();
        options?.Invoke(crudOptions);

        app
            .MapGetById<TEntity, TRequest, TService>(crudOptions)
            .MapGetPagedList<TEntity, TRequest, TService>(crudOptions)
            .MapCreate<TEntity, TRequest, TService>(crudOptions)
            .MapUpdate<TEntity, TRequest, TService>(crudOptions)
            .MapDelete<TEntity, TRequest, TService>(crudOptions);

        return app;
    }
    
    private static string GetPattern<TEntity>(bool withId = false, Type? referenceType = null)
    {
        var entityType = typeof(TEntity);
        var entityName = entityType.Name;
        var prefix = entityName.Pluralize().Camelize();
        var pattern = $"/{prefix}";
        if (referenceType != null)
        {
            pattern = $"/{referenceType.Name.Pluralize().Camelize()}/{{referenceId:int}}{pattern}";
        }

        if (withId) pattern = $"{pattern}/{{id:int}}";
        return pattern;
    }

    private static WebApplication MapGetById<TEntity, TRequest, TService>(this WebApplication app, CrudOptions<TEntity> options)
        where TEntity : class, new()
        where TService : ICrudServiceBase<TEntity, TRequest>
    {
        var pattern = GetPattern<TEntity>(true, options.Get.ReferenceType);
        var handlers = new CrudEndpointHandlers<TEntity, TRequest, TService>(options);
        Delegate handler = options.Get.ReferenceType != null
            ? handlers.GetReferencedById
            : handlers.GetById;
        
        app
            .MapGet(pattern, handler)
            .SetOptions<TEntity>(options.Get)
            .Produces<TEntity?>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }

    private static WebApplication MapGetPagedList<TEntity, TRequest, TService>(this WebApplication app, CrudOptions<TEntity> options)
        where TEntity : class, new()
        where TService : ICrudServiceBase<TEntity, TRequest>
    {
        var pattern = GetPattern<TEntity>(false, options.List.ReferenceType);
        var handlers = new CrudEndpointHandlers<TEntity, TRequest, TService>(options);
        Delegate handler = options.List.ReferenceType != null
            ? handlers.GetReferencedPagedList
            : handlers.GetPagedList;
        
        app
            .MapGet(pattern, handler)
            .SetOptions<TEntity>(options.List)
            .Produces<PagedListResult<TEntity>>(StatusCodes.Status200OK);
        return app;
    }

    private static WebApplication MapCreate<TEntity, TRequest, TService>(this WebApplication app, CrudOptions<TEntity> options)
        where TEntity : class, new()
        where TService : ICrudServiceBase<TEntity, TRequest>
    {
        var pattern = GetPattern<TEntity>(false, options.Create.ReferenceType);
        var handlers = new CrudEndpointHandlers<TEntity, TRequest, TService>(options);
        Delegate handler = options.Create.ReferenceType != null
            ? handlers.ReferencedCreate
            : handlers.Create;
        app
            .MapPost(pattern, handler)
            .SetOptions<TEntity>(options.Create)
            .Produces<int>(StatusCodes.Status201Created);
        return app;
    }

    private static WebApplication MapUpdate<TEntity, TRequest, TService>(this WebApplication app, CrudOptions<TEntity> options)
        where TEntity : class, new()
        where TService : ICrudServiceBase<TEntity, TRequest>
    {
        var pattern = GetPattern<TEntity>(true, options.Update.ReferenceType);
        var handlers = new CrudEndpointHandlers<TEntity, TRequest, TService>(options);
        Delegate handler = options.Update.ReferenceType != null
            ? handlers.ReferencedUpdate
            : handlers.Update;
        app
            .MapPut(pattern, handler)
            .SetOptions<TEntity>(options.Update)
            .Produces(StatusCodes.Status200OK);

        return app;
    }

    private static WebApplication MapDelete<TEntity, TRequest, TService>(this WebApplication app, CrudOptions<TEntity> options)
        where TEntity : class, new()
        where TService : ICrudServiceBase<TEntity, TRequest>
    {
        var pattern = GetPattern<TEntity>(true, options.Delete.ReferenceType);
        var handlers = new CrudEndpointHandlers<TEntity, TRequest, TService>(options);
        Delegate handler = options.Delete.ReferenceType != null
            ? handlers.ReferencedDelete
            : handlers.Delete;
        app
            .MapDelete(pattern, handler)
            .SetOptions<TEntity>(options.Delete)
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

        endpoint.WithOpenApi();

        return endpoint;
    }
}