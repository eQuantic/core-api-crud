using eQuantic.Core.Api.Crud.Options;
using eQuantic.Core.Application.Crud.Entities.Requests;
using eQuantic.Core.Application.Crud.Services;
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
    /// Initializes a new instance of the <see cref="CrudEndpointHandlers{TEntity, TRequest, TService}"/> class
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
    /// <param name="referenceId"></param>
    /// <param name="request"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<CreatedAtRoute<TKey>> ReferencedCreate(
        [FromRoute] int referenceId, 
        [FromBody] TRequest request, 
        [FromServices]TService service)
    {
        var createRequest = new CreateRequest<TRequest, int>(referenceId, request);
        return await Create(createRequest, service);
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
        var updateRequest = new UpdateRequest<TRequest, TKey>(id, request);
        return await Update(updateRequest, service);
    }
    
    /// <summary>
    /// Update a referenced entity
    /// </summary>
    /// <param name="referenceId"></param>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<Results<Ok, BadRequest>> ReferencedUpdate(
        [FromRoute] int referenceId, 
        [FromRoute] TKey id, 
        [FromBody] TRequest request,
        [FromServices] TService service)
    {
        var updateRequest = new UpdateRequest<TRequest, TKey, int>(referenceId, id, request);
        return await Update(updateRequest, service);
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
        var request = new ItemRequest<TKey>(id);
        return await Delete(request, service);
    }
    
    /// <summary>
    /// Delete a referenced entity
    /// </summary>
    /// <param name="referenceId"></param>
    /// <param name="id"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<Results<Ok, BadRequest>> ReferencedDelete(
        [FromRoute] int referenceId, 
        [FromRoute] TKey id, 
        [FromServices] TService service)
    {
        var request = new ItemRequest<TKey, int>(referenceId, id);
        return await Delete(request, service);
    }
    
    private async Task<CreatedAtRoute<TKey>> Create(
        CreateRequest<TRequest> request, 
        TService service)
    {
        var result = await service.CreateAsync(request);
        object routeValues = request is CreateRequest<TRequest, int> referencedRequest ? 
            new { referenceId = referencedRequest.ReferenceId, id = result } : 
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
}