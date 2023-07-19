using eQuantic.Core.Application.Crud.Entities.Requests;
using eQuantic.Core.Application.Services;
using eQuantic.Core.Collections;

namespace eQuantic.Core.Application.Crud.Services;

public interface ICrudServiceBase : IApplicationService
{
}

public interface ICrudServiceBase<TEntity, TRequest> : ICrudServiceBase
    where TEntity : class, new()
{
    Task<TEntity?> GetByIdAsync(ItemRequest request, CancellationToken cancellationToken = default);
    Task<IPagedEnumerable<TEntity>?> GetPagedListAsync(PagedListRequest<TEntity> request, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateRequest<TRequest> request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(UpdateRequest<TRequest> request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(ItemRequest request, CancellationToken cancellationToken = default);
}