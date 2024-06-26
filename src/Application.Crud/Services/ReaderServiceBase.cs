using System.Reflection;
using eQuantic.Core.Application.Crud.Enums;
using eQuantic.Core.Application.Crud.Options;
using eQuantic.Core.Application.Entities.Data;
using eQuantic.Core.Collections;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Config;
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

public abstract class ReaderServiceBase<TEntity, TDataEntity>(
    IApplicationContext<int> applicationContext,
    IQueryableUnitOfWork unitOfWork,
    IMapperFactory mapperFactory,
    ILogger logger,
    Action<ReadOptions>? options = null)
    : ReaderServiceBase<TEntity, TDataEntity, int, int>(applicationContext, unitOfWork, mapperFactory, logger, options)
    where TEntity : class, new()
    where TDataEntity : class, IEntity<int>, new();

public abstract class ReaderServiceBase<TEntity, TDataEntity, TKey, TUserKey> : IReaderService<TEntity, TKey>
    where TEntity : class, new()
    where TDataEntity : class, IEntity<TKey>, new()
{
    protected readonly ILogger Logger;
    protected IApplicationContext<TUserKey> ApplicationContext { get; }
    protected IQueryableUnitOfWork UnitOfWork { get; }
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
        UnitOfWork = unitOfWork;
        MapperFactory = mapperFactory;
        Repository = unitOfWork.GetAsyncQueryableRepository<IQueryableUnitOfWork, TDataEntity, TKey>();

        var readOptions = new ReadOptions();
        options?.Invoke(readOptions);

        ReadOptions = readOptions;
    }

    public virtual async Task<TEntity?> GetByIdAsync(ItemRequest<TKey> request,
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

        var result = await OnMapEntityAsync(item, MappingPriority.SyncOrAsync, cancellationToken);

        await OnAfterGetByIdAsync(item, result, cancellationToken);
        return result;
    }

    public virtual async Task<IPagedEnumerable<TEntity>?> GetPagedListAsync(PagedListRequest<TEntity> request,
        CancellationToken cancellationToken = default)
    {
        var requestFiltering = await OnFilterByPermissionsAsync(request.Filtering);
        request.Filtering = requestFiltering.ToArray();

        var sorting = request.Sorting
            .Cast<TDataEntity>(opt =>
            {
                OnCastEntity(opt);
                OnCastSorting(opt);
            })
            .ToList();

        var specification = await OnGetSpecificationAsync(request);

        await OnBeforeGetPagedListAsync(request, specification, sorting, cancellationToken);

        var count = await Repository.CountAsync(specification, cancellationToken);
        var pagedList = (await Repository.GetPagedAsync(specification, request.PageIndex, request.PageSize,
                config =>
                {
                    config.WithSorting(sorting.ToArray());
                    OnSetQueryableConfiguration(CrudAction.GetPaged, config);
                }, cancellationToken))
            .ToList();

        var list = new List<TEntity>();
        foreach (var dataEntity in pagedList)
        {
            var item = await OnMapEntityAsync(dataEntity, MappingPriority.SyncOrAsync, cancellationToken);
            if (item != null)
                list.Add(item);
        }

        await OnAfterGetPagedListAsync(pagedList, list, cancellationToken);

        return new PagedList<TEntity>(list, count) { PageIndex = request.PageIndex, PageSize = request.PageSize };
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

        if (!ReadOptions.OnlyOwner || !IsOwned())
            return filtering;

        var userId = await ApplicationContext.GetCurrentUserIdAsync();
        if (userId == null)
            throw new ForbiddenAccessException();

        return filtering.Append(new Filtering(nameof(IEntityOwned.CreatedById), userId.ToString()!));
    }

    protected virtual Task<Specification<TDataEntity>> OnGetSpecificationAsync(PagedListRequest<TEntity> request)
    {
        var filtering = request.Filtering
            .Cast<TDataEntity>(opt =>
            {
                OnCastEntity(opt);
                OnCastFiltering(opt);
            }).ToList();

        SetReferenceFiltering(request, filtering);

        Specification<TDataEntity> specification = filtering.Any()
            ? new EntityFilterSpecification<TDataEntity>(filtering.ToArray())
            : new TrueSpecification<TDataEntity>();

        return Task.FromResult(specification);
    }

    protected virtual Task<TEntity?> OnMapEntityAsync(
        TDataEntity dataEntity,
        MappingPriority mappingPriority = MappingPriority.SyncOrAsync,
        CancellationToken cancellationToken = default)
    {
        return Map<TDataEntity, TEntity>(dataEntity, null, mappingPriority, cancellationToken);
    }

    internal async Task<TDestination?> Map<TSource, TDestination>(
        TSource? source, TDestination? destination = default,
        MappingPriority mappingPriority = MappingPriority.SyncOrAsync,
        CancellationToken cancellationToken = default)
    {
        return mappingPriority switch
        {
            MappingPriority.SyncOnly => MapSync(source, destination, true),
            MappingPriority.SyncOrAsync => MapSync(source, destination) ?? await MapAsync(source, destination, true, cancellationToken),
            MappingPriority.SyncAndAsync => await MapAsync(source, MapSync(source, destination), true, cancellationToken),
            MappingPriority.AsyncOnly => await MapAsync(source, destination, true, cancellationToken),
            MappingPriority.AsyncOrSync => await MapAsync(source, destination, cancellationToken: cancellationToken) ?? MapSync(source, destination, true),
            MappingPriority.AsyncAndSync => MapSync(source, await MapAsync(source, destination, cancellationToken: cancellationToken), true),
            _ => throw new ArgumentOutOfRangeException(nameof(mappingPriority), mappingPriority, null)
        };
    }
    private TDestination? MapSync<TSource, TDestination>(
        TSource? source, 
        TDestination? destination = default, 
        bool throwIfNotExists = false)
    {
        var mapper = MapperFactory.GetMapper<TSource, TDestination>();
        if(mapper != null)
            return mapper.Map(source, destination);
        
        if (!throwIfNotExists)
            return default;
        
        ThrowMapperException<IMapper<TSource, TDestination>>();
        return default;
    }

    private async Task<TDestination?> MapAsync<TSource, TDestination>(
        TSource? source, 
        TDestination? destination = default, 
        bool throwIfNotExists = false,
        CancellationToken cancellationToken = default)
    {
        var asyncMapper = MapperFactory.GetAsyncMapper<TSource, TDestination>();
        if (asyncMapper != null)
            return await asyncMapper.MapAsync(source, destination);

        if (!throwIfNotExists)
            return default;

        ThrowMapperException<IAsyncMapper<TSource, TDestination>>();
        return default;
    }

    private void ThrowMapperException<TMapper>() where TMapper : IMapper
    {
        var mapperType = typeof(TMapper);
        var ex = new DependencyNotFoundException(mapperType);
        Logger.LogError(ex, "{ServiceName}: Mapper of {MapperName} not found", GetType().Name, mapperType.Name);
        throw ex;
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
        if (ReadOptions.OnlyOwner && dataEntity is IEntityOwned<TUserKey> ownedEntity)
        {
            var userId = await ApplicationContext.GetCurrentUserIdAsync();
            if (userId == null || !userId.Equals(ownedEntity.CreatedById))
            {
                throw new ForbiddenAccessException();
            }
        }
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

        if (referencedItem.GetReferenceId()?.Equals(referencedRequest.ReferenceId) == false)
        {
            throw new InvalidEntityReferenceException<TReferenceKey>(referencedRequest.ReferenceId!);
        }
    }

    private static IFiltering<TDataEntity>? GetReferenceFiltering<TReferenceKey>(BasicRequest request)
    {
        if (request is not IReferencedRequest<TReferenceKey> referencedRequest)
            return null;

        var dataEntity = new TDataEntity();
        if (dataEntity is not IWithReferenceId<TDataEntity, TReferenceKey> referencedDataEntity)
            return null;

        referencedDataEntity.SetReferenceId(referencedRequest.ReferenceId!);
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