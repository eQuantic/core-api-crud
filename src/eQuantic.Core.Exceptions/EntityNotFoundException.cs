using System.Runtime.Serialization;
using eQuantic.Core.Exceptions.Resources;

namespace eQuantic.Core.Exceptions;

[Serializable]
public class EntityNotFoundException : Exception
{
    public EntityNotFoundException()
    {
    }

    public EntityNotFoundException(string message) 
        : base(message)
    {
    }

    public EntityNotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
    
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
    public EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
}

[Serializable]
public class EntityNotFoundException<TKey> : EntityNotFoundException
{
    public TKey? Id { get; }

    public EntityNotFoundException()
    {
    }
    
    public EntityNotFoundException(TKey id) : this(id, ExceptionsResource.EntityNotFound)
    {
    }

    public EntityNotFoundException(TKey id, string message) 
        : base(message)
    {
        Id = id;
    }

    public EntityNotFoundException(TKey id, string message, Exception innerException) 
        : base(message, innerException)
    {
        Id = id;
    }
    
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
    public EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
}