using Microsoft.AspNetCore.Http;

namespace eQuantic.Core.Api.Options;

public class ExceptionResult
{
    public int HttpStatusCode { get; set; }
    public string? Message { get; set; }

    public ExceptionResult(int httpStatusCode = StatusCodes.Status500InternalServerError, string? message = null)
    {
        HttpStatusCode = httpStatusCode;
        Message = message;
    }
}
public class ExceptionFilterOptions
{
    private readonly Dictionary<Type, ExceptionResultOptions> _options = new();
    public ExceptionResultOptions<TException> For<TException>() where TException : Exception
    {
        var resultOptions = new ExceptionResultOptions<TException>(this);
        var type = typeof(TException);
        _options[type] = resultOptions;
        return resultOptions;
    }

    public Dictionary<Type, ExceptionResultOptions> GetOptions() => _options;
}

public class ExceptionResultOptions
{
    protected readonly ExceptionFilterOptions Options;
    protected ExceptionResult Result = new();
    protected Func<Exception, ExceptionResult>? Generator;
    public ExceptionResultOptions(ExceptionFilterOptions options)
    {
        Options = options;
    }

    public ExceptionFilterOptions Use(int httpStatusCode, string message)
    {
        Result = new ExceptionResult(httpStatusCode, message);
        return Options;
    }
    
    public ExceptionFilterOptions UseDefault()
    {
        return Options;
    }

    internal ExceptionResult GetResult() => Result;

    internal void SetResult(Exception exception)
    {
        var result = Generator?.Invoke(exception);
        if (result != null)
            Result = result;
    }
}
public class ExceptionResultOptions<TException> : ExceptionResultOptions where TException : Exception
{
    public ExceptionResultOptions(ExceptionFilterOptions options) : base(options)
    {
    }
    
    public ExceptionFilterOptions Use(Func<TException, ExceptionResult> opt)
    {
        Generator = (ex) => opt.Invoke((TException)ex);
        return Options;
    }

    
}