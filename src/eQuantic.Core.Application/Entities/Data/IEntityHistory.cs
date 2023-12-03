namespace eQuantic.Core.Application.Entities.Data;

public interface IEntityHistory<TUser> : IEntityHistory<TUser, int>
{
}

public interface IEntityHistory<TUser, TKey> : IEntityTimeEnded
{
    /// <summary>
    /// Gets or sets the value of the deleted by id
    /// </summary>
    TKey? DeletedById { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the deleted by
    /// </summary>
    TUser? DeletedBy { get; set; }
}