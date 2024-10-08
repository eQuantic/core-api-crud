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

public interface IExampleWithComplexKeyService : ICrudService<ExampleWithComplexKey, ExampleWithComplexKeyRequest, ExampleWithComplexKeyData.ExampleKey> {}

[MapCrudEndpoints]
public class ExampleWithComplexKeyService : CrudServiceBase<ExampleWithComplexKey, ExampleWithComplexKeyRequest, ExampleWithComplexKeyData, UserData, ExampleWithComplexKeyData.ExampleKey, int>, IExampleWithComplexKeyService
{
    public ExampleWithComplexKeyService(
        IApplicationContext<int> applicationContext,
        IQueryableUnitOfWork unitOfWork, 
        IDateTimeProviderService dateTimeProviderService,
        IMapperFactory mapperFactory, 
        ILogger<ExampleWithComplexKeyService> logger) 
        : base(applicationContext, unitOfWork, dateTimeProviderService, mapperFactory, logger)
    {
    }
}