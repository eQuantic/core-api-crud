using eQuantic.Core.Collections;
using eQuantic.Linq.Web;

namespace eQuantic.Core.Api.Crud.Client;

public interface ICrudClient<TEntity, in TRequest, TKey>
{
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<IPagedEnumerable<TEntity>> GetPagedListAsync(
        QueryFilterBuilder<TEntity>? filtering = null,
        QuerySortBuilder<TEntity>? sorting = null,
        int pageIndex = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<TKey?> CreateAsync(TRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(TKey id, TRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}