using System.ComponentModel.DataAnnotations.Schema;

namespace eQuantic.Core.Application.Entities.Data;

public interface IEntityTrack<TUser>
{
    /// <summary>
    /// Gets or sets the value of the updated by id
    /// </summary>
    int? UpdatedById { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the updated by
    /// </summary>
    TUser? UpdatedBy { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the updated on
    /// </summary>
    DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// The entity track data base class
/// </summary>
/// <seealso cref="EntityDataBase"/>
public abstract class EntityTrackDataBase<TUser> : EntityOwnedDataBase<TUser>, IEntityTrack<TUser>
{
    /// <summary>
    /// Gets or sets the value of the updated by id
    /// </summary>
    public int? UpdatedById { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the updated by
    /// </summary>
    [ForeignKey(nameof(UpdatedById))]
    public virtual TUser? UpdatedBy { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the updated on
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}