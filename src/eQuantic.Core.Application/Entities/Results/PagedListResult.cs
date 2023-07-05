using eQuantic.Core.Collections;

namespace eQuantic.Core.Application.Entities.Results;

/// <summary>
/// Paged List Result
/// </summary>
/// <typeparam name="T"></typeparam>Re
public class PagedListResult<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PagedListResult{T}"/> class.
    /// </summary>
    public PagedListResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedListResult{T}"/> class.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="pageIndex">Index of the page.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="total">The total.</param>
    public PagedListResult(IEnumerable<T> items, int pageIndex, int pageSize, long total)
    {
        Items = items.ToList();
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = total;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedListResult{T}"/> class.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="total">The total.</param>
    public PagedListResult(IEnumerable<T> items, long total) : this(items, 1, (int)total, total)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedListResult{T}"/> class.
    /// </summary>
    /// <param name="items">The items.</param>
    public PagedListResult(IPagedEnumerable<T>? items)
    {
        if (items == null)
        {
            return;
        }

        PageIndex = items.PageIndex;
        PageSize = items.PageSize;
        TotalCount = items.TotalCount;
        Items = items.ToList();
    }

    /// <summary>
    /// Gets or sets the items.
    /// </summary>
    /// <value>
    /// The items.
    /// </value>
    public List<T>? Items { get; set; }

    /// <summary>
    /// Gets or sets the index of the page.
    /// </summary>
    /// <value>
    /// The index of the page.
    /// </value>
    public int PageIndex { get; set; }

    /// <summary>
    /// Gets or sets the size of the page.
    /// </summary>
    /// <value>
    /// The size of the page.
    /// </value>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total count.
    /// </summary>
    /// <value>
    /// The total count.
    /// </value>
    public long TotalCount { get; set; }
}