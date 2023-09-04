using eQuantic.Core.Application.Crud.Entities.Requests;
using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Api.Crud.Controllers;

/// <summary>
/// CRUD Controller
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TKey"></typeparam>
public interface ICrudController<TEntity, TRequest, TKey> where TEntity : class, new()
{
    /// <summary>
    /// Get entity by identifier
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResult> GetById(ItemRequest<TKey> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paged list of entity
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResult> GetPagedList(PagedListRequest<TEntity> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create an entity
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResult> Create([FromQuery] CreateRequest<TRequest> request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an entity by identifier
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResult> Update([FromQuery] UpdateRequest<TRequest, TKey> request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an entity by identifier
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResult> Delete([FromQuery] ItemRequest<TKey> request, CancellationToken cancellationToken = default);
}