using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Application.Crud.Entities.Requests;

/// <summary>
/// The item request class
/// </summary>
public class ItemRequest
{
    /// <summary>
    /// Gets or sets the value of the id
    /// </summary>
    [FromRoute]
    public int Id { get; set; }

    public ItemRequest()
    {
    }

    public ItemRequest(int id)
    {
        Id = id;
    }
}

public class ItemRequest<TReferenceKey> : ItemRequest, IReferencedRequest<TReferenceKey>
{
    /// <summary>
    /// Gets or sets the value of the reference identifier
    /// </summary>
    [FromRoute]
    public TReferenceKey? ReferenceId { get; set; }

    public ItemRequest()
    {
        
    }

    public ItemRequest(TReferenceKey referenceId, int id) : base(id)
    {
        ReferenceId = referenceId;
    }
}