using System.Reflection;
using eQuantic.Core.Api.Crud.Binders;
using eQuantic.Core.Api.Crud.Extensions;
using eQuantic.Core.Api.Crud.Options;
using eQuantic.Core.Application.Crud.Services;
using eQuantic.Core.Application.Entities.Results;
using eQuantic.Core.Domain.Entities.Requests;
using eQuantic.Core.Exceptions;
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
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<Results<Ok<TEntity>, NotFound>> GetReferencedById<TReferenceKey>(
        HttpContext context,
        [FromRoute] TKey id, 
        [FromServices]TService service)
    {
        var referenceId = context.GetReference<TReferenceKey>(_options.Get);
        if (referenceId == null)
            throw new InvalidEntityReferenceException<TReferenceKey>();
        
        var request = new ItemRequest<TKey, TReferenceKey>(referenceId, id);
        return await GetById(request, service);
    }
    
    public Delegate GetReferencedByIdDelegate<TReferenceKey>()
    {
        return GetReferencedById<TReferenceKey>;
    }
    
    /// <summary>
    /// Get referenced entity by complex identifier
    /// </summary>
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public Task<Results<Ok<TEntity>, NotFound>> GetReferencedByComplexId<TReferenceKey>(
        HttpContext context,
        [AsParameters] TKey id, 
        [FromServices]TService service)
    {
        return GetReferencedById<TReferenceKey>(context, id, service);
    }
    
    public Delegate GetReferencedByComplexIdDelegate<TReferenceKey>()
    {
        return GetReferencedByComplexId<TReferenceKey>;
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
    /// <param name="context"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="filterBy"></param>
    /// <param name="orderBy"></param>
    /// <param name="service"></param>
    /// <returns></returns>
    public async Task<Ok<PagedListResult<TEntity>>> GetReferencedPagedList<TReferenceKey>(
        HttpContext context,
        [FromQuery] int? pageIndex, 
        [FromQuery] int? pageSize, 
        [FromQuery] IFiltering[]? filterBy, 
        [FromQuery] ISorting[]? orderBy, 
        [FromServices]TService service)
    {
        var referenceId = context.GetReference<TReferenceKey>(_options.List);
        if (referenceId == null)
            throw new InvalidEntityReferenceException<TReferenceKey>();
        var request = new PagedListRequest<TEntity,TReferenceKey>(referenceId, pageIndex, pageSize, filterBy, orderBy);
        return await GetPagedList(request, service);
    }
    
    public Delegate GetReferencedPagedListDelegate<TReferenceKey>()
    {
        return GetReferencedPagedList<TReferenceKey>;
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
        [FromQuery] FilteringCollection? filterBy, 
        [FromQuery] ISorting[]? orderBy, 
        [FromServices]TService service)
    {
        var request = new PagedListRequest<TEntity>(pageIndex, pageSize, filterBy?.ToArray(), orderBy);
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
    
    public Delegate GetReferencedHandler(Type referenceKeyType, string methodName)
    {
        var handler = GetType()
            .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance)?
            .MakeGenericMethod(referenceKeyType).Invoke(this, null);
        return (Delegate) handler!;
    }
}