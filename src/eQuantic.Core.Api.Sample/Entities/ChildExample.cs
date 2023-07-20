namespace eQuantic.Core.Api.Sample.Entities;

public class ChildExample
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Example? Example { get; set; }
}