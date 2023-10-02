using eQuantic.Core.Api.Sample.Entities;
using eQuantic.Core.Api.Sample.Entities.Data;
using eQuantic.Core.Api.Sample.Entities.Requests;
using eQuantic.Mapper;

namespace eQuantic.Core.Api.Sample.Mappers;

public class ExampleMapper : IMapper<ExampleData, Example>, IMapper<ExampleRequest, ExampleData>
{
    public Example? Map(ExampleData? source)
    {
        return Map(source, new Example());
    }

    public Example? Map(ExampleData? source, Example? destination)
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

    public ExampleData? Map(ExampleRequest? source)
    {
        return Map(source, new ExampleData());
    }

    public ExampleData? Map(ExampleRequest? source, ExampleData? destination)
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