using eQuantic.Core.Api.Sample.Entities;
using eQuantic.Core.Api.Sample.Entities.Data;
using eQuantic.Core.Api.Sample.Entities.Requests;
using eQuantic.Core.Application;
using eQuantic.Core.Application.Crud.Attributes;
using eQuantic.Core.Application.Crud.Services;
using eQuantic.Core.Data.Repository;
using eQuantic.Mapper;

namespace eQuantic.Core.Api.Sample.Services;

public interface IChildExampleWithGuidService : ICrudService<ChildExampleWithGuid, ChildExampleWithGuidRequest, Guid> {}

[MapCrudEndpoints(ReferenceType = typeof(ExampleWithGuid), ReferenceKeyType = typeof(Guid))]
public class ChildExampleWithGuidService : CrudServiceBase<ChildExampleWithGuid, ChildExampleWithGuidRequest, ChildExampleWithGuidData, UserData, Guid, int>, IChildExampleWithGuidService
{
    public ChildExampleWithGuidService(
        IApplicationContext<int> applicationContext,
        IQueryableUnitOfWork unitOfWork, 
        IMapperFactory mapperFactory, 
        ILogger<ChildExampleWithGuidService> logger) 
        : base(applicationContext, unitOfWork, mapperFactory, logger)
    {
    }
}