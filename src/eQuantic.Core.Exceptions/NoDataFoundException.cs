using System.Runtime.Serialization;

namespace eQuantic.Core.Exceptions;

[Serializable]
public class NoDataFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoDataFoundException"/> class.
    /// </summary>
    public NoDataFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoDataFoundException"/> class.
    /// </summary>
    /// <param name="entityType"></param>
    public NoDataFoundException(Type entityType) : base($"No data found for entity {entityType.FullName}")
    {
        this.EntityType = entityType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoDataFoundException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    public NoDataFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
    public NoDataFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
    
    /// <summary>
    /// Entity Type
    /// </summary>
    public Type? EntityType { get; set; }
}