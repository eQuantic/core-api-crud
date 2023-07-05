using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace eQuantic.Core.Api.Swagger;

/// <summary>
/// The expression schema filter class
/// </summary>
/// <seealso cref="ISchemaFilter"/>
public class ExpressionSchemaFilter<TColumn> : ISchemaFilter
{
    /// <summary>
    /// Applies the schema
    /// </summary>
    /// <param name="schema">The schema</param>
    /// <param name="context">The context</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if(context.Type != typeof(TColumn))
            return;

        schema.Type = "string";
    }
}