using System.Reflection;
using eQuantic.Core.Application.Crud.Enums;
using eQuantic.Core.Application.Crud.Options;
using eQuantic.Core.Application.Entities.Data;
using eQuantic.Core.Application.Extensions;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Domain.Entities.Requests;
using eQuantic.Core.Exceptions;
using eQuantic.Mapper;
using Microsoft.Extensions.Logging;

namespace eQuantic.Core.Application.Crud.Services;

public abstract class CrudServiceBase<TEntity, TRequest, TDataEntity, TUser>(
    IApplicationContext<int> applicationContext,
    IQueryableUnitOfWork unitOfWork,
    IMapperFactory mapperFactory,
    ILogger logger,
    Action<ReadOptions>? options = null)
    : CrudServiceBase<TEntity, TRequest, TDataEntity, TUser, int, int>(applicationContext, unitOfWork, mapperFactory,
        logger, options)
    where TEntity : class, new()
    where TDataEntity : class, IEntity<int>, new();

public abstract class CrudServiceBase<TEntity, TRequest, TDataEntity, TUser, TKey, TUserKey>(
    IApplicationContext<TUserKey> applicationContext,
    IQueryableUnitOfWork unitOfWork,
    IMapperFactory mapperFactory,
    ILogger logger,
    Action<ReadOptions>? options = null)
    : ReaderServiceBase<TEntity, TDataEntity, TKey, TUserKey>(applicationContext, unitOfWork, mapperFactory, logger,
        options), ICrudService<TEntity, TRequest, TKey>
    where TEntity : class, new()
    where TDataEntity : class, IEntity<TKey>, new()
    where TUserKey : struct
{
    public virtual async Task<TKey> CreateAsync(CreateRequest<TRequest> request, CancellationToken cancellationToken = default)
    {
        var item = await OnMapRequestAsync(CrudAction.Create, request.Body, cancellationToken: cancellationToken);
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
        
        await OnMapRequestAsync(CrudAction.Update, request.Body, item, cancellationToken: cancellationToken);

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

    protected virtual Task<TDataEntity?> OnMapRequestAsync(
        CrudAction action, 
        TRequest? request, 
        TDataEntity? dataEntity = null,
        MappingPriority mappingPriority = MappingPriority.SyncOrAsync,
        CancellationToken cancellationToken = default)
    {
        return Map(request, dataEntity, mappingPriority, cancellationToken);
    }
    
    /// <summary>
    /// This method is invoked after the request entity has been mapped
    /// </summary>
    /// <param name="request"></param>
    /// <param name="dataEntity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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
            .InvokePrivateStaticMethod(nameof(ValidateReference), referenceType, [request, dataEntity]);
    }

    private static void ValidateReference<TReferenceKey>(BasicRequest request, TDataEntity item)
    {
        if (request is not IReferencedRequest<TReferenceKey> referencedRequest ||
            item is not IWithReferenceId<TDataEntity, TReferenceKey> referencedItem)
            return;
        
        if (referencedItem.GetReferenceId()?.Equals(referencedRequest.GetReferenceId()) == false)
        {
            throw new InvalidEntityReferenceException<TReferenceKey>(referencedRequest.GetReferenceId()!);
        }
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
        
        if(referencedRequest.GetReferenceId() != null)
            referencedItem.SetReferenceId(referencedRequest.GetReferenceId()!);
    }
}