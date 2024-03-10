using System.Runtime.Serialization;

namespace eQuantic.Core.Exceptions;

[Serializable]
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
    
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
    public DependencyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
}