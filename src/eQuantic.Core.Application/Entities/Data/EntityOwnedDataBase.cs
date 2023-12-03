using System.ComponentModel.DataAnnotations.Schema;

namespace eQuantic.Core.Application.Entities.Data;

public class EntityOwnedDataBase<TUser> : EntityOwnedDataBase<TUser, int>, IEntityOwned<TUser>
{
}

/// <summary>
/// The entity owned data base class
/// </summary>
/// <seealso cref="EntityDataBase"/>
public class EntityOwnedDataBase<TUser, TKey> : EntityDataBase, IEntityOwned<TUser, TKey>
{
    /// <summary>
    /// Gets or sets the value of the created by id
    /// </summary>
    public TKey CreatedById { get; set; } = default!;

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