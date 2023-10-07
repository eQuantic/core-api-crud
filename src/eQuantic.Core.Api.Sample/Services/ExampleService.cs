using eQuantic.Core.Api.Sample.Entities;
using eQuantic.Core.Api.Sample.Entities.Data;
using eQuantic.Core.Api.Sample.Entities.Requests;
using eQuantic.Core.Application.Crud.Attributes;
using eQuantic.Core.Application.Crud.Services;
using eQuantic.Core.Data.Repository;
using eQuantic.Mapper;

namespace eQuantic.Core.Api.Sample.Services;

public interface IExampleService : ICrudService<Example, ExampleRequest> {}

[MapCrudEndpoints]
public class ExampleService : CrudServiceBase<Example, ExampleRequest, ExampleData, UserData>, IExampleService
{
    public ExampleService(
        IDefaultUnitOfWork unitOfWork, 
        IMapperFactory mapperFactory, 
        ILogger<ExampleService> logger) : base(unitOfWork, mapperFactory, logger)
    {
    }
}