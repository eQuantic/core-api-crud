using eQuantic.Core.Domain.Entities;

namespace eQuantic.Core.Api.Sample.Entities;

public class ExampleWithComplexKey : IDomainEntity
{
    public string Code { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}