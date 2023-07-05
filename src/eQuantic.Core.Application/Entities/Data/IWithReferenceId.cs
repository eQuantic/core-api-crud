using eQuantic.Core.Data.Repository;
using eQuantic.Linq.Filter;

namespace eQuantic.Core.Application.Entities.Data;

public interface IWithReferenceId<TDataEntity, TKey> where TDataEntity : IEntity
{
    TKey GetReferenceId();
    void SetReferenceId(TKey referenceId);
    IFiltering<TDataEntity> GetReferenceFiltering();
}