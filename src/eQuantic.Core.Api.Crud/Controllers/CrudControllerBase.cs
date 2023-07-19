using System.Reflection;
using eQuantic.Core.Application.Crud.Entities.Requests;
using eQuantic.Core.Application.Crud.Services;
using eQuantic.Core.Application.Entities.Results;
using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Api.Crud.Controllers;

/// <summary>
/// The CRUD controller base
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TRequest"></typeparam>
public abstract class CrudControllerBase<TEntity, TRequest> : ControllerBase, ICrudController<TEntity, TRequest> where TEntity : class, new()
{
    private readonly ICrudServiceBase<TEntity, TRequest> _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrudControllerBase{TEntity, TRequest}"/> class
    /// </summary>
    /// <param name="service"></param>
    protected CrudControllerBase(ICrudServiceBase<TEntity, TRequest> service)
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
    public virtual async Task<IActionResult> GetById([FromRoute] ItemRequest request, CancellationToken cancellationToken = default)
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
    public virtual async Task<IActionResult> Update([FromQuery] UpdateRequest<TRequest> request, CancellationToken cancellationToken = default)
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
    public virtual async Task<IActionResult> Delete([FromQuery] ItemRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _service.DeleteAsync(request, cancellationToken);
        return NoContent();
    }
}