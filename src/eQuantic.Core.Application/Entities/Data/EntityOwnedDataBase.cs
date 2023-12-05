using System.ComponentModel.DataAnnotations.Schema;

namespace eQuantic.Core.Application.Entities.Data;

public class EntityOwnedDataBase : EntityDataBase, IEntityOwned
{
    public DateTime CreatedAt { get; set; }
    public int CreatedById { get; set; }
}

public class EntityOwnedDataBase<TUser> : EntityOwnedDataBase, IEntityOwned<TUser, int>
{
    public TUser? CreatedBy { get; set; }
}

/// <summary>
/// The entity owned data base class
/// </summary>
/// <seealso cref="EntityDataBase"/>
public class EntityOwnedDataBase<TUser, TUserKey> : EntityDataBase, IEntityOwned<TUser, TUserKey>
{
    /// <summary>
    /// Gets or sets the value of the created by id
    /// </summary>
    public TUserKey CreatedById { get; set; } = default!;

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