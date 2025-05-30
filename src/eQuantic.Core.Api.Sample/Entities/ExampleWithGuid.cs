using eQuantic.Core.Domain.Entities;

namespace eQuantic.Core.Api.Sample.Entities;

public class ExampleWithGuid : IDomainEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ChildExampleWithGuid : IDomainEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ExampleWithGuid? Example { get; set; }
}