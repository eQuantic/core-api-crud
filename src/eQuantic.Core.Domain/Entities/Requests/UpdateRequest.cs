using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Domain.Entities.Requests;

public class UpdateRequest<TBody> : UpdateRequest<TBody, int>
{
}

/// <summary>
/// The update request class
/// </summary>
/// <seealso cref="ItemRequest"/>
public class UpdateRequest<TBody, TKey> : ItemRequest<TKey>
{
    /// <summary>
    /// Gets or sets the value of the body
    /// </summary>
    [FromBody]
    public TBody? Body { get; set; }

    public UpdateRequest()
    {
        
    }

    public UpdateRequest(TKey id, TBody body) : base(id)
    {
        Body = body;
    }
}

public class UpdateRequest<TBody, TKey, TReferenceKey> : UpdateRequest<TBody, TKey>, IReferencedRequest<TReferenceKey>
{
    /// <summary>
    /// Gets or sets the value of the reference identifier
    /// </summary>
    [FromRoute]
    public TReferenceKey? ReferenceId { get; set; }
    
    public UpdateRequest()
    {
        
    }
    
    public UpdateRequest(TReferenceKey referenceId, TKey id, TBody body) : base(id, body)
    {
        ReferenceId = referenceId;
    }
}