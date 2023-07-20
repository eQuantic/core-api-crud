using eQuantic.Core.Api.Sample.Entities;
using eQuantic.Core.Application.Crud.Attributes;
using eQuantic.Core.Application.Crud.Services;
using eQuantic.Core.Data.Repository;
using eQuantic.Mapper;

namespace eQuantic.Core.Api.Sample.Services;

public interface IChildExampleService : ICrudServiceBase<ChildExample, ChildExampleRequest> {}

[MapCrudEndpoints(ReferenceType = typeof(Example))]
public class ChildExampleService : CrudServiceBase<ChildExample, ChildExampleRequest, ChildExampleData, UserData>, IChildExampleService
{
    public ChildExampleService(IDefaultUnitOfWork unitOfWork, IMapperFactory mapperFactory) : base(unitOfWork, mapperFactory)
    {
    }
}