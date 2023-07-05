using eQuantic.Core.Application.Crud.Entities.Requests;
using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Api.Crud.Controllers;

/// <summary>
/// CRUD Controller
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface ICrudController<TEntity>
{
    /// <summary>
    /// Get entity by identifier
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<IActionResult> GetById(ItemRequest request);
    /// <summary>
    /// Get paged list of entity
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<IActionResult> GetPagedList(PagedListRequest<TEntity> request);
}