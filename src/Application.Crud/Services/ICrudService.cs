using eQuantic.Core.Application.Services;
using eQuantic.Core.Collections;
using eQuantic.Core.Domain.Entities;
using eQuantic.Core.Domain.Entities.Requests;

namespace eQuantic.Core.Application.Crud.Services;

public interface IReaderService : IApplicationService
{
    
}

public interface IReaderService<TEntity> : IReaderService<TEntity, int>
    where TEntity : class, IDomainEntity, new()
{
    
}

public interface IReaderService<TEntity, TKey> : IReaderService
    where TEntity : class, IDomainEntity, new()
{
    Task<TEntity?> GetByIdAsync(GetRequest<TKey> request, CancellationToken cancellationToken = default);
    Task<IPagedEnumerable<TEntity>?> GetPagedListAsync(PagedListRequest<TEntity> request, CancellationToken cancellationToken = default);
}

public interface ICrudService : IReaderService
{
}

public interface ICrudService<TEntity, TRequest> : ICrudService<TEntity, TRequest, int>
    where TEntity : class, IDomainEntity, new()
{
    
}
public interface ICrudService<TEntity, TRequest, TKey> : ICrudService, IReaderService<TEntity, TKey>
    where TEntity : class, IDomainEntity, new()
{
    Task<TKey> CreateAsync(CreateRequest<TRequest> request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(UpdateRequest<TRequest, TKey> request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(ItemRequest<TKey> request, CancellationToken cancellationToken = default);
}