using eQuantic.Core.Api.Crud.Binders;
using eQuantic.Core.Application.Crud.Services;
using eQuantic.Core.Domain.Entities;
using eQuantic.Core.Domain.Entities.Requests;
using eQuantic.Core.Domain.Entities.Results;
using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Api.Crud.Controllers;

/// <summary>
/// Read-only controller base. Exposes <c>GetById</c> and <c>GetPagedList</c> mirroring
/// <see cref="Handlers.ReaderEndpointHandlers{TEntity,TService,TKey}"/>.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
[ApiController]
public abstract class ReaderControllerBase<TEntity, TKey> : ControllerBase, IReaderController<TEntity, TKey>
    where TEntity : class, IDomainEntity, new()
    where TKey : notnull
{
    private readonly IReaderService<TEntity, TKey> _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReaderControllerBase{TEntity, TKey}"/> class
    /// </summary>
    /// <param name="service"></param>
    protected ReaderControllerBase(IReaderService<TEntity, TKey> service)
    {
        _service = service;
    }

    /// <summary>
    /// Get entity by identifier
    /// </summary>
    [HttpGet("{id}")]
    public virtual async Task<ActionResult<TEntity>> GetById(
        [ModelBinder(typeof(KeyModelBinder))] TKey id,
        [FromQuery] string[]? includeFields = null,
        CancellationToken cancellationToken = default)
    {
        var request = new GetRequest<TKey>(id, includeFields);
        var result = await _service.GetByIdAsync(request, cancellationToken);
        return result != null ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Get paged list of entity by criteria
    /// </summary>
    [HttpGet("")]
    public virtual async Task<ActionResult<PagedListResult<TEntity>>> GetPagedList(
        [FromQuery] int? pageIndex = null,
        [FromQuery] int? pageSize = null,
        [FromQuery] FilteringCollection<TEntity>? filterBy = null,
        [FromQuery] SortingCollection<TEntity>? orderBy = null,
        [FromQuery] string[]? includeFields = null,
        CancellationToken cancellationToken = default)
    {
        var request = new PagedListRequest<TEntity>(pageIndex, pageSize, filterBy, orderBy, includeFields);
        var result = await _service.GetPagedListAsync(request, cancellationToken);
        return Ok(new PagedListResult<TEntity>(result));
    }
}
