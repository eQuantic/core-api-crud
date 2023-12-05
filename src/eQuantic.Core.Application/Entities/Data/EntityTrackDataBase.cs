using System.ComponentModel.DataAnnotations.Schema;

namespace eQuantic.Core.Application.Entities.Data;

public abstract class EntityTrackDataBase : EntityTrackDataBase<int>, IEntityTrack
{
}

public abstract class EntityTrackDataBase<TUserKey> 
    : EntityOwnedDataBase<TUserKey>, IEntityTrack<TUserKey>
{
    public DateTime? UpdatedAt { get; set; }
    public TUserKey? UpdatedById { get; set; }
}

/// <summary>
/// The entity track data base class
/// </summary>
/// <seealso cref="EntityDataBase"/>
public abstract class EntityTrackDataBase<TUser, TUserKey> 
    : EntityOwnedDataBase<TUser, TUserKey>, IEntityTrack<TUser, TUserKey>
{
    /// <summary>
    /// Gets or sets the value of the updated by id
    /// </summary>
    public TUserKey? UpdatedById { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the updated by
    /// </summary>
    public virtual TUser? UpdatedBy { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the updated on
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}