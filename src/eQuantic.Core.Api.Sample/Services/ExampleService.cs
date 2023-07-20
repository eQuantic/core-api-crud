using eQuantic.Core.Api.Sample.Entities;
using eQuantic.Core.Application.Crud.Attributes;
using eQuantic.Core.Application.Crud.Services;
using eQuantic.Core.Data.Repository;
using eQuantic.Mapper;

namespace eQuantic.Core.Api.Sample.Services;

public interface IExampleService : ICrudServiceBase<Example, ExampleRequest> {}

[MapCrudEndpoints]
public class ExampleService : CrudServiceBase<Example, ExampleRequest, ExampleData, UserData>, IExampleService
{
    public ExampleService(IDefaultUnitOfWork unitOfWork, IMapperFactory mapperFactory) : base(unitOfWork, mapperFactory)
    {
    }
}