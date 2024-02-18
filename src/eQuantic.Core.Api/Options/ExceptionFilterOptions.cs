namespace eQuantic.Core.Api.Options;

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

