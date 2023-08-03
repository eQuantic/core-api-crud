namespace eQuantic.Core.Application.Entities.Results;

public class ErrorResult
{
    public string? Message { get; set; }
    public string? Details { get; set; }
    
    public ErrorResult()
    {
    }

    public ErrorResult(string message, string? details = null)
    {
        Message = message;
        Details = details;
    }
}