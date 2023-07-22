using eQuantic.Core.Application.Crud.Entities.Requests;
using eQuantic.Core.Application.Entities.Data;
using eQuantic.Core.Application.Exceptions;
using eQuantic.Core.Collections;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Sql;
using eQuantic.Linq.Filter;
using eQuantic.Linq.Filter.Extensions;
using eQuantic.Linq.Sorter.Extensions;
using eQuantic.Linq.Specification;
using eQuantic.Mapper;

namespace eQuantic.Core.Application.Crud.Services;

public abstract class CrudServiceBase<TEntity, TRequest, TDataEntity, TUser> : ICrudServiceBase<TEntity, TRequest>
    where TEntity : class, new()
    where TDataEntity : EntityDataBase, new()
{
    protected IDefaultUnitOfWork UnitOfWork { get; }
    protected IMapperFactory MapperFactory { get; }
    protected IAsyncQueryableRepository<ISqlUnitOfWork, TDataEntity, int> Repository { get; }

    protected CrudServiceBase(IDefaultUnitOfWork unitOfWork, IMapperFactory mapperFactory)
    {
        UnitOfWork = unitOfWork;
        MapperFactory = mapperFactory;
        Repository = unitOfWork.GetAsyncQueryableRepository<TDataEntity, int>();
    }

    public async Task<TEntity?> GetByIdAsync(ItemRequest request, CancellationToken cancellationToken = default)
    {
        var item = await Repository.GetAsync(request.Id, opt =>
        {
            opt.WithProperties(OnGetProperties());
        }, cancellationToken);

        if (item == null)
        {
            throw new EntityNotFoundException<int>(request.Id);
        }
        
        if (request is IReferencedRequest<int> referencedRequest && item is IWithReferenceId<TDataEntity, int> referencedItem)
        {
            if (referencedItem.GetReferenceId() != referencedRequest.ReferenceId)
            {
                throw new InvalidReferenceException<int>(referencedRequest.ReferenceId);
            }
        }
        
        var result = OnMapEntity(item);

        await OnBeforeGetByIdAsync(item, result, cancellationToken);
        return result;
    }

    public async Task<IPagedEnumerable<TEntity>?> GetPagedListAsync(PagedListRequest<TEntity> request,
        CancellationToken cancellationToken = default)
    {
        var filtering = request.Filtering
            .Cast<TDataEntity>(opt => opt.ExcludeUnmapped()).ToList();

        SetReferenceFiltering(request, filtering);

        var sorting = request.Sorting
            .Cast<TDataEntity>(opt => opt.ExcludeUnmapped());

        Specification<TDataEntity> specification = filtering.Any()
            ? new EntityFilterSpecification<TDataEntity>(filtering.ToArray())
            : new TrueSpecification<TDataEntity>();

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
    
    public async Task<int> CreateAsync(CreateRequest<TRequest> request, CancellationToken cancellationToken = default)
    {
        var item = OnMapRequest(request.Body);
        if (item == null)
        {
            return default;
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
        await Repository.UnitOfWork.CommitAsync();
        await OnBeforeCreateAsync(item, cancellationToken);
        
        return item.Id;
    }

    public async Task<bool> UpdateAsync(UpdateRequest<TRequest> request, CancellationToken cancellationToken = default)
    {
        var item = await GetItem(request, cancellationToken);
        if (item == null)
        {
            throw new EntityNotFoundException<int>(request.Id);
        }

        await OnAfterUpdateAsync(item, cancellationToken);
        
        OnMapRequest(request.Body, item);

        if (item is IEntityTrack<TUser> itemWithTrack)
        {
            itemWithTrack.UpdatedAt = DateTime.UtcNow;
        }

        await Repository.ModifyAsync(item);
        await Repository.UnitOfWork.CommitAsync();
        await OnBeforeUpdateAsync(item, cancellationToken);
        
        return true;
    }

    public async Task<bool> DeleteAsync(ItemRequest request, CancellationToken cancellationToken = default)
    {
        var item = await GetItem(request, cancellationToken);
        if (item == null)
            throw new EntityNotFoundException<int>(request.Id);

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

        await Repository.UnitOfWork.CommitAsync();
        await OnBeforeDeleteAsync(item, cancellationToken);
        
        return true;
    }

    protected virtual string[] OnGetProperties()
    {
        return Array.Empty<string>();
    }
    
    protected virtual TEntity? OnMapEntity(TDataEntity dataEntity)
    {
        var mapper = MapperFactory.GetMapper<TDataEntity, TEntity>();
        return mapper?.Map(dataEntity);
    }

    protected virtual TDataEntity? OnMapRequest(TRequest? request, TDataEntity? dataEntity = null)
    {
        var mapper = MapperFactory.GetMapper<TRequest, TDataEntity>();
        return mapper?.Map(request, dataEntity);
    }
    
    protected virtual Task OnBeforeGetByIdAsync(TDataEntity? dataEntity, TEntity? entity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnBeforeGetPagedListAsync(IEnumerable<TDataEntity> dataEntityList,
        IEnumerable<TEntity> entityList, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
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

    private async Task<TDataEntity?> GetItem(ItemRequest request, CancellationToken cancellationToken = default)
    {
        Specification<TDataEntity> specification = new DirectSpecification<TDataEntity>(o => o.Id == request.Id);
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