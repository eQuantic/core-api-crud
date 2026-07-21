using System.Linq.Expressions;
using eQuantic.Core.DataModel;

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

    public Expression<Func<ChildExampleWithGuidData, bool>> GetReferenceFilter()
    {
        var exampleId = ExampleId;
        return o => o.ExampleId == exampleId;
    }
}