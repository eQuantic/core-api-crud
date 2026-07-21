using eQuantic.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Api.Crud.Controllers;

/// <summary>
/// CRUD controller contract (mirrors the Minimal API CRUD endpoints).
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TKey"></typeparam>
public interface ICrudController<TEntity, TRequest, TKey> : IReaderController<TEntity, TKey>
    where TEntity : class, IDomainEntity, new()
    where TKey : notnull
{
    /// <summary>
    /// Create an entity
    /// </summary>
    Task<ActionResult<TKey>> Create(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Update an entity by identifier
    /// </summary>
    Task<IActionResult> Update(TKey id, TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Delete an entity by identifier
    /// </summary>
    Task<IActionResult> Delete(TKey id, CancellationToken cancellationToken);
}
