using System.Reflection;

namespace eQuantic.Core.Api.Options;

public class DocumentationOptions
{
    public string? Title { get; set; }
    public Assembly? Assembly { get; set; }
}