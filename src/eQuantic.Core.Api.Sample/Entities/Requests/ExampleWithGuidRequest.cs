using System.ComponentModel.DataAnnotations;

namespace eQuantic.Core.Api.Sample.Entities.Requests;

public class ExampleWithGuidRequest
{
    [Required]
    public string? Name { get; set; }
}

public class ChildExampleWithGuidRequest
{
    [Required]
    public string? Name { get; set; }
}