using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Application.Crud.Entities.Requests;

/// <summary>
/// The update request class
/// </summary>
/// <seealso cref="ItemRequest"/>
public class UpdateRequest<TBody> : ItemRequest
{
    /// <summary>
    /// Gets or sets the value of the body
    /// </summary>
    [FromBody]
    public TBody? Body { get; set; }

    public UpdateRequest()
    {
        
    }

    public UpdateRequest(int id, TBody body) : base(id)
    {
        Body = body;
    }
}

public class UpdateRequest<TBody, TReferenceKey> : UpdateRequest<TBody>, IReferencedRequest<TReferenceKey>
{
    /// <summary>
    /// Gets or sets the value of the reference identifier
    /// </summary>
    [FromRoute]
    public TReferenceKey? ReferenceId { get; set; }
    
    public UpdateRequest()
    {
        
    }
    
    public UpdateRequest(TReferenceKey referenceId, int id, TBody body) : base(id, body)
    {
        ReferenceId = referenceId;
    }
}