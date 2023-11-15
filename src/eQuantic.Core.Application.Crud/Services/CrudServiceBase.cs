using eQuantic.Core.Application.Crud.Entities.Requests;
using eQuantic.Core.Application.Entities.Data;
using eQuantic.Core.Application.Exceptions;
using eQuantic.Core.Data.EntityFramework.Repository;
using eQuantic.Core.Data.EntityFramework.Specifications;
using eQuantic.Core.Data.Repository;
using eQuantic.Linq.Filter;
using eQuantic.Linq.Specification;
using eQuantic.Mapper;
using Microsoft.Extensions.Logging;

namespace eQuantic.Core.Application.Crud.Services;

public abstract class CrudServiceBase<TEntity, TRequest, TDataEntity, TUser> : CrudServiceBase<TEntity, TRequest, TDataEntity, TUser, int>
    where TEntity : class, new()
    where TDataEntity : class, IEntity<int>, new()
{
    protected CrudServiceBase(
        IDefaultUnitOfWork unitOfWork, 
        IMapperFactory mapperFactory, 
        ILogger logger) : base(unitOfWork, mapperFactory, logger)
    {
    }
}
public abstract class CrudServiceBase<TEntity, TRequest, TDataEntity, TUser, TKey> 
    : ReaderServiceBase<TEntity, TDataEntity, TKey>, ICrudService<TEntity, TRequest, TKey>
    where TEntity : class, new()
    where TDataEntity : class, IEntity<TKey>, new()
{
    protected CrudServiceBase(
        IDefaultUnitOfWork unitOfWork, 
        IMapperFactory mapperFactory, 
        ILogger logger) : base(unitOfWork, mapperFactory, logger)
    {
    }
    
    public virtual async Task<TKey> CreateAsync(CreateRequest<TRequest> request, CancellationToken cancellationToken = default)
    {
        var item = OnMapRequest(request.Body);
        if (item == null)
        {
            Logger.LogError("{ServiceName} - Create: Bad request of {Name}", GetType().Name, typeof(TRequest).Name);
            throw new InvalidEntityRequestException();
        }

        await OnAfterCreateAsync(item, cancellationToken);
        
        if (request is IReferencedRequest<int> referencedRequest && item is IWithReferenceId<TDataEntity, int> referencedItem)
        {
            referencedItem.SetReferenceId(referencedRequest.ReferenceId);
        }

        if (item is IEntityOwned<TUser> itemWithOwner)
        {
            itemWithOwner.CreatedAt = DateTime.UtcNow;
        }

        await Repository.AddAsync(item);
        await Repository.UnitOfWork.CommitAsync(cancellationToken);
        await OnBeforeCreateAsync(item, cancellationToken);
        
        return item.GetKey();
    }

    public virtual async Task<bool> UpdateAsync(UpdateRequest<TRequest, TKey> request, CancellationToken cancellationToken = default)
    {
        var item = await GetItem(request, cancellationToken);
        if (item == null)
        {
            var ex = new EntityNotFoundException<TKey>(request.Id);
            Logger.LogError(ex, "{ServiceName} - Create: Entity of {Name} not found", GetType().Name, typeof(TEntity).Name);
            throw ex;
        }

        await OnAfterUpdateAsync(item, cancellationToken);
        
        OnMapRequest(request.Body, item);

        if (item is IEntityTrack<TUser> itemWithTrack)
        {
            itemWithTrack.UpdatedAt = DateTime.UtcNow;
        }

        await Repository.ModifyAsync(item);
        await Repository.UnitOfWork.CommitAsync(cancellationToken);
        await OnBeforeUpdateAsync(item, cancellationToken);
        
        return true;
    }

    public virtual async Task<bool> DeleteAsync(ItemRequest<TKey> request, CancellationToken cancellationToken = default)
    {
        var item = await GetItem(request, cancellationToken);
        if (item == null)
        {
            var ex = new EntityNotFoundException<TKey>(request.Id);
            Logger.LogError(ex, "{ServiceName} - Delete: Entity of {Name} not found", GetType().Name, typeof(TEntity).Name);
            throw ex;
        }
            

        await OnAfterDeleteAsync(item, cancellationToken);
        
        if (item is IEntityHistory<TUser> itemWithHistory)
        {
            itemWithHistory.DeletedAt = DateTime.UtcNow;
            await Repository.ModifyAsync(item);
        }
        else
        {
            await Repository.RemoveAsync(item);
        }

        await Repository.UnitOfWork.CommitAsync(cancellationToken);
        await OnBeforeDeleteAsync(item, cancellationToken);
        
        return true;
    }

    protected virtual TDataEntity? OnMapRequest(TRequest? request, TDataEntity? dataEntity = null)
    {
        var mapper = MapperFactory.GetMapper<TRequest, TDataEntity>();

        if (mapper != null) 
            return mapper.Map(request, dataEntity);
        
        var ex = new DependencyNotFoundException(typeof(IMapper<TRequest, TDataEntity>));
        Logger.LogError(ex, "{ServiceName} - OnMapRequest: Mapper not found", GetType().Name);
        throw ex;
    }

    protected virtual Task OnAfterCreateAsync(TDataEntity? dataEntity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task OnBeforeCreateAsync(TDataEntity? dataEntity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task OnAfterUpdateAsync(TDataEntity? dataEntity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task OnBeforeUpdateAsync(TDataEntity? dataEntity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task OnAfterDeleteAsync(TDataEntity? dataEntity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task OnBeforeDeleteAsync(TDataEntity? dataEntity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    private async Task<TDataEntity?> GetItem(ItemRequest<TKey> request, CancellationToken cancellationToken = default)
    {
        Specification<TDataEntity> specification = new GetEntityByIdSpecification<TDataEntity, TKey>(request.Id, UnitOfWork as UnitOfWork);
        var filterByRef = GetReferenceFiltering(request);
        if (filterByRef != null)
        {
            specification = specification &&
                            new EntityFilterSpecification<TDataEntity>(new IFiltering[] { filterByRef });
        }

        return await Repository.GetFirstAsync(specification, opt => { }, cancellationToken);
    }

    private static IFiltering<TDataEntity>? GetReferenceFiltering<TAnyRequest>(TAnyRequest request)
    {
        if (request is not IReferencedRequest<int> referencedRequest)
            return null;

        var dataEntity = new TDataEntity();
        if (dataEntity is not IWithReferenceId<TDataEntity, int> referencedDataEntity)
            return null;

        referencedDataEntity.SetReferenceId(referencedRequest.ReferenceId);
        var filterByRef = referencedDataEntity.GetReferenceFiltering();

        return filterByRef;
    }
}