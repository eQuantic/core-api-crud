namespace eQuantic.Core.Api.Error.Results;

public class ValidationErrorResult : ErrorResult
{
    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>(StringComparer.Ordinal);
}