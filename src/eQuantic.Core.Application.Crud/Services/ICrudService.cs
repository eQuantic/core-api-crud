using eQuantic.Core.Application.Crud.Entities.Requests;
using eQuantic.Core.Application.Services;
using eQuantic.Core.Collections;

namespace eQuantic.Core.Application.Crud.Services;

public interface IReaderService : IApplicationService
{
    
}

public interface IReaderService<TEntity> : IReaderService<TEntity, int>
    where TEntity : class, new()
{
    
}

public interface IReaderService<TEntity, TKey> : IReaderService
    where TEntity : class, new()
{
    Task<TEntity?> GetByIdAsync(ItemRequest<TKey> request, CancellationToken cancellationToken = default);
    Task<IPagedEnumerable<TEntity>?> GetPagedListAsync(PagedListRequest<TEntity> request, CancellationToken cancellationToken = default);
}

public interface ICrudService : IReaderService
{
}

public interface ICrudService<TEntity, TRequest> : ICrudService<TEntity, TRequest, int>
    where TEntity : class, new()
{
    
}
public interface ICrudService<TEntity, TRequest, TKey> : ICrudService, IReaderService<TEntity, TKey>
    where TEntity : class, new()
{
    Task<TKey> CreateAsync(CreateRequest<TRequest> request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(UpdateRequest<TRequest, TKey> request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(ItemRequest<TKey> request, CancellationToken cancellationToken = default);
}