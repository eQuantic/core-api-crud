using Microsoft.AspNetCore.Http;

namespace eQuantic.Core.Api.Options;

public class ExceptionResult
{
    public int HttpStatusCode { get; set; }
    public string? Message { get; set; }

    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>(StringComparer.Ordinal);
    
    public ExceptionResult(
        int httpStatusCode = StatusCodes.Status500InternalServerError, 
        string? message = null, 
        IDictionary<string, string[]>? errors = null)
    {
        HttpStatusCode = httpStatusCode;
        Message = message;

        if (errors != null)
            Errors = errors;
    }
}