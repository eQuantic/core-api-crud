using System.ComponentModel.DataAnnotations;

namespace eQuantic.Core.Api.Sample.Entities;

public class ChildExampleRequest
{
    [Required]
    public string? Name { get; set; }
}