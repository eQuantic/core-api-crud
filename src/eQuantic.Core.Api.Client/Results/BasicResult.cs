using System.Text.Json.Serialization;

namespace eQuantic.Core.Api.Client.Results;

public class BasicResult
{
    public bool Succeeded { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorMessage { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Errors { get; set; }
}