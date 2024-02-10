namespace eQuantic.Core.Exceptions;

public class DependencyNotFoundException: Exception
{
    public Type? DependencyType { get; }

    public DependencyNotFoundException()
    {
    }

    public DependencyNotFoundException(Type dependencyType) : base($"Dependency not found of type '{dependencyType.FullName}'")
    {
        DependencyType = dependencyType;
    }
    
    public DependencyNotFoundException(string message) 
        : base(message)
    {
    }

    public DependencyNotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}