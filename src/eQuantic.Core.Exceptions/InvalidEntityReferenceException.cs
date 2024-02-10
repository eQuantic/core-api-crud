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
}