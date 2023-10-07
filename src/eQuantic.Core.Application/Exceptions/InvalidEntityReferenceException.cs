using System.Runtime.Serialization;
using eQuantic.Core.Application.Resources;

namespace eQuantic.Core.Application.Exceptions;

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
    
    protected InvalidEntityReferenceException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
    }
}

[Serializable]
public class InvalidEntityReferenceException<TReferenceKey> : InvalidEntityReferenceException
{
    public TReferenceKey? Id { get; }

    public InvalidEntityReferenceException()
    {
    }
    
    public InvalidEntityReferenceException(TReferenceKey id) : this(id, ApplicationResource.InvalidReference)
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
    
    protected InvalidEntityReferenceException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
    }
}