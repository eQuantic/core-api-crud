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
public abstract class CrudControllerBase<TEntity, TRequest> : ControllerBase where TEntity : class, new()
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
    /// <returns></returns>
    [HttpGet("")]
    public virtual async Task<IActionResult> GetPagedList([FromQuery] PagedListRequest<TEntity> request)
    {
        var pagedList = await _service.GetPagedListAsync(request);
        return Ok(new PagedListResult<TEntity>(pagedList));
    }
    
    /// <summary>
    /// Get entity by identifier
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public virtual async Task<IActionResult> GetById([FromRoute] ItemRequest request)
    {
        var item = await _service.GetByIdAsync(request);
        return Ok(item);
    }

    [HttpPost("")]
    public virtual async Task<IActionResult> Create([FromQuery] CreateRequest<TRequest> request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    [HttpPut("{id:int}")]
    public virtual async Task<IActionResult> Update([FromQuery] UpdateRequest<TRequest> request)
    {
        var result = await _service.UpdateAsync(request);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public virtual async Task<IActionResult> Delete([FromQuery] ItemRequest request)
    {
        var result = await _service.DeleteAsync(request);
        return NoContent();
    }
}