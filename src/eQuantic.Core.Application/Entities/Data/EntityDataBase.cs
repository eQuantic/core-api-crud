using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eQuantic.Core.Data.Repository;

namespace eQuantic.Core.Application.Entities.Data;

public abstract class EntityDataBase : IEntity<int>
{
    /// <summary>
    /// Gets or sets the value of the id
    /// </summary>
    [Key]
    [Column(Order = 1)]
    public int Id { get; set; }
    
    /// <summary>
    /// Implicit operator to compare entity with id
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static implicit operator int(EntityDataBase entity)
    {
        return entity.Id;
    }
    
    /// <summary>
    /// Gets the key
    /// </summary>
    /// <returns>The id</returns>
    public int GetKey()
    {
        return Id;
    }

    /// <summary>
    /// Sets the key
    /// </summary>
    /// <param name="key"></param>
    public void SetKey(int key)
    {
        Id = key;
    }
}