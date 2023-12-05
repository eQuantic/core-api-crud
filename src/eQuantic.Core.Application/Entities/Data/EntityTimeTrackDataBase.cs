namespace eQuantic.Core.Application.Entities.Data;

public abstract class EntityTimeTrackDataBase : EntityTimeMarkDataBase, IEntityTimeTrack
{
    public DateTime? UpdatedAt { get; set; }
}

public abstract class EntityTimeTrackDataBase<TKey> : EntityTimeMarkDataBase<TKey>, IEntityTimeTrack
{
    public DateTime? UpdatedAt { get; set; }
}