namespace eQuantic.Core.Application.Entities.Data;

public interface IEntityTrack : IEntityTrack<int>
{
}

public interface IEntityTrack<TUserKey> : IEntityTimeTrack 
    where TUserKey : struct
{
    /// <summary>
    /// Gets or sets the value of the updated by id
    /// </summary>
    Nullable<TUserKey> UpdatedById { get; set; }
}

public interface IEntityTrack<TUser, TUserKey> : IEntityTrack<TUserKey>
    where TUserKey : struct
{
    /// <summary>
    /// Gets or sets the value of the updated by
    /// </summary>
    TUser? UpdatedBy { get; set; }
}