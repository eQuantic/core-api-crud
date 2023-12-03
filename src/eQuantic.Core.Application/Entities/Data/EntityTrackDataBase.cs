using System.ComponentModel.DataAnnotations.Schema;

namespace eQuantic.Core.Application.Entities.Data;

public abstract class EntityTrackDataBase : EntityTimeMarkDataBase, IEntityTimeTrack
{
    public DateTime? UpdatedAt { get; set; }
}

public abstract class EntityTrackDataBase<TUser> : EntityTrackDataBase<TUser, int>, IEntityOwned<TUser>, IEntityTrack<TUser>
{
}

/// <summary>
/// The entity track data base class
/// </summary>
/// <seealso cref="EntityDataBase"/>
public abstract class EntityTrackDataBase<TUser, TKey> : EntityOwnedDataBase<TUser, TKey>, IEntityTrack<TUser, TKey>
{
    /// <summary>
    /// Gets or sets the value of the updated by id
    /// </summary>
    public TKey? UpdatedById { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the updated by
    /// </summary>
    public virtual TUser? UpdatedBy { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the updated on
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}