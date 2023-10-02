using System.Reflection;
using eQuantic.Core.Data.Repository;

namespace eQuantic.Core.Api.Sample.Entities.Data;

public class ExampleWithComplexKeyData : IEntity<ExampleWithComplexKeyData.ExampleKey>
{
    public record struct ExampleKey(string Code, string Location)
    {
        public static ValueTask<ExampleKey> BindAsync(HttpContext httpContext, ParameterInfo parameter)
        {
            return ValueTask.FromResult(
                new ExampleKey(
                    httpContext.Request.RouteValues["code"]!.ToString()!, 
                    httpContext.Request.RouteValues["location"]!.ToString()!
                )
            );
        }
    }

    public string Code { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    
    public ExampleKey GetKey()
    {
        return new ExampleKey(Code, Location);
    }

    public void SetKey(ExampleKey key)
    {
        Code = key.Code;
        Location = key.Location;
    }
}