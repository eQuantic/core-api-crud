namespace eQuantic.Core.Api.Client.Results;

/// <summary>
/// Paged List Result
/// </summary>
/// <typeparam name="T"></typeparam>Re
public class PagedListResult<T> : BasicResult
{
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