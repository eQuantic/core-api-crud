using eQuantic.Core.Application.Crud.Services;
using eQuantic.Core.Domain.Entities;

namespace eQuantic.Core.Api.Crud.Controllers;

// Concrete, instantiable generic controllers used by the auto-registration (AddCrudControllers).
// The *ControllerBase classes are abstract (meant for explicit subclassing), so a closed generic of
// them is still abstract and cannot be activated by MVC — these concrete shims provide a public
// constructor the controller activator can use.

/// <summary>
/// Concrete read-only controller materialized per entity by the auto-registration.
/// </summary>
public sealed class GenericReaderController<TEntity, TKey> : ReaderControllerBase<TEntity, TKey>
    where TEntity : class, IDomainEntity, new()
    where TKey : notnull
{
    public GenericReaderController(IReaderService<TEntity, TKey> service) : base(service)
    {
    }
}

/// <summary>
/// Concrete CRUD controller materialized per entity by the auto-registration.
/// </summary>
public sealed class GenericCrudController<TEntity, TRequest, TKey> : CrudControllerBase<TEntity, TRequest, TKey>
    where TEntity : class, IDomainEntity, new()
    where TKey : notnull
{
    public GenericCrudController(ICrudService<TEntity, TRequest, TKey> service) : base(service)
    {
    }
}

/// <summary>
/// Concrete referenced read-only controller materialized per entity by the auto-registration.
/// </summary>
public sealed class GenericReferencedReaderController<TEntity, TKey, TReferenceEntity, TReferenceKey>
    : ReferencedReaderControllerBase<TEntity, TKey, TReferenceEntity, TReferenceKey>
    where TEntity : class, IDomainEntity, new()
    where TKey : notnull
{
    public GenericReferencedReaderController(IReaderService<TEntity, TKey> service) : base(service)
    {
    }
}

/// <summary>
/// Concrete referenced CRUD controller materialized per entity by the auto-registration.
/// </summary>
public sealed class GenericReferencedCrudController<TEntity, TRequest, TKey, TReferenceEntity, TReferenceKey>
    : ReferencedCrudControllerBase<TEntity, TRequest, TKey, TReferenceEntity, TReferenceKey>
    where TEntity : class, IDomainEntity, new()
    where TKey : notnull
{
    public GenericReferencedCrudController(ICrudService<TEntity, TRequest, TKey> service) : base(service)
    {
    }
}
