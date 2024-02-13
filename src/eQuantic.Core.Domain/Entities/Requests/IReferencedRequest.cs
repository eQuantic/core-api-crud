namespace eQuantic.Core.Domain.Entities.Requests;

public interface IReferencedRequest<TReferenceKey>
{
    TReferenceKey? ReferenceId { get; set; }
}