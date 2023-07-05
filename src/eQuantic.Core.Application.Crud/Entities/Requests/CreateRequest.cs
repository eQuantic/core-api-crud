using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Application.Crud.Entities.Requests;

/// <summary>
/// The update request class
/// </summary>
/// <seealso cref="CreateRequest{TBody}"/>
public class CreateRequest<TBody>
{
    /// <summary>
    /// Gets or sets the value of the body
    /// </summary>
    [FromBody]
    public TBody? Body { get; set; }

    public CreateRequest()
    {
        
    }

    public CreateRequest(TBody body)
    {
        Body = body;
    }
}

public class CreateRequest<TBody, TReferenceKey> : CreateRequest<TBody>, IReferencedRequest<TReferenceKey>
{
    /// <summary>
    /// Gets or sets the value of the reference identifier
    /// </summary>
    [FromRoute]
    public TReferenceKey? ReferenceId { get; set; }
    
    public CreateRequest()
    {
        
    }
    
    public CreateRequest(TReferenceKey referenceId, TBody body) : base(body)
    {
        ReferenceId = referenceId;
    }
}