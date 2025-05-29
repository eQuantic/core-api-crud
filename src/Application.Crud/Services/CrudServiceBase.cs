using System.Reflection;
using eQuantic.Core.Application.Crud.Enums;
using eQuantic.Core.Application.Crud.Options;
using eQuantic.Core.Application.Entities.Data;
using eQuantic.Core.Application.Extensions;
using eQuantic.Core.Application.Services;
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
    protected CrudServiceBase(IApplicationContext<int> applicationContext,
        IQueryableUnitOfWork unitOfWork,
        IDateTimeProviderService dateTimeProviderService,
        IMapperFactory mapperFactory,
        ILogger logger,
        Action<ReadOptions>? options = null) : base(applicationContext, 
        unitOfWork, 
        dateTimeProviderService, 
        mapperFactory,
        logger, options)
    {
    }
    
    protected CrudServiceBase(IApplicationContext<int> applicationContext,
        IAsyncQueryableRepository<IQueryableUnitOfWork, TDataEntity, int> repository,
        IDateTimeProviderService dateTimeProviderService,
        IMapperFactory mapperFactory,
        ILogger logger,
        Action<ReadOptions>? options = null) : base(applicationContext, 
        repository, 
        dateTimeProviderService, 
        mapperFactory,
        logger, options)
    {
    }
}

public abstract class CrudServiceBase<TEntity, TRequest, TDataEntity, TUser, TKey, TUserKey> : ReaderServiceBase<TEntity, TDataEntity, TKey, TUserKey>, ICrudService<TEntity, TRequest, TKey>
    where TEntity : class, new()
    where TDataEntity : class, IEntity<TKey>, new()
    where TUserKey : struct
{
    protected IDateTimeProviderService DateTimeProviderService { get; }
    
    protected CrudServiceBase(IApplicationContext<TUserKey> applicationContext,
        IQueryableUnitOfWork unitOfWork,
        IDateTimeProviderService dateTimeProviderService,
        IMapperFactory mapperFactory,
        ILogger logger,
        Action<ReadOptions>? options = null) : base(applicationContext, unitOfWork, mapperFactory, logger,
        options)
    {
        DateTimeProviderService = dateTimeProviderService;
    }
    
    protected CrudServiceBase(IApplicationContext<TUserKey> applicationContext,
        IAsyncQueryableRepository<IQueryableUnitOfWork, TDataEntity, TKey> repository,
        IDateTimeProviderService dateTimeProviderService,
        IMapperFactory mapperFactory,
        ILogger logger,
        Action<ReadOptions>? options = null) : base(applicationContext, repository, mapperFactory, logger,
        options)
    {
        DateTimeProviderService = dateTimeProviderService;
    }
    
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
            itemWithTimeMark.CreatedAt = DateTimeProviderService.GetUtcNow().DateTime;
        }

        if (item is IEntityOwned<TUserKey> itemWithOwner)
        {
            var userId = await ApplicationContext.GetCurrentUserIdAsync();
            itemWithOwner.CreatedById = userId;
        }

        try
        {
            await Repository.AddAsync(item);
            await Repository.UnitOfWork.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "One error was occurred when creating '{EntityName}'", typeof(TEntity).Name);
            throw;
        }

        await OnAfterCreateAsync(request, item, cancellationToken);
        
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
            itemWithTimeTrack.UpdatedAt = DateTimeProviderService.GetUtcNow().DateTime;
        }
        
        if (item is IEntityTrack<TUserKey> itemWithTrack)
        {
            var userId = await ApplicationContext.GetCurrentUserIdAsync();
            itemWithTrack.UpdatedById = userId;
        }

        try
        {
            await Repository.ModifyAsync(item);
            await Repository.UnitOfWork.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "One error was occurred when updating '{EntityName}'", typeof(TEntity).Name);
            throw;
        }

        await OnAfterUpdateAsync(request, item, cancellationToken);
        
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
            itemWithTimeEnded.DeletedAt = DateTimeProviderService.GetUtcNow().DateTime;
        }
        
        if (item is IEntityHistory<TUserKey> itemWithHistory)
        {
            softDelete = true;
            var userId = await ApplicationContext.GetCurrentUserIdAsync();
            itemWithHistory.DeletedById = userId;
        }

        try
        {
            if (softDelete)
            {
                await Repository.ModifyAsync(item);
            }
            else
            {
                await Repository.RemoveAsync(item);
            }

            await Repository.UnitOfWork.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "One error was occurred when deleting '{EntityName}'", typeof(TEntity).Name);
            throw;
        }

        await OnAfterDeleteAsync(item, cancellationToken);
        
        return true;
    }

    protected virtual async Task<TDataEntity?> OnMapRequestAsync(
        CrudAction action, 
        TRequest? request, 
        TDataEntity? dataEntity = null,
        CancellationToken cancellationToken = default)
    {
        var mapper = MapperFactory.GetAnyMapper<TRequest, TDataEntity>();
        return await mapper.MapAsync(request, dataEntity, cancellationToken);
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
    
    protected virtual Task OnAfterCreateAsync(CreateRequest<TRequest> request, TDataEntity? dataEntity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task OnBeforeUpdateAsync(UpdateRequest<TRequest, TKey> request, TDataEntity? dataEntity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task OnAfterUpdateAsync(UpdateRequest<TRequest, TKey> request, TDataEntity? dataEntity, CancellationToken cancellationToken = default)
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