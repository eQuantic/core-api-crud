using eQuantic.Core.Api.Crud.Binders;
using eQuantic.Core.Api.Crud.Extensions;
using eQuantic.Core.Application.Crud.Services;
using eQuantic.Core.Domain.Entities;
using eQuantic.Core.Domain.Entities.Requests;
using eQuantic.Core.Domain.Entities.Results;
using eQuantic.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Api.Crud.Controllers;

/// <summary>
/// Read-only controller base for entities owned by a referenced parent (e.g.
/// <c>/examples/{exampleId}/childExamples</c>). Mirrors the referenced reader handlers.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TReferenceEntity">The parent entity (defines the reference route segment name)</typeparam>
/// <typeparam name="TReferenceKey">The parent identifier type</typeparam>
[ApiController]
public abstract class ReferencedReaderControllerBase<TEntity, TKey, TReferenceEntity, TReferenceKey>
    : ControllerBase, IReaderController<TEntity, TKey>
    where TEntity : class, IDomainEntity, new()
    where TKey : notnull
{
    private readonly IReaderService<TEntity, TKey> _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferencedReaderControllerBase{TEntity, TKey, TReferenceEntity, TReferenceKey}"/> class
    /// </summary>
    /// <param name="service"></param>
    protected ReferencedReaderControllerBase(IReaderService<TEntity, TKey> service)
    {
        _service = service;
    }

    /// <summary>
    /// The reference route segment name (defaults to <c>{referenceEntity}Id</c>).
    /// </summary>
    protected virtual string ReferenceName => typeof(TReferenceEntity).GetReferenceName();

    /// <summary>
    /// Get referenced entity by identifier
    /// </summary>
    [HttpGet("{id}")]
    public virtual async Task<ActionResult<TEntity>> GetById(
        [ModelBinder(typeof(KeyModelBinder))] TKey id,
        [FromQuery] string[]? includeFields = null,
        CancellationToken cancellationToken = default)
    {
        var referenceId = GetReferenceId();
        var request = new GetRequest<TKey, TReferenceKey>(referenceId, id, includeFields);
        var result = await _service.GetByIdAsync(request, cancellationToken);
        return result != null ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Get paged list of referenced entity by criteria
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
        var referenceId = GetReferenceId();
        var request = new PagedListRequest<TEntity, TReferenceKey>(referenceId, pageIndex, pageSize, filterBy, orderBy, includeFields);
        var result = await _service.GetPagedListAsync(request, cancellationToken);
        return Ok(new PagedListResult<TEntity>(result));
    }

    /// <summary>
    /// Resolves the referenced parent identifier from the route.
    /// </summary>
    protected TReferenceKey GetReferenceId()
    {
        var referenceId = HttpContext.GetReference<TReferenceKey>(ReferenceName);
        if (referenceId == null)
            throw new InvalidEntityReferenceException<TReferenceKey>();
        return referenceId;
    }
}
