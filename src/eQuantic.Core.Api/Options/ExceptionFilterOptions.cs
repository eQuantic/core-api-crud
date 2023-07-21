using Microsoft.AspNetCore.Http;

namespace eQuantic.Core.Api.Options;

public class ExceptionFilterOptions
{
    private readonly Dictionary<Type, ExceptionResultOptions> _options = new();
    public ExceptionResultOptions For<TException>() where TException : Exception
    {
        var resultOptions = new ExceptionResultOptions(this);
        var type = typeof(TException);
        _options[type] = resultOptions;
        return resultOptions;
    }

    public Dictionary<Type, ExceptionResultOptions> GetOptions() => _options;
}

public class ExceptionResultOptions
{
    private readonly ExceptionFilterOptions _options;

    public int HttpStatusCode { get; private set; } = StatusCodes.Status500InternalServerError;
    public string? Message { get; private set; }
    
    public ExceptionResultOptions(ExceptionFilterOptions options)
    {
        _options = options;
    }

    public ExceptionFilterOptions Use(int httpStatusCode, string message)
    {
        HttpStatusCode = httpStatusCode;
        Message = message;

        return _options;
    }
    
    public ExceptionFilterOptions UseDefault()
    {
        return _options;
    }
}