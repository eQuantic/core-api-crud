namespace eQuantic.Core.Api.Client.Results;

public class ItemResult<T> : BasicResult
{
    public T? Item { get; set; }
}