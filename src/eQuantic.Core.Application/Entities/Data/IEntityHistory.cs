namespace eQuantic.Core.Application.Entities.Data;

public interface IEntityHistory<TUser> : IEntityTimeEnded
{
    /// <summary>
    /// Gets or sets the value of the deleted by id
    /// </summary>
    int? DeletedById { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the deleted by
    /// </summary>
    TUser? DeletedBy { get; set; }
}