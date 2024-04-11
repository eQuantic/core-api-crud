using System.Reflection;
using eQuantic.Core.Application.Crud.Enums;
using eQuantic.Core.Application.Crud.Options;
using eQuantic.Core.Application.Entities.Data;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Domain.Entities.Requests;
using eQuantic.Core.Exceptions;
using eQuantic.Mapper;
using Microsoft.Extensions.Logging;

namespace eQuantic.Core.Application.Crud.Services;

public abstract class CrudServiceBase<TEntity, TRequest, TDataEntity, TUser> : CrudServiceBase<TEntity, TRequest, TDataEntity, TUser, int, int>
    where TEntity : class, new()
    where TDataEntity : class, IEntity<int>, new()
{
    protected CrudServiceBase(
        IApplicationContext<int> applicationContext,
        IQueryableUnitOfWork unitOfWork, 
        IMapperFactory mapperFactory,
        ILogger logger, Action<ReadOptions>? options = null) 
        : base(applicationContext, unitOfWork, mapperFactory, logger, options)
    {
    }
}
public abstract class CrudServiceBase<TEntity, TRequest, TDataEntity, TUser, TKey, TUserKey> 
    : ReaderServiceBase<TEntity, TDataEntity, TKey, TUserKey>, ICrudService<TEntity, TRequest, TKey>
    where TEntity : class, new()
    where TDataEntity : class, IEntity<TKey>, new()
    where TUserKey : struct
{
    protected CrudServiceBase(
        IApplicationContext<TUserKey> applicationContext,
        IQueryableUnitOfWork unitOfWork, 
        IMapperFactory mapperFactory, 
        ILogger logger, Action<ReadOptions>? options = null) 
        : base(applicationContext, unitOfWork, mapperFactory, logger, options)
    {
    }
    
    public virtual async Task<TKey> CreateAsync(CreateRequest<TRequest> request, CancellationToken cancellationToken = default)
    {
        var item = OnMapRequest(CrudAction.Create, request.Body);
        if (item == null)
        {
            Logger.LogError("{ServiceName} - Create: Bad request of {Name}", GetType().Name, typeof(TRequest).Name);
            throw new InvalidEntityRequestException();
        }
        
        SetReferenceKey(request, item);
        await OnBeforeCreateAsync(request, item, cancellationToken);
        
        if (item is IEntityTimeMark itemWithTimeMark)
        {
            itemWithTimeMark.CreatedAt = DateTime.UtcNow;
        }

        if (item is IEntityOwned<TUser, TUserKey> itemWithOwner)
        {
            var userId = await ApplicationContext.GetCurrentUserIdAsync();
            itemWithOwner.CreatedById = userId;
        }
        
        await Repository.AddAsync(item);
        await Repository.UnitOfWork.CommitAsync(cancellationToken);
        await OnAfterCreateAsync(item, cancellationToken);
        
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

        await OnCheckPermissionsAsync(CrudAction.Update, item, cancellationToken);

        await OnBeforeUpdateAsync(request, item, cancellationToken);
        
        OnMapRequest(CrudAction.Update, request.Body, item);

        if (item is IEntityTimeTrack itemWithTimeTrack)
        {
            itemWithTimeTrack.UpdatedAt = DateTime.UtcNow;
        }
        
        if (item is IEntityTrack<TUser, TUserKey> itemWithTrack)
        {
            var userId = await ApplicationContext.GetCurrentUserIdAsync();
            itemWithTrack.UpdatedById = userId;
        }

        await Repository.ModifyAsync(item);
        await Repository.UnitOfWork.CommitAsync(cancellationToken);
        await OnAfterUpdateAsync(item, cancellationToken);
        
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
        
        await OnCheckPermissionsAsync(CrudAction.Delete, item, cancellationToken);
        
        await OnBeforeDeleteAsync(item, cancellationToken);

        var softDelete = false;
        if (item is IEntityTimeEnded itemWithTimeEnded)
        {
            softDelete = true;
            itemWithTimeEnded.DeletedAt = DateTime.UtcNow;
        }
        
        if (item is IEntityHistory<TUser, TUserKey> itemWithHistory)
        {
            softDelete = true;
            var userId = await ApplicationContext.GetCurrentUserIdAsync();
            itemWithHistory.DeletedById = userId;
        }

        if (softDelete)
        {
            await Repository.ModifyAsync(item);
        }
        else
        {
            await Repository.RemoveAsync(item);
        }

        await Repository.UnitOfWork.CommitAsync(cancellationToken);
        await OnAfterDeleteAsync(item, cancellationToken);
        
        return true;
    }

    protected virtual TDataEntity? OnMapRequest(CrudAction action, TRequest? request, TDataEntity? dataEntity = null)
    {
        var mapper = MapperFactory.GetMapper<TRequest, TDataEntity>();

        if (mapper != null) 
            return mapper.Map(request, dataEntity);
        
        var ex = new DependencyNotFoundException(typeof(IMapper<TRequest, TDataEntity>));
        Logger.LogError(ex, "{ServiceName} - OnMapRequest: Mapper not found", GetType().Name);
        throw ex;
    }

    protected virtual Task OnBeforeCreateAsync(CreateRequest<TRequest> request, TDataEntity? dataEntity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task OnAfterCreateAsync(TDataEntity? dataEntity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task OnBeforeUpdateAsync(UpdateRequest<TRequest, TKey> request, TDataEntity? dataEntity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task OnAfterUpdateAsync(TDataEntity? dataEntity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task OnBeforeDeleteAsync(TDataEntity? dataEntity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task OnAfterDeleteAsync(TDataEntity? dataEntity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    private async Task<TDataEntity?> GetItem(ItemRequest<TKey> request, CancellationToken cancellationToken = default)
    {
        var item = await Repository.GetAsync(request.Id, cancellationToken);
        ValidateReference(request, item);
        return item;
    }

    private void ValidateReference(BasicRequest request, TDataEntity dataEntity)
    {
        var referenceType = request.GetReferenceType();
        if (referenceType == null)
            return;
        
        typeof(CrudServiceBase<TEntity, TRequest, TDataEntity, TUser, TKey, TUserKey>)
            .GetMethod(nameof(ValidateReference), BindingFlags.NonPublic | BindingFlags.Static)?
            .MakeGenericMethod(referenceType)
            .Invoke(null, [request]);
    }

    private static void ValidateReference<TReferenceKey>(BasicRequest request)
    {
        if (request is not IReferencedRequest<TReferenceKey> referencedRequest)
            return;

        var dataEntity = new TDataEntity();
        if (dataEntity is not IWithReferenceId<TDataEntity, TReferenceKey> referencedDataEntity)
            return;

        var result = referencedDataEntity.GetReferenceId()?.Equals(referencedRequest.ReferenceId);
        if (result.HasValue && !result.Value)
            throw new InvalidEntityReferenceException<TReferenceKey>(referencedRequest.ReferenceId!);
    }

    private void SetReferenceKey(BasicRequest request, TDataEntity? dataEntity)
    {
        var referenceType = request.GetReferenceType();
        if (referenceType == null)
            return;
        
        typeof(CrudServiceBase<TEntity, TRequest, TDataEntity, TUser, TKey, TUserKey>)
            .GetMethod(nameof(SetReferenceKey), BindingFlags.NonPublic | BindingFlags.Static)?
            .MakeGenericMethod(referenceType)
            .Invoke(null, [request, dataEntity]);
    }
    
    private static void SetReferenceKey<TReferenceKey>(CreateRequest<TRequest> request, TDataEntity? dataEntity)
    {
        if (request is not IReferencedRequest<TReferenceKey> referencedRequest ||
            dataEntity is not IWithReferenceId<TDataEntity, TReferenceKey> referencedItem)
            return;
        if(referencedRequest.ReferenceId != null)
            referencedItem.SetReferenceId(referencedRequest.ReferenceId);
    }
}