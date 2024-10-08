using eQuantic.Core.Api.Sample.Entities;
using eQuantic.Core.Api.Sample.Entities.Data;
using eQuantic.Core.Api.Sample.Entities.Requests;
using eQuantic.Core.Application;
using eQuantic.Core.Application.Crud.Attributes;
using eQuantic.Core.Application.Crud.Services;
using eQuantic.Core.Application.Services;
using eQuantic.Core.Data.Repository;
using eQuantic.Mapper;

namespace eQuantic.Core.Api.Sample.Services;

public interface IChildExampleService : ICrudService<ChildExample, ChildExampleRequest> {}

[MapCrudEndpoints(ReferenceType = typeof(Example))]
public class ChildExampleService : CrudServiceBase<ChildExample, ChildExampleRequest, ChildExampleData, UserData>, IChildExampleService
{
    public ChildExampleService(
        IApplicationContext<int> applicationContext,
        IQueryableUnitOfWork unitOfWork,
        IDateTimeProviderService dateTimeProviderService,
        IMapperFactory mapperFactory, 
        ILogger<ChildExampleService> logger) 
        : base(applicationContext, unitOfWork, dateTimeProviderService, mapperFactory, logger)
    {
    }
}