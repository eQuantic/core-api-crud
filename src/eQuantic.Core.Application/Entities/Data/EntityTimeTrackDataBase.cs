namespace eQuantic.Core.Application.Entities.Data;

public abstract class EntityTimeTrackDataBase : EntityTimeMarkDataBase, IEntityTimeTrack
{
    public DateTime? UpdatedAt { get; set; }
}