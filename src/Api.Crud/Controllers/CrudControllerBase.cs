using System.Reflection;
using eQuantic.Core.Application.Crud.Services;
using eQuantic.Core.Domain.Entities;
using eQuantic.Core.Domain.Entities.Results;
using eQuantic.Core.Domain.Entities.Requests;
using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Api.Crud.Controllers;

/// <summary>
/// The CRUD controller base
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract class CrudControllerBase<TEntity, TRequest, TKey> : ControllerBase, ICrudController<TEntity, TRequest, TKey> where TEntity : class, IDomainEntity, new()
{
    private readonly ICrudService<TEntity, TRequest, TKey> _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrudControllerBase{TEntity, TRequest, TKey}"/> class
    /// </summary>
    /// <param name="service"></param>
    protected CrudControllerBase(ICrudService<TEntity, TRequest, TKey> service)
    {
        _service = service;
    }

    /// <summary>
    /// Get paged list of entity by criteria
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("")]
    public virtual async Task<IActionResult> GetPagedList([FromQuery] PagedListRequest<TEntity> request, CancellationToken cancellationToken = default)
    {
        var pagedList = await _service.GetPagedListAsync(request, cancellationToken);
        return Ok(new PagedListResult<TEntity>(pagedList));
    }

    /// <summary>
    /// Get entity by identifier
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public virtual async Task<IActionResult> GetById([FromRoute] GetRequest<TKey> request, CancellationToken cancellationToken = default)
    {
        var item = await _service.GetByIdAsync(request, cancellationToken);
        return Ok(item);
    }

    /// <summary>
    /// Create an entity
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("")]
    public virtual async Task<IActionResult> Create([FromQuery] CreateRequest<TRequest> request, CancellationToken cancellationToken = default)
    {
        var result = await _service.CreateAsync(request, cancellationToken);
        var routeName = this.GetType()
            .GetMethod(nameof(GetById))?
            .GetCustomAttribute<HttpGetAttribute>()?
            .Name;
        if (!string.IsNullOrEmpty(routeName))
        {
            return CreatedAtRoute(routeName, new { id = result }, result);
        }

        var actionName = nameof(GetById);
        return CreatedAtAction(actionName, new { id = result }, result);
    }

    /// <summary>
    /// Update an entity by identifier
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:int}")]
    public virtual async Task<IActionResult> Update([FromQuery] UpdateRequest<TRequest, TKey> request, CancellationToken cancellationToken = default)
    {
        var result = await _service.UpdateAsync(request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete an entity by identifier
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public virtual async Task<IActionResult> Delete([FromQuery] ItemRequest<TKey> request, CancellationToken cancellationToken = default)
    {
        var result = await _service.DeleteAsync(request, cancellationToken);
        return NoContent();
    }
}