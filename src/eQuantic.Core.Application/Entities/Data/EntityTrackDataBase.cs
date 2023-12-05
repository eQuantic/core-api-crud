using System.ComponentModel.DataAnnotations.Schema;

namespace eQuantic.Core.Application.Entities.Data;

public abstract class EntityTrackDataBase : EntityOwnedDataBase
{
    public DateTime? UpdatedAt { get; set; }
    public int UpdatedById { get; set; }
}

public abstract class EntityTrackDataBase<TUser> 
    : EntityTrackDataBase, IEntityOwned<TUser, int>, IEntityTrack<TUser, int>
{
    public TUser? CreatedBy { get; set; }
    public TUser? UpdatedBy { get; set; }
}

/// <summary>
/// The entity track data base class
/// </summary>
/// <seealso cref="EntityDataBase"/>
public abstract class EntityTrackDataBase<TUser, TKey> 
    : EntityOwnedDataBase<TUser, TKey>, IEntityTrack<TUser, TKey>
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