using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using eQuantic.Core.Api.Client.Results;
using eQuantic.Core.Collections;
using eQuantic.Linq.Filter;
using eQuantic.Linq.Sorter;

namespace eQuantic.Core.Api.Crud.Client;

/// <summary>
/// CRUD Client Base
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract class CrudClientBase<TEntity, TRequest, TKey> : ICrudClient<TEntity, TRequest, TKey>
{
    private readonly string _route;
    
    /// <summary>
    /// HTTP Client
    /// </summary>
    protected HttpClient HttpClient { get; }

    /// <summary>
    /// CRUD Client constructor
    /// </summary>
    /// <param name="route">The route</param>
    /// <param name="httpClient">The HTTP Client</param>
    protected CrudClientBase(string route, HttpClient httpClient)
    {
        _route = route;
        HttpClient = httpClient;
    }

    /// <summary>
    /// Get entity by identifier
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetAsync($"{_route}/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
        var item = await response.Content.ReadFromJsonAsync<TEntity?>((JsonSerializerOptions?)null, cancellationToken);
        return item;
    }

    /// <summary>
    /// Get paged entity by criteria
    /// </summary>
    /// <param name="filtering">The filter</param>
    /// <param name="sorting">The sorter</param>
    /// <param name="pageIndex">The page index</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public async Task<IPagedEnumerable<TEntity>> GetPagedListAsync(
        IFiltering<TEntity>[] filtering,
        ISorting<TEntity>[] sorting,
        int pageIndex = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var httpValueCollection = HttpUtility.ParseQueryString(string.Empty);
        foreach (var filter in filtering)
        {
            httpValueCollection.Add("filterBy", filter.ToString());
        }
        foreach (var sorter in sorting)
        {
            httpValueCollection.Add("orderBy", sorter.ToString());
        }
        httpValueCollection.Add(nameof(pageIndex), pageIndex.ToString());
        httpValueCollection.Add(nameof(pageSize), pageSize.ToString());
        
        var response = await HttpClient.GetAsync($"{_route}?{httpValueCollection}", cancellationToken);
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PagedListResult<TEntity>>((JsonSerializerOptions?)null, cancellationToken);

        if (list == null)
            return new PagedList<TEntity>(Array.Empty<TEntity>(), 0);
        
        return new PagedList<TEntity>(list.Items, list.TotalCount)
        {
            PageIndex = list.PageIndex,
            PageSize = list.PageSize
        };
    }

    /// <summary>
    /// Create an entity
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public async Task<TKey?> CreateAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostAsync(_route, JsonContent.Create(request), cancellationToken);
        if (response.StatusCode != HttpStatusCode.Created)
            return default;

        var id = await response.Content.ReadFromJsonAsync<TKey>((JsonSerializerOptions?)null, cancellationToken);
        return id;
    }

    /// <summary>
    /// Update an entity by identifier
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(TKey id, TRequest request, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PutAsync($"{_route}/{id}", JsonContent.Create(request), cancellationToken);
        response.EnsureSuccessStatusCode();
        return true;
    }

    /// <summary>
    /// Delete an entity by identifier
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.DeleteAsync($"{_route}/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return true;
    }
}