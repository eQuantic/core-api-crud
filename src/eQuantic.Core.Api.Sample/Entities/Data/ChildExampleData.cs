using System.Linq.Expressions;
using eQuantic.Core.DataModel;

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

    public Expression<Func<ChildExampleData, bool>> GetReferenceFilter()
    {
        var exampleId = ExampleId;
        return o => o.ExampleId == exampleId;
    }
}