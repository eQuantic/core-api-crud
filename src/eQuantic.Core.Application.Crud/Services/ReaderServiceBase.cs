using eQuantic.Core.Application.Crud.Entities.Requests;
using eQuantic.Core.Application.Entities.Data;
using eQuantic.Core.Application.Exceptions;
using eQuantic.Core.Collections;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Sql;
using eQuantic.Linq.Casting;
using eQuantic.Linq.Filter;
using eQuantic.Linq.Filter.Casting;
using eQuantic.Linq.Filter.Extensions;
using eQuantic.Linq.Sorter.Casting;
using eQuantic.Linq.Sorter.Extensions;
using eQuantic.Linq.Specification;
using eQuantic.Mapper;
using Microsoft.Extensions.Logging;

namespace eQuantic.Core.Application.Crud.Services;

public abstract class ReaderServiceBase<TEntity, TDataEntity> : ReaderServiceBase<TEntity, TDataEntity, int>
    where TEntity : class, new()
    where TDataEntity : class, IEntity<int>, new()
{
    protected ReaderServiceBase(
        IDefaultUnitOfWork unitOfWork, 
        IMapperFactory mapperFactory, 
        ILogger logger) : base(unitOfWork, mapperFactory, logger)
    {
    }
}

public abstract class ReaderServiceBase<TEntity, TDataEntity, TKey> : IReaderService<TEntity, TKey>
    where TEntity : class, new()
    where TDataEntity : class, IEntity<TKey>, new()
{
    protected readonly ILogger Logger;
    protected IDefaultUnitOfWork UnitOfWork { get; }
    protected IMapperFactory MapperFactory { get; }
    protected IAsyncQueryableRepository<ISqlUnitOfWork, TDataEntity, TKey> Repository { get; }

    protected ReaderServiceBase(
        IDefaultUnitOfWork unitOfWork, 
        IMapperFactory mapperFactory, 
        ILogger logger)
    {
        Logger = logger;
        UnitOfWork = unitOfWork;
        MapperFactory = mapperFactory;
        Repository = unitOfWork.GetAsyncQueryableRepository<TDataEntity, TKey>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(ItemRequest<TKey> request, CancellationToken cancellationToken = default)
    {
        var item = await Repository.GetAsync(request.Id, opt =>
        {
            opt.WithProperties(OnGetProperties());
        }, cancellationToken);

        if (item == null)
        {
            var ex = new EntityNotFoundException<TKey>(request.Id);
            Logger.LogError(ex, "{ServiceName} - GetById: Entity of {EntityName} not found", GetType().Name, typeof(TEntity).Name);
            throw ex;
        }
        
        if (request is IReferencedRequest<int> referencedRequest && item is IWithReferenceId<TDataEntity, int> referencedItem)
        {
            if (referencedItem.GetReferenceId() != referencedRequest.ReferenceId)
            {
                throw new InvalidEntityReferenceException<int>(referencedRequest.ReferenceId);
            }
        }
        
        var result = OnMapEntity(item);

        await OnBeforeGetByIdAsync(item, result, cancellationToken);
        return result;
    }

    public virtual async Task<IPagedEnumerable<TEntity>?> GetPagedListAsync(PagedListRequest<TEntity> request,
        CancellationToken cancellationToken = default)
    {
        var sorting = request.Sorting
            .Cast<TDataEntity>(opt =>
            {
                OnCastEntity(opt);
                OnCastSorting(opt);
            });

        var specification = OnGetSpecification(request);

        await OnAfterGetPagedListAsync(request, specification, cancellationToken);
        
        var count = await Repository.CountAsync(specification, cancellationToken);
        var pagedList = (await Repository.GetPagedAsync(specification, request.PageIndex, request.PageSize,
                config =>
                {
                    config
                        .WithSorting(sorting)
                        .WithProperties(OnGetProperties());
                }, cancellationToken))
            .ToList();
        
        var list = pagedList
            .Select(dataEntity => OnMapEntity(dataEntity)!)
            .Where(item => item != null)
            .ToList();

        await OnBeforeGetPagedListAsync(pagedList, list, cancellationToken);

        return new PagedList<TEntity>(list, count) { PageIndex = request.PageIndex, PageSize = request.PageSize };
    }
    
    protected virtual string[] OnGetProperties()
    {
        return Array.Empty<string>();
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

    protected virtual Specification<TDataEntity> OnGetSpecification(PagedListRequest<TEntity> request)
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

        return specification;
    }
    
    protected virtual TEntity? OnMapEntity(TDataEntity dataEntity)
    {
        var mapper = MapperFactory.GetMapper<TDataEntity, TEntity>();
        if (mapper != null) 
            return mapper.Map(dataEntity);

        var mapperType = typeof(IMapper<TDataEntity, TEntity>);
        var ex = new DependencyNotFoundException(mapperType);
        Logger.LogError(ex, "{ServiceName}: Mapper of {MapperName} not found", GetType().Name, mapperType.Name);
        throw ex;
    }

    protected virtual Task OnBeforeGetByIdAsync(
        TDataEntity? dataEntity, 
        TEntity? entity, 
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task OnBeforeGetPagedListAsync(
        IEnumerable<TDataEntity> dataEntityList,
        IEnumerable<TEntity> entityList, 
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnAfterGetPagedListAsync(
        PagedListRequest<TEntity> request, 
        Specification<TDataEntity> specification, 
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
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

    private static void SetReferenceFiltering<TAnyRequest>(TAnyRequest request,
        ICollection<IFiltering<TDataEntity>> filtering)
    {
        var filterByRef = GetReferenceFiltering(request);

        if (filterByRef == null)
            return;

        if (filtering.All(f => f.ColumnName != filterByRef.ColumnName))
        {
            filtering.Add(filterByRef);
        }
    }
}