using System.ComponentModel.DataAnnotations.Schema;

namespace eQuantic.Core.Application.Entities.Data;

public abstract class EntityHistoryDataBase : EntityHistoryDataBase<int>, IEntityHistory
{
}

public abstract class EntityHistoryDataBase<TUserKey> 
    : EntityTrackDataBase<TUserKey>, IEntityHistory<TUserKey>
{
    public DateTime? DeletedAt { get; set; }
    public TUserKey? DeletedById { get; set; }
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