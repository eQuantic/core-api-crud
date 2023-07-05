namespace eQuantic.Core.Application.Crud.Entities.Requests;

public interface IReferencedRequest<TReferenceKey>
{
    TReferenceKey? ReferenceId { get; set; }
}