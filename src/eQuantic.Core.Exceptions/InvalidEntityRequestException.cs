using System.Runtime.Serialization;

namespace eQuantic.Core.Exceptions;

[Serializable]
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
    
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
    public InvalidEntityRequestException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
}