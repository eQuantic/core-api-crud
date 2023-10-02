using eQuantic.Core.Api.Sample.Entities;
using eQuantic.Core.Api.Sample.Entities.Data;
using eQuantic.Core.Api.Sample.Entities.Requests;
using eQuantic.Mapper;

namespace eQuantic.Core.Api.Sample.Mappers;

public class ChildExampleMapper: IMapper<ChildExampleData, ChildExample>, IMapper<ChildExampleRequest, ChildExampleData>
{
    public ChildExample? Map(ChildExampleData? source)
    {
        return Map(source, new ChildExample());
    }

    public ChildExample? Map(ChildExampleData? source, ChildExample? destination)
    {
        if (source == null)
        {
            return null;
        }

        if (destination == null)
        {
            return Map(source);
        }

        destination.Id = source.Id;
        destination.Name = source.Name;

        return destination;
    }

    public ChildExampleData? Map(ChildExampleRequest? source)
    {
        return Map(source, new ChildExampleData());
    }

    public ChildExampleData? Map(ChildExampleRequest? source, ChildExampleData? destination)
    {
        if (source == null)
        {
            return null;
        }

        if (destination == null)
        {
            return Map(source);
        }
        
        destination.Name = source.Name ?? string.Empty;

        return destination;
    }
}