using eQuantic.Linq.Filter;
using eQuantic.Linq.Sorter;
using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Domain.Entities.Requests;

/// <summary>
/// The paged list request class
/// </summary>
public class PagedListRequest<TEntity> : BasicRequest
{
    /// <summary>
    /// Gets or sets the value of the page index
    /// </summary>
    [FromQuery]
    public int PageIndex { get; set; } = 1;
    
    /// <summary>
    /// Gets or sets the value of the page size
    /// </summary>
    [FromQuery]
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// Gets or sets the value of the filtering
    /// </summary>
    [FromQuery]
    public IFiltering[] Filtering { get; set; } = Array.Empty<IFiltering>();
    
    /// <summary>
    /// Gets or sets the value of the sorting
    /// </summary>
    [FromQuery]
    public ISorting[] Sorting { get; set; } = Array.Empty<ISorting>();

    public PagedListRequest()
    {
    }

    public PagedListRequest(int? pageIndex, int? pageSize, IFiltering[]? filtering, ISorting[]? sorting)
    {
        if (pageIndex.HasValue) PageIndex = pageIndex.Value;
        if (pageSize.HasValue) PageSize = pageSize.Value;
        if (filtering != null) Filtering = filtering;
        if (sorting != null) Sorting = sorting;
    }
}

/// <summary>
/// The referenced paged list request class
/// </summary>
public class PagedListRequest<TEntity, TReferenceKey> : PagedListRequest<TEntity>, IReferencedRequest<TReferenceKey>
{
    /// <summary>
    /// Gets or sets the value of the reference identifier
    /// </summary>
    [FromRoute]
    public TReferenceKey? ReferenceId { get; set; }

    public PagedListRequest()
    {
        
    }
    public PagedListRequest(TReferenceKey referenceId, int? pageIndex, int? pageSize, IFiltering[]? filtering, ISorting[]? sorting)
        : base(pageIndex, pageSize, filtering, sorting)
    {
        ReferenceId = referenceId;
    }
}