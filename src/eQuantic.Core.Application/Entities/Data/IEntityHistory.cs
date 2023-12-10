namespace eQuantic.Core.Application.Entities.Data;

public interface IEntityHistory : IEntityHistory<int>
{
}

public interface IEntityHistory<TUserKey> : IEntityTimeEnded
    where TUserKey : struct
{
    /// <summary>
    /// Gets or sets the value of the deleted by id
    /// </summary>
    Nullable<TUserKey> DeletedById { get; set; }
}

public interface IEntityHistory<TUser, TUserKey> : IEntityHistory<TUserKey>
    where TUserKey : struct
{
    /// <summary>
    /// Gets or sets the value of the deleted by
    /// </summary>
    TUser? DeletedBy { get; set; }
}