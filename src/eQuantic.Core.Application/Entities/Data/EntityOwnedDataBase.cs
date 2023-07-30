using System.ComponentModel.DataAnnotations.Schema;

namespace eQuantic.Core.Application.Entities.Data;

/// <summary>
/// The entity owned data base class
/// </summary>
/// <seealso cref="EntityDataBase"/>
public class EntityOwnedDataBase<TUser> : EntityDataBase, IEntityOwned<TUser>
{
    /// <summary>
    /// Gets or sets the value of the created by id
    /// </summary>
    public int CreatedById { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the created by
    /// </summary>
    public virtual TUser? CreatedBy { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the created on
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt { get; set; }
}