namespace eQuantic.Core.Exceptions;

public class InvalidEntityRequestException : Exception
{
    public InvalidEntityRequestException()
    {
    }

    public InvalidEntityRequestException(string message) 
        : base(message)
    {
    }

    public InvalidEntityRequestException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}