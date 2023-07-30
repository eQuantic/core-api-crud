namespace eQuantic.Core.Application.Entities.Data;

public interface IEntityOwned<TUser> : IEntityTimeMark
{
    /// <summary>
    /// Gets or sets the value of the created by id
    /// </summary>
    int CreatedById { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the created by
    /// </summary>
    TUser? CreatedBy { get; set; }
}