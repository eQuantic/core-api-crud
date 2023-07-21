using System.Runtime.Serialization;
using eQuantic.Core.Application.Resources;

namespace eQuantic.Core.Application.Exceptions;

[Serializable]
public class InvalidReferenceException : Exception
{
    public InvalidReferenceException()
    {
    }

    public InvalidReferenceException(string message) 
        : base(message)
    {
    }

    public InvalidReferenceException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
    
    protected InvalidReferenceException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
    }
}

[Serializable]
public class InvalidReferenceException<TReferenceKey> : InvalidReferenceException
{
    public TReferenceKey? Id { get; }

    public InvalidReferenceException()
    {
    }
    
    public InvalidReferenceException(TReferenceKey id) : this(id, ApplicationResource.InvalidReference)
    {
    }

    public InvalidReferenceException(TReferenceKey id, string message) 
        : base(message)
    {
        Id = id;
    }

    public InvalidReferenceException(TReferenceKey id, string message, Exception innerException) 
        : base(message, innerException)
    {
        Id = id;
    }
    
    protected InvalidReferenceException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
    }
}