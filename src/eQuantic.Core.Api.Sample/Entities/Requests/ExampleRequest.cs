using System.ComponentModel.DataAnnotations;

namespace eQuantic.Core.Api.Sample.Entities.Requests;

public class ExampleRequest
{
    [Required]
    public string? Name { get; set; }
}