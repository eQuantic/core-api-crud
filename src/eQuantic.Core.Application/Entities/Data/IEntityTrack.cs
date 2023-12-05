namespace eQuantic.Core.Application.Entities.Data;

public interface IEntityTrack : IEntityTrack<int>
{
}

public interface IEntityTrack<TUserKey> : IEntityTimeTrack
{
    /// <summary>
    /// Gets or sets the value of the updated by id
    /// </summary>
    TUserKey? UpdatedById { get; set; }
}

public interface IEntityTrack<TUser, TUserKey> : IEntityTrack<TUserKey>
{
    /// <summary>
    /// Gets or sets the value of the updated by
    /// </summary>
    TUser? UpdatedBy { get; set; }
}