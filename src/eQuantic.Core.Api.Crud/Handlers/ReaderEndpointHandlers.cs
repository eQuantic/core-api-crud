using eQuantic.Core.Api.Crud.Options;
using eQuantic.Core.Application.Crud.Entities.Requests;
using eQuantic.Core.Application.Crud.Services;
using eQuantic.Core.Application.Entities.Results;
using eQuantic.Linq.Filter;
using eQuantic.Linq.Sorter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Api.Crud.Handlers;

internal class ReaderEndpointHandlers<TEntity, TService, TKey>
    where TEntity : class, new()
    where TService : IReaderService<TEntity, TKey>
{
    private readonly CrudOptions<TEntity> _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReaderEndpointHandlers{TEntity, TService, TKey}"/> class
    /// </summary>
    /// <param name="options"></param>
    public ReaderEndpointHandlers(CrudOptions<TEntity> options)
    {
        _options = options;
    }
    
    /// <summary>
    /// Get referenced entity by identifier
    /// </summary>
    /// <param name="referenceId"></param>
    /// <param name="id"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<Results<Ok<TEntity>, NotFound>> GetReferencedById(
        [FromRoute] int referenceId, 
        [FromRoute] TKey id, 
        [FromServices]TService service)
    {
        var request = new ItemRequest<TKey, int>(referenceId, id);
        return await GetById(request, service);
    }
    
    /// <summary>
    /// Get referenced entity by complex identifier
    /// </summary>
    /// <param name="referenceId"></param>
    /// <param name="id"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public Task<Results<Ok<TEntity>, NotFound>> GetReferencedByComplexId(
        [FromRoute] int referenceId, 
        [AsParameters] TKey id, 
        [FromServices]TService service)
    {
        return GetReferencedById(referenceId, id, service);
    }
    
    /// <summary>
    /// Get entity by identifier
    /// </summary>
    /// <param name="id"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<Results<Ok<TEntity>, NotFound>> GetById(
        [FromRoute] TKey id, 
        [FromServices]TService service)
    {
        var request = new ItemRequest<TKey>(id);
        return await GetById(request, service);
    }

    /// <summary>
    /// Get entity by complex identifier
    /// </summary>
    /// <param name="id"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public Task<Results<Ok<TEntity>, NotFound>> GetByComplexId(
        [AsParameters] TKey id, 
        [FromServices]TService service)
    {
        return GetById(id, service);
    }
    
    /// <summary>
    /// Get paged list of referenced entity by criteria
    /// </summary>
    /// <param name="referenceId"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="filterBy"></param>
    /// <param name="orderBy"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<Ok<PagedListResult<TEntity>>> GetReferencedPagedList(
        [FromRoute] int referenceId, 
        [FromQuery] int? pageIndex, 
        [FromQuery] int? pageSize, 
        [FromQuery] IFiltering[]? filterBy, 
        [FromQuery] ISorting[]? orderBy, 
        [FromServices]TService service)
    {
        var request = new PagedListRequest<TEntity,int>(referenceId, pageIndex, pageSize, filterBy, orderBy);
        return await GetPagedList(request, service);
    }
    
    /// <summary>
    /// Get paged list of entity by criteria
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="filterBy"></param>
    /// <param name="orderBy"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<Ok<PagedListResult<TEntity>>> GetPagedList(
        [FromQuery] int? pageIndex, 
        [FromQuery] int? pageSize, 
        [FromQuery] IFiltering[]? filterBy, 
        [FromQuery] ISorting[]? orderBy, 
        [FromServices]TService service)
    {
        var request = new PagedListRequest<TEntity>(pageIndex, pageSize, filterBy, orderBy);
        return await GetPagedList(request, service);
    }
    
    private static async Task<Results<Ok<TEntity>, NotFound>> GetById(ItemRequest<TKey> request, TService service)
    {
        var result = await service.GetByIdAsync(request);
        if (result != null)
            return TypedResults.Ok(result);

        return TypedResults.NotFound();
    }

    private static async Task<Ok<PagedListResult<TEntity>>> GetPagedList(
        PagedListRequest<TEntity> request,
        TService service)
    {
        var result = await service.GetPagedListAsync(request);
        return TypedResults.Ok(new PagedListResult<TEntity>(result));
    }

}