using System.ComponentModel.DataAnnotations.Schema;

namespace eQuantic.Core.Application.Entities.Data;

/// <summary>
/// The entity history data base class
/// </summary>
/// <seealso cref="EntityTrackDataBase"/>
public abstract class EntityHistoryDataBase<TUser> : EntityTrackDataBase<TUser>, IEntityHistory<TUser>
{
    /// <summary>
    /// Gets or sets the value of the deleted by id
    /// </summary>
    public int? DeletedById { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the deleted by
    /// </summary>
    public virtual TUser? DeletedBy { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the deleted on
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}