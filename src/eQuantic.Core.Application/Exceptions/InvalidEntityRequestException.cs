using System.Runtime.Serialization;

namespace eQuantic.Core.Application.Exceptions;

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
    
    protected InvalidEntityRequestException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
    }
}