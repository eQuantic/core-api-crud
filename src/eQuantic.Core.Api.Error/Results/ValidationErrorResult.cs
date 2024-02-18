namespace eQuantic.Core.Api.Error.Results;

public class ValidationErrorResult : ErrorResult
{
    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>(StringComparer.Ordinal);

    public ValidationErrorResult()
    {
    }

    public ValidationErrorResult(
        string? message, 
        string? details = null,
        IDictionary<string, string[]>? errors = null) : base(message, details)
    {
        if (errors != null)
            Errors = errors;
    }
}