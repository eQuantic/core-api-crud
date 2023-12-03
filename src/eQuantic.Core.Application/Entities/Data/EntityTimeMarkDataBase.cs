namespace eQuantic.Core.Application.Entities.Data;

public abstract class EntityTimeMarkDataBase : IEntityTimeMark
{
    public DateTime CreatedAt { get; set; }
}