using eQuantic.Core.Domain.Entities;

namespace eQuantic.Core.Api.Sample.Entities;

public class Example : IDomainEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}