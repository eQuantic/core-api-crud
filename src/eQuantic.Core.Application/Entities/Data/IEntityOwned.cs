namespace eQuantic.Core.Application.Entities.Data;

public interface IEntityOwned<TUser> : IEntityOwned<TUser, int>
{
}

public interface IEntityOwned<TUser, TKey> : IEntityTimeMark
{
    /// <summary>
    /// Gets or sets the value of the created by id
    /// </summary>
    TKey CreatedById { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the created by
    /// </summary>
    TUser? CreatedBy { get; set; }
}