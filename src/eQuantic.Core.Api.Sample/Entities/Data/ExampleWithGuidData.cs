using eQuantic.Core.Application.Entities.Data;
using eQuantic.Linq.Filter;

namespace eQuantic.Core.Api.Sample.Entities.Data;

public class ExampleWithGuidData : EntityDataBase<Guid>
{
    public string Name { get; set; } = string.Empty;
}

public class ChildExampleWithGuidData : EntityDataBase<Guid>, IWithReferenceId<ChildExampleWithGuidData, Guid>
{
    public Guid ExampleId { get; set; }
    public virtual ExampleWithGuidData? Example { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public Guid GetReferenceId()
    {
        return ExampleId;
    }

    public void SetReferenceId(Guid referenceId)
    {
        ExampleId = referenceId;
    }

    public IFiltering<ChildExampleWithGuidData> GetReferenceFiltering()
    {
        return new Filtering<ChildExampleWithGuidData>(o => o.ExampleId, ExampleId.ToString());
    }
}