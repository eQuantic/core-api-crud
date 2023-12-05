namespace eQuantic.Core.Application.Entities.Data;

public interface IEntityHistory : IEntityHistory<int>
{
}

public interface IEntityHistory<TUserKey> : IEntityTimeEnded
{
    /// <summary>
    /// Gets or sets the value of the deleted by id
    /// </summary>
    TUserKey? DeletedById { get; set; }
}

public interface IEntityHistory<TUser, TUserKey> : IEntityHistory<TUserKey>
{
    /// <summary>
    /// Gets or sets the value of the deleted by
    /// </summary>
    TUser? DeletedBy { get; set; }
}