using eQuantic.Core.Domain.Entities;
using eQuantic.Core.Domain.Entities.Requests;
using eQuantic.Core.Domain.Entities.Results;
using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Api.Crud.Controllers;

/// <summary>
/// Read-only controller contract (mirrors the Minimal API reader endpoints).
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
public interface IReaderController<TEntity, TKey>
    where TEntity : class, IDomainEntity, new()
    where TKey : notnull
{
    /// <summary>
    /// Get entity by identifier
    /// </summary>
    Task<ActionResult<TEntity>> GetById(TKey id, string[]? includeFields, CancellationToken cancellationToken);

    /// <summary>
    /// Get paged list of entity by criteria
    /// </summary>
    Task<ActionResult<PagedListResult<TEntity>>> GetPagedList(
        int? pageIndex,
        int? pageSize,
        FilteringCollection<TEntity>? filterBy,
        SortingCollection<TEntity>? orderBy,
        string[]? includeFields,
        CancellationToken cancellationToken);
}
