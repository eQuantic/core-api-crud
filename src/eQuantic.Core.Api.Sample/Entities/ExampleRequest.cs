using System.ComponentModel.DataAnnotations;

namespace eQuantic.Core.Api.Sample.Entities;

public class ExampleRequest
{
    [Required]
    public string? Name { get; set; }
}