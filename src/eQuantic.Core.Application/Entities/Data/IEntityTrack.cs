namespace eQuantic.Core.Application.Entities.Data;

public interface IEntityTrack<TUser> : IEntityTrack<TUser, int>
{
}

public interface IEntityTrack<TUser, TKey> : IEntityTimeTrack
{
    /// <summary>
    /// Gets or sets the value of the updated by id
    /// </summary>
    TKey? UpdatedById { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the updated by
    /// </summary>
    TUser? UpdatedBy { get; set; }
}