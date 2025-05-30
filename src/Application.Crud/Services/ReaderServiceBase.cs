using System.Reflection;
using eQuantic.Core.Application.Crud.Enums;
using eQuantic.Core.Application.Crud.Options;
using eQuantic.Core.Collections;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Config;
using eQuantic.Core.DataModel;
using eQuantic.Core.Domain.Entities;
using eQuantic.Core.Domain.Entities.Requests;
using eQuantic.Core.Exceptions;
using eQuantic.Linq.Casting;
using eQuantic.Linq.Filter;
using eQuantic.Linq.Filter.Casting;
using eQuantic.Linq.Filter.Extensions;
using eQuantic.Linq.Sorter;
using eQuantic.Linq.Sorter.Casting;
using eQuantic.Linq.Sorter.Extensions;
using eQuantic.Linq.Specification;
using eQuantic.Mapper;
using Microsoft.Extensions.Logging;

namespace eQuantic.Core.Application.Crud.Services;

public abstract class ReaderServiceBase<TEntity, TDataEntity> : ReaderServiceBase<TEntity, TDataEntity, int, int>
    where TEntity : class, IDomainEntity, new()
    where TDataEntity : class, IEntity<int>, new()
{
    protected ReaderServiceBase(IApplicationContext<int> applicationContext,
        IQueryableUnitOfWork unitOfWork,
        IMapperFactory mapperFactory,
        ILogger logger,
        Action<ReadOptions>? options = null) : base(applicationContext, unitOfWork, mapperFactory, logger, options)
    {
    }
    
    protected ReaderServiceBase(IApplicationContext<int> applicationContext,
        IAsyncQueryableRepository<IQueryableUnitOfWork, TDataEntity, int> repository,
        IMapperFactory mapperFactory,
        ILogger logger,
        Action<ReadOptions>? options = null) : base(applicationContext, repository, mapperFactory, logger, options)
    {
    }
}

