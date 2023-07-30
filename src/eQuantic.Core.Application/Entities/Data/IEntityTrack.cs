namespace eQuantic.Core.Application.Entities.Data;

public interface IEntityTrack<TUser> : IEntityTimeTrack
{
    /// <summary>
    /// Gets or sets the value of the updated by id
    /// </summary>
    int? UpdatedById { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the updated by
    /// </summary>
    TUser? UpdatedBy { get; set; }
}