using eQuantic.Core.Application.Resources;

namespace eQuantic.Core.Application.Exceptions;

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
}

[Serializable]
public class EntityNotFoundException<TKey> : EntityNotFoundException
{
    public TKey? Id { get; }

    public EntityNotFoundException()
    {
    }
    
    public EntityNotFoundException(TKey id) : this(id, ApplicationResource.EntityNotFound)
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
}