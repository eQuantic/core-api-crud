using System.Runtime.Serialization;

namespace eQuantic.Core.Exceptions;

[Serializable]
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException()
    {
    }

    public ForbiddenAccessException(string message) 
        : base(message)
    {
    }

    public ForbiddenAccessException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
    
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
    public ForbiddenAccessException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
}