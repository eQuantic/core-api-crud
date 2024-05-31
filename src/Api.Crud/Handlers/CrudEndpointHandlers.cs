using eQuantic.Core.Api.Crud.Extensions;
using eQuantic.Core.Api.Crud.Options;
using eQuantic.Core.Application.Crud.Services;
using eQuantic.Core.Domain.Entities.Requests;
using eQuantic.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Api.Crud.Handlers;

/// <summary>
/// CRUD endpoint handlers
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TService"></typeparam>
/// <typeparam name="TKey"></typeparam>
internal sealed class CrudEndpointHandlers<TEntity, TRequest, TService, TKey>
    where TEntity : class, new()
    where TService : ICrudService<TEntity, TRequest, TKey>
{
    private readonly CrudOptions<TEntity> _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrudEndpointHandlers{TEntity, TRequest, TService, TKey}"/> class
    /// </summary>
    /// <param name="options"></param>
    public CrudEndpointHandlers(CrudOptions<TEntity> options)
    {
        _options = options;
    }

    /// <summary>
    /// Create an entity
    /// </summary>
    /// <param name="request"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<CreatedAtRoute<TKey>> Create(
        [FromBody] TRequest request, 
        [FromServices]TService service)
    {
        var createRequest = new CreateRequest<TRequest>(request);
        return await Create(createRequest, service);
    }
    
    /// <summary>
    /// Create a referenced entity
    /// </summary>
    /// <param name="context"></param>
    /// <param name="request"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<CreatedAtRoute<TKey>> ReferencedCreate<TReferenceKey>(
        HttpContext context,
        [FromBody] TRequest request, 
        [FromServices]TService service)
    {
        var referenceId = context.GetReference<TReferenceKey>(_options.Create);
        if (referenceId == null)
            throw new InvalidEntityReferenceException<TReferenceKey>();
        var createRequest = new CreateRequest<TRequest, TReferenceKey>(referenceId, request);
        return await Create(createRequest, service, referenceId);
    }

    public Delegate GetReferencedCreateDelegate<TReferenceKey>()
    {
        return ReferencedCreate<TReferenceKey>;
    }
    
    /// <summary>
    /// Update an entity
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<Results<Ok, BadRequest>> Update(
        [FromRoute] TKey id, 
        [FromBody] TRequest request, 
        [FromServices]TService service)
    {
        if (id == null) throw new ArgumentNullException(nameof(id));
        var updateRequest = new UpdateRequest<TRequest, TKey>(id, request);
        return await Update(updateRequest, service);
    }
    
    /// <summary>
    /// Update an entity by complex identifier
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public Task<Results<Ok, BadRequest>> UpdateByComplexId(
        [AsParameters] TKey id, 
        [FromBody] TRequest request, 
        [FromServices]TService service)
    {
        return Update(id, request, service);
    }
    
    /// <summary>
    /// Update a referenced entity
    /// </summary>
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<Results<Ok, BadRequest>> ReferencedUpdate<TReferenceKey>(
        HttpContext context,
        [FromRoute] TKey id, 
        [FromBody] TRequest request,
        [FromServices] TService service)
    {
        var referenceId = context.GetReference<TReferenceKey>(_options.Update);
        if (referenceId == null)
            throw new InvalidEntityReferenceException<TReferenceKey>();
        var updateRequest = new UpdateRequest<TRequest, TKey, TReferenceKey>(referenceId, id, request);
        return await Update(updateRequest, service);
    }
    
    public Delegate GetReferencedUpdateDelegate<TReferenceKey>()
    {
        return ReferencedUpdate<TReferenceKey>;
    }
    
    /// <summary>
    /// Update a referenced entity by complex identifier
    /// </summary>
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public Task<Results<Ok, BadRequest>> ReferencedUpdateByComplexId<TReferenceKey>(
        HttpContext context,
        [AsParameters] TKey id, 
        [FromBody] TRequest request,
        [FromServices] TService service)
    {
        return ReferencedUpdate<TReferenceKey>(context, id, request, service);
    }
    
    public Delegate GetReferencedUpdateByComplexIdDelegate<TReferenceKey>()
    {
        return ReferencedUpdateByComplexId<TReferenceKey>;
    }
    
    /// <summary>
    /// Delete an entity
    /// </summary>
    /// <param name="id"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<Results<Ok, BadRequest>> Delete(
        [FromRoute] TKey id,
        [FromServices]TService service)
    {
        if (id == null) throw new ArgumentNullException(nameof(id));
        var request = new ItemRequest<TKey>(id);
        return await Delete(request, service);
    }
    
    /// <summary>
    /// Delete an entity by complex identifier
    /// </summary>
    /// <param name="id"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public Task<Results<Ok, BadRequest>> DeleteByComplexId(
        [AsParameters] TKey id,
        [FromServices]TService service)
    {
        return Delete(id, service);
    }
    
    /// <summary>
    /// Delete a referenced entity
    /// </summary>
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<Results<Ok, BadRequest>> ReferencedDelete<TReferenceKey>(
        HttpContext context,
        [FromRoute] TKey id, 
        [FromServices] TService service)
    {
        var referenceId = context.GetReference<TReferenceKey>(_options.Delete);
        if (referenceId == null)
            throw new InvalidEntityReferenceException<TReferenceKey>();
        var request = new ItemRequest<TKey, TReferenceKey>(referenceId, id);
        return await Delete(request, service);
    }
    
    public Delegate GetReferencedDeleteDelegate<TReferenceKey>()
    {
        return ReferencedDelete<TReferenceKey>;
    }
    
    /// <summary>
    /// Delete a referenced entity by complex identifier
    /// </summary>
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public Task<Results<Ok, BadRequest>> ReferencedDeleteByComplexId<TReferenceKey>(
        HttpContext context,
        [AsParameters] TKey id, 
        [FromServices] TService service)
    {
        return ReferencedDelete<TReferenceKey>(context, id, service);
    }
    
    public Delegate GetReferencedDeleteByComplexIdDelegate<TReferenceKey>()
    {
        return ReferencedDeleteByComplexId<TReferenceKey>;
    }
    
    private async Task<CreatedAtRoute<TKey>> Create(
        CreateRequest<TRequest> request, 
        TService service,
        object? referenceValue = null)
    {
        var result = await service.CreateAsync(request);
        object routeValues = referenceValue != null ? 
            new { referenceId = referenceValue, id = result } : 
            new { id = result };
        return TypedResults.CreatedAtRoute(result, _options.Get.Name, routeValues);
    }
    
    private static async Task<Results<Ok, BadRequest>> Update(
        UpdateRequest<TRequest, TKey> request, 
        TService service)
    {
        var result = await service.UpdateAsync(request);
        return result ? TypedResults.Ok() : TypedResults.BadRequest();
    }
    
    private static async Task<Results<Ok, BadRequest>> Delete(
        ItemRequest<TKey> request, 
        TService service)
    {
        var result = await service.DeleteAsync(request);
        return result ? TypedResults.Ok() : TypedResults.BadRequest();
    }
    
    public Delegate GetReferencedHandler(Type referenceKeyType, string methodName)
    {
        var handler = GetType()
            .GetMethod(methodName)?
            .MakeGenericMethod(referenceKeyType).Invoke(this, null);
        return (Delegate) handler!;
    }
}