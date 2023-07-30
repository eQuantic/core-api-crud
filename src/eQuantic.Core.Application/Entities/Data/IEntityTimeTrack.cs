namespace eQuantic.Core.Application.Entities.Data;

public interface IEntityTimeTrack
{
    /// <summary>
    /// Gets or sets the value of the updated on
    /// </summary>
    DateTime? UpdatedAt { get; set; }
}