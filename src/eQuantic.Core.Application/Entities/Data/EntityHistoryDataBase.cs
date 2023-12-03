using System.ComponentModel.DataAnnotations.Schema;

namespace eQuantic.Core.Application.Entities.Data;

public abstract class EntityHistoryDataBase : EntityTrackDataBase, IEntityTimeEnded
{
    public DateTime? DeletedAt { get; set; }
}

public abstract class EntityHistoryDataBase<TUser> : EntityHistoryDataBase<TUser, int>, IEntityOwned<TUser>, IEntityTrack<TUser>, IEntityHistory<TUser>
{
    
}

/// <summary>
/// The entity history data base class
/// </summary>
/// <seealso cref="EntityTrackDataBase"/>
public abstract class EntityHistoryDataBase<TUser, TKey> : EntityTrackDataBase<TUser, TKey>, IEntityHistory<TUser, TKey>
{
    /// <summary>
    /// Gets or sets the value of the deleted by id
    /// </summary>
    public TKey? DeletedById { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the deleted by
    /// </summary>
    public virtual TUser? DeletedBy { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the deleted on
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}