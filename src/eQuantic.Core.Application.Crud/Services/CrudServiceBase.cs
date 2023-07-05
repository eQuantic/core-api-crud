using eQuantic.Core.Application.Crud.Entities.Requests;
using eQuantic.Core.Application.Entities.Data;
using eQuantic.Core.Application.Services;
using eQuantic.Core.Collections;
using eQuantic.Core.Data.Repository;
using eQuantic.Linq.Filter;
using eQuantic.Linq.Filter.Extensions;
using eQuantic.Linq.Sorter.Extensions;
using eQuantic.Linq.Specification;
using eQuantic.Mapper;

namespace eQuantic.Core.Application.Crud.Services;

public interface ICrudServiceBase : IApplicationService {}
public interface ICrudServiceBase<TEntity, TRequest> : ICrudServiceBase 
    where TEntity : class, new()
{
    Task<TEntity?> GetByIdAsync(ItemRequest request);
    Task<IPagedEnumerable<TEntity>?> GetPagedListAsync(PagedListRequest<TEntity> request);
    Task<int> CreateAsync(CreateRequest<TRequest> request);
    Task<bool> UpdateAsync(UpdateRequest<TRequest> request);
    Task<bool> DeleteAsync(ItemRequest request);
}

public abstract class CrudServiceBase<TEntity, TRequest, TDataEntity, TUser> : ICrudServiceBase<TEntity, TRequest> where TEntity : class, new()
    where TDataEntity : EntityDataBase, new()
{
    private readonly IMapperFactory _mapperFactory;
    private readonly IAsyncQueryableRepository<IQueryableUnitOfWork, TDataEntity, int> _repository;

    protected CrudServiceBase(IQueryableUnitOfWork unitOfWork, IMapperFactory mapperFactory)
    {
        _mapperFactory = mapperFactory;
        _repository = unitOfWork.GetAsyncQueryableRepository<IQueryableUnitOfWork, TDataEntity, int>();
    }

    public async Task<TEntity?> GetByIdAsync(ItemRequest request)
    {
        var item = await _repository.GetAsync(request.Id);

        if (item == null)
        {
            return null;
        }
        
        var mapper = _mapperFactory.GetMapper<TDataEntity, TEntity>();
        if (mapper == null)
        {
            return null;
        }

        var result = mapper.Map(item);

        await OnBeforeGetByIdAsync(item, result);
        return result;
    }

    public async Task<IPagedEnumerable<TEntity>?> GetPagedListAsync(PagedListRequest<TEntity> request)
    {
        var filtering = request.Filtering
            .Cast<TDataEntity>(opt => opt.ExcludeUnmapped()).ToList();
        
        SetReferenceFiltering(request, filtering);
        
        var sorting = request.Sorting
            .Cast<TDataEntity>(opt => opt.ExcludeUnmapped());
        var specification = new EntityFilterSpecification<TDataEntity>(filtering.ToArray());

        var count = await _repository.CountAsync(specification);
        var pagedList = (await _repository.GetPagedAsync(specification, request.PageIndex, request.PageSize, sorting))
            .ToList();
        var mapper = _mapperFactory.GetMapper<TDataEntity, TEntity>();

        if (mapper == null)
        {
            return null;
        }

        var list = pagedList.Select(o => mapper.Map(o)!).ToList();

        await OnBeforeGetPagedListAsync(pagedList, list);

        return new PagedList<TEntity>(list, count) { PageIndex = request.PageIndex, PageSize = request.PageSize };
    }

    public async Task<int> CreateAsync(CreateRequest<TRequest> request)
    {
        var mapper = _mapperFactory.GetMapper<TRequest, TDataEntity>();
        var item = mapper?.Map(request.Body);
        if (item == null)
        {
            return default;
        }

        if (request is IReferencedRequest<int> referencedRequest && item is IWithReferenceId<TDataEntity, int> referencedItem)
        {
            referencedItem.SetReferenceId(referencedRequest.ReferenceId);
        }
        
        if (item is IEntityOwned<TUser> itemWithOwner)
        {
            itemWithOwner.CreatedAt = DateTime.UtcNow;
        }
        
        await _repository.AddAsync(item);
        await _repository.UnitOfWork.CommitAsync();

        return item.Id;
    }
    
    public async Task<bool> UpdateAsync(UpdateRequest<TRequest> request)
    {
        var item = await GetItem(request);
        if (item == null)
        {
            return false;
        }
        var mapper = _mapperFactory.GetMapper<TRequest, TDataEntity>();
        mapper?.Map(request.Body, item);

        if (item is IEntityTrack<TUser> itemWithTrack)
        {
            itemWithTrack.UpdatedAt = DateTime.UtcNow;
        }
        
        await _repository.ModifyAsync(item);
        await _repository.UnitOfWork.CommitAsync();

        return true;
    }
    
    public async Task<bool> DeleteAsync(ItemRequest request)
    {
        var item = await GetItem(request);
        if (item == null)
            return false;

        if (item is IEntityHistory<TUser> itemWithHistory)
        {
            itemWithHistory.DeletedAt = DateTime.UtcNow;
            await _repository.ModifyAsync(item);
        }
        else
        {
            await _repository.RemoveAsync(item);
        }

        await _repository.UnitOfWork.CommitAsync();

        return true;
    }

    protected virtual Task OnBeforeGetByIdAsync(TDataEntity? dataEntity, TEntity? entity)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task OnBeforeGetPagedListAsync(IEnumerable<TDataEntity> dataEntityList, IEnumerable<TEntity> entityList)
    {
        return Task.CompletedTask;
    }

    private async Task<TDataEntity?> GetItem(ItemRequest request)
    {
        Specification<TDataEntity> specification = new DirectSpecification<TDataEntity>(o => o.Id == request.Id);
        var filterByRef = GetReferenceFiltering(request);
        if (filterByRef != null)
        {
            specification = specification && new EntityFilterSpecification<TDataEntity>(new IFiltering[]{ filterByRef });
        }

        return await _repository.GetFirstAsync(specification);
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
    
    private static void SetReferenceFiltering<TAnyRequest>(TAnyRequest request, ICollection<IFiltering<TDataEntity>> filtering)
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