public abstract class ReaderServiceBase<TEntity, TDataEntity, TKey, TUserKey> : IReaderService<TEntity, TKey>
    where TEntity : class, IDomainEntity, new()
    where TDataEntity : class, IEntity<TKey>, new()
{
    protected readonly ILogger Logger;
    protected IApplicationContext<TUserKey> ApplicationContext { get; }
    protected IMapperFactory MapperFactory { get; }
    protected IAsyncQueryableRepository<IQueryableUnitOfWork, TDataEntity, TKey> Repository { get; }
    protected ReadOptions ReadOptions { get; }

    protected ReaderServiceBase(
        IApplicationContext<TUserKey> applicationContext,
        IQueryableUnitOfWork unitOfWork,
        IMapperFactory mapperFactory,
        ILogger logger, Action<ReadOptions>? options = null)
    {
        Logger = logger;
        ApplicationContext = applicationContext;
        MapperFactory = mapperFactory;
        Repository = unitOfWork.GetAsyncQueryableRepository<IQueryableUnitOfWork, TDataEntity, TKey>();

        var readOptions = new ReadOptions();
        options?.Invoke(readOptions);

        ReadOptions = readOptions;
    }
    
    protected ReaderServiceBase(
        IApplicationContext<TUserKey> applicationContext,
        IAsyncQueryableRepository<IQueryableUnitOfWork, TDataEntity, TKey> repository,
        IMapperFactory mapperFactory,
        ILogger logger, Action<ReadOptions>? options = null)
    {
        Logger = logger;
        ApplicationContext = applicationContext;
        MapperFactory = mapperFactory;
        Repository = repository;

        var readOptions = new ReadOptions();
        options?.Invoke(readOptions);

        ReadOptions = readOptions;
    }

    public virtual async Task<TEntity?> GetByIdAsync(GetRequest<TKey> request,
        CancellationToken cancellationToken = default)
    {
        var item = await Repository.GetAsync(request.Id, opt => { OnSetQueryableConfiguration(CrudAction.Get, opt); },
            cancellationToken);

        if (item == null)
        {
            var ex = new EntityNotFoundException<TKey>(request.Id);
            Logger.LogError(ex, "{ServiceName} - GetById: Entity of {EntityName} not found", GetType().Name,
                typeof(TEntity).Name);
            throw ex;
        }

        await OnCheckPermissionsAsync(CrudAction.Get, item, cancellationToken);

        ValidateReference(request, item);

        var result = await OnMapEntityAsync(request, item, cancellationToken);

        await OnAfterGetByIdAsync(item, result, cancellationToken);
        return result;
    }

    public virtual async Task<IPagedEnumerable<TEntity>?> GetPagedListAsync(PagedListRequest<TEntity> request,
        CancellationToken cancellationToken = default)
    {
        var requestFiltering = await OnFilterByPermissionsAsync(request.FilterBy);
        request.FilterBy = new FilteringCollection(requestFiltering);

        var sorting = request.OrderBy?
            .Cast<TDataEntity>(opt =>
            {
                OnCastEntity(opt);
                OnCastSorting(opt);
            })
            .ToList() ?? [];

        var specification = await OnGetSpecificationAsync(request);

        await OnBeforeGetPagedListAsync(request, specification, sorting, cancellationToken);

        var count = await Repository.CountAsync(specification, cancellationToken);
        var pageIndex = request.PageIndex ?? 1;
        var pageSize = request.PageSize ?? 10;
        var pagedList = (await Repository.GetPagedAsync(specification, pageIndex, pageSize,
                config =>
                {
                    config.WithSorting(sorting.ToArray());
                    OnSetQueryableConfiguration(CrudAction.GetPaged, config);
                }, cancellationToken))
            .ToList();

        var list = new List<TEntity>();
        foreach (var dataEntity in pagedList)
        {
            var item = await OnMapEntityAsync(request, dataEntity, cancellationToken);
            if (item != null)
                list.Add(item);
        }

        await OnAfterGetPagedListAsync(pagedList, list, cancellationToken);

        return new PagedList<TEntity>(list, count) { PageIndex = pageIndex, PageSize = pageSize };
    }

    protected virtual void OnSetQueryableConfiguration(CrudAction action, QueryableConfiguration<TDataEntity> config)
    {
    }

    protected virtual void OnCastEntity<TCastOptions>(TCastOptions options)
        where TCastOptions : ICastOptions<TCastOptions, TDataEntity>
    {
    }

    protected virtual void OnCastFiltering(FilteringCastOptions<TDataEntity> options)
    {
    }

    protected virtual void OnCastSorting(SortingCastOptions<TDataEntity> options)
    {
    }

    protected virtual async Task<IEnumerable<IFiltering>> OnFilterByPermissionsAsync(IEnumerable<IFiltering>? filtering)
    {
        filtering ??= [];

        if (!IsOwned())
            return filtering;

        var isInRole = await CheckIsInRolesAsync();
        if (!ReadOptions.OnlyOwner)
        {
            if(isInRole == false)
                throw new ForbiddenAccessException();
            
            return filtering;
        }
        
        var userId = await ApplicationContext.GetCurrentUserIdAsync();
        if (userId == null)
            throw new ForbiddenAccessException();

        if(isInRole == true)
            return filtering;
        
        return filtering.Append(new Filtering(nameof(IEntityOwned.CreatedById), userId.ToString()!));
    }

    protected virtual Task<Specification<TDataEntity>> OnGetSpecificationAsync(PagedListRequest<TEntity> request)
    {
        var filtering = request.FilterBy?.ToArray()
            .Cast<TDataEntity>(opt =>
            {
                OnCastEntity(opt);
                OnCastFiltering(opt);
            }).ToList() ?? [];

        SetReferenceFiltering(request, filtering);

        Specification<TDataEntity> specification = filtering.Any()
            ? new EntityFilterSpecification<TDataEntity>(filtering.ToArray())
            : new TrueSpecification<TDataEntity>();

        return Task.FromResult(specification);
    }
    
    protected virtual async Task<TEntity?> OnMapEntityAsync(
        IGetRequest getRequest,
        TDataEntity dataEntity, 
        CancellationToken cancellationToken = default)
    {
        var mapper = MapperFactory.GetAnyMapper<TDataEntity, TEntity>();
        return await mapper.MapAsync(dataEntity, cancellationToken);
    }

    protected virtual Task OnAfterGetByIdAsync(
        TDataEntity? dataEntity,
        TEntity? entity,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    protected virtual async Task OnCheckPermissionsAsync(
        CrudAction action,
        TDataEntity? dataEntity,
        CancellationToken cancellationToken = default)
    {
        if (dataEntity is not IEntityOwned<TUserKey> ownedEntity)
            return;
        
        var isOwner = await CheckOwnerAsync(ownedEntity);
        var isInRole = await CheckIsInRolesAsync();
        if (!isOwner)
        {
            if (isInRole == false)
                throw new ForbiddenAccessException();
        }
    }

    private async Task<bool?> CheckIsInRolesAsync()
    {
        if (!ReadOptions.Roles.Any())
            return null;
        
        var roles = await ApplicationContext.GetCurrentUserRolesAsync();
        return roles.Length != 0 && roles.Any(o => ReadOptions.Roles.Contains(o));
    }
    
    private async Task<bool> CheckOwnerAsync(IEntityOwned<TUserKey> ownedEntity)
    {
        if (!ReadOptions.OnlyOwner) 
            return true;
        
        var userId = await ApplicationContext.GetCurrentUserIdAsync();
        return userId != null && userId.Equals(ownedEntity.CreatedById);
    }

    protected virtual Task OnAfterGetPagedListAsync(
        IEnumerable<TDataEntity> dataEntityList,
        IEnumerable<TEntity> entityList,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnBeforeGetPagedListAsync(
        PagedListRequest<TEntity> request,
        Specification<TDataEntity> specification,
        List<Sorting<TDataEntity>> sorting,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    private void ValidateReference(BasicRequest request, TDataEntity item)
    {
        var referenceType = request.GetReferenceType();
        if (referenceType == null)
            return;

        typeof(ReaderServiceBase<TEntity, TDataEntity, TKey, TUserKey>)
            .GetMethod(nameof(ValidateReference), BindingFlags.NonPublic | BindingFlags.Static)?
            .MakeGenericMethod(referenceType)
            .Invoke(null, [request, item]);
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

    private static IFiltering<TDataEntity>? GetReferenceFiltering<TReferenceKey>(BasicRequest request)
    {
        if (request is not IReferencedRequest<TReferenceKey> referencedRequest)
            return null;

        var dataEntity = new TDataEntity();
        if (dataEntity is not IWithReferenceId<TDataEntity, TReferenceKey> referencedDataEntity)
            return null;

        referencedDataEntity.SetReferenceId(referencedRequest.GetReferenceId()!);
        var filterByRef = referencedDataEntity.GetReferenceFiltering();

        return filterByRef;
    }

    private void SetReferenceFiltering(BasicRequest request, ICollection<IFiltering<TDataEntity>> filtering)
    {
        var referenceType = request.GetReferenceType();
        if (referenceType == null)
            return;

        typeof(ReaderServiceBase<TEntity, TDataEntity, TKey, TUserKey>)
            .GetMethod(nameof(SetReferenceFiltering), BindingFlags.NonPublic | BindingFlags.Static)?
            .MakeGenericMethod(referenceType)
            .Invoke(null, [request, filtering]);
    }

    private static void SetReferenceFiltering<TReferenceKey>(BasicRequest request,
        ICollection<IFiltering<TDataEntity>> filtering)
    {
        var filterByRef = GetReferenceFiltering<TReferenceKey>(request);

        if (filterByRef == null)
            return;

        if (filtering.All(f => f.ColumnName != filterByRef.ColumnName))
        {
            filtering.Add(filterByRef);
        }
    }

    private static bool IsOwned()
    {
        return typeof(TDataEntity)
            .GetInterfaces()
            .Any(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IEntityOwned<>));
    }
}