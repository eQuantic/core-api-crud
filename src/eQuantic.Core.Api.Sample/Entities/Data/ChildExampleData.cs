using eQuantic.Core.Application.Entities.Data;
using eQuantic.Linq.Filter;

namespace eQuantic.Core.Api.Sample.Entities.Data;

public class ChildExampleData : EntityDataBase, IWithReferenceId<ChildExampleData, int>
{
    public int ExampleId { get; set; }
    public virtual ExampleData? Example { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public int GetReferenceId()
    {
        return ExampleId;
    }

    public void SetReferenceId(int referenceId)
    {
        ExampleId = referenceId;
    }

    public IFiltering<ChildExampleData> GetReferenceFiltering()
    {
        return new Filtering<ChildExampleData>(o => o.ExampleId, ExampleId.ToString());
    }
}