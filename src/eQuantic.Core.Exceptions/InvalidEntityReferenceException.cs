using System.Runtime.Serialization;
using eQuantic.Core.Exceptions.Resources;

namespace eQuantic.Core.Exceptions;

[Serializable]
public class InvalidEntityReferenceException : Exception
{
    public InvalidEntityReferenceException()
    {
    }

    public InvalidEntityReferenceException(string message) 
        : base(message)
    {
    }

    public InvalidEntityReferenceException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
    
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
    public InvalidEntityReferenceException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
}

[Serializable]
public class InvalidEntityReferenceException<TReferenceKey> : InvalidEntityReferenceException
{
    public TReferenceKey? Id { get; }

    public InvalidEntityReferenceException()
    {
    }
    
    public InvalidEntityReferenceException(TReferenceKey id) : this(id, ExceptionsResource.InvalidReference)
    {
    }

    public InvalidEntityReferenceException(TReferenceKey id, string message) 
        : base(message)
    {
        Id = id;
    }

    public InvalidEntityReferenceException(TReferenceKey id, string message, Exception innerException) 
        : base(message, innerException)
    {
        Id = id;
    }
    
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
    public InvalidEntityReferenceException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
}