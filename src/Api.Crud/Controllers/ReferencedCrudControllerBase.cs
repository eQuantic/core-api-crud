using eQuantic.Core.Api.Crud.Binders;
using eQuantic.Core.Api.Crud.Extensions;
using eQuantic.Core.Application.Crud.Services;
using eQuantic.Core.Domain.Entities;
using eQuantic.Core.Domain.Entities.Requests;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace eQuantic.Core.Api.Crud.Controllers;

/// <summary>
/// CRUD controller base for entities owned by a referenced parent (e.g.
/// <c>/examples/{exampleId}/childExamples</c>). Mirrors the referenced CRUD handlers.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TReferenceEntity">The parent entity (defines the reference route segment name)</typeparam>
/// <typeparam name="TReferenceKey">The parent identifier type</typeparam>
[ApiController]
public abstract class ReferencedCrudControllerBase<TEntity, TRequest, TKey, TReferenceEntity, TReferenceKey>
    : ReferencedReaderControllerBase<TEntity, TKey, TReferenceEntity, TReferenceKey>,
        ICrudController<TEntity, TRequest, TKey>
    where TEntity : class, IDomainEntity, new()
    where TKey : notnull
{
    private readonly ICrudService<TEntity, TRequest, TKey> _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferencedCrudControllerBase{TEntity, TRequest, TKey, TReferenceEntity, TReferenceKey}"/> class
    /// </summary>
    /// <param name="service"></param>
    protected ReferencedCrudControllerBase(ICrudService<TEntity, TRequest, TKey> service) : base(service)
    {
        _service = service;
    }

    /// <summary>
    /// Create a referenced entity
    /// </summary>
    [HttpPost("")]
    public virtual async Task<ActionResult<TKey>> Create(
        [FromBody] TRequest request,
        CancellationToken cancellationToken = default)
    {
        var referenceId = GetReferenceId();
        var result = await _service.CreateAsync(new CreateRequest<TRequest, TReferenceKey>(referenceId, request), cancellationToken);
        return CreatedResult(result, referenceId);
    }

    /// <summary>
    /// Update a referenced entity by identifier
    /// </summary>
    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Update(
        [ModelBinder(typeof(KeyModelBinder))] TKey id,
        [FromBody] TRequest request,
        CancellationToken cancellationToken = default)
    {
        var referenceId = GetReferenceId();
        var result = await _service.UpdateAsync(new UpdateRequest<TRequest, TKey, TReferenceKey>(referenceId, id, request), cancellationToken);
        return result ? Ok() : BadRequest();
    }

    /// <summary>
    /// Delete a referenced entity by identifier
    /// </summary>
    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete(
        [ModelBinder(typeof(KeyModelBinder))] TKey id,
        CancellationToken cancellationToken = default)
    {
        var referenceId = GetReferenceId();
        var result = await _service.DeleteAsync(new ItemRequest<TKey, TReferenceKey>(referenceId, id), cancellationToken);
        return result ? Ok() : BadRequest();
    }

    private ActionResult<TKey> CreatedResult(TKey key, TReferenceKey referenceId)
    {
        var values = new RouteValueDictionary { [ReferenceName] = referenceId };
        if (RoutePatternBuilder.IsPrimitiveKey(typeof(TKey)))
        {
            values["id"] = key;
        }
        else
        {
            foreach (var property in typeof(TKey).GetProperties())
            {
                values[property.Name.Camelize()!] = property.GetValue(key);
            }
        }

        var url = Url.Action(nameof(GetById), values);
        return url != null
            ? Created(url, key)
            : StatusCode(StatusCodes.Status201Created, key);
    }
}
