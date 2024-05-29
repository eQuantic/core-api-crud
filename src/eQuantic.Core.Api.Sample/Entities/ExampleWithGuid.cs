namespace eQuantic.Core.Api.Sample.Entities;

public class ExampleWithGuid
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ChildExampleWithGuid
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ExampleWithGuid? Example { get; set; }
}