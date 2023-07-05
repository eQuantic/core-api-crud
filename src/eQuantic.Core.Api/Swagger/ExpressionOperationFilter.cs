using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace eQuantic.Core.Api.Swagger;

/// <summary>
/// The expression operation filter class
/// </summary>
/// <seealso cref="ISchemaFilter"/>
public class ExpressionOperationFilter<TColumn> : IOperationFilter
{
    /// <summary>
    /// The parameter description suffix
    /// </summary>
    private readonly string _parameterDescriptionSuffix;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionOperationFilter{TColumn}"/> class
    /// </summary>
    /// <param name="parameterDescriptionSuffix">The parameter description suffix</param>
    public ExpressionOperationFilter(string parameterDescriptionSuffix = "")
    {
        _parameterDescriptionSuffix = parameterDescriptionSuffix;
    }
    /// <summary>
    /// Applies the operation
    /// </summary>
    /// <param name="operation">The operation</param>
    /// <param name="context">The context</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var parameters = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(TColumn))
            .Select(p => p.Name!)
            .ToList();
        if (!parameters.Any())
        {
            parameters = context.MethodInfo.GetParameters()
                .SelectMany(p => p.ParameterType.GetProperties())
                .Where(o => o.PropertyType == typeof(TColumn))
                .Select(o => o.Name)
                .ToList();
        }

        if (!parameters.Any())
        {
            return;
        }

        var openApiParameters = parameters
            .Select(parameter =>
                operation.Parameters.FirstOrDefault(o =>
                    o.Name.Equals(parameter, StringComparison.InvariantCultureIgnoreCase)))
            .Where(openApiParameter => openApiParameter != null);
        
        foreach (var openApiParameter in openApiParameters)
        {
            openApiParameter!.Description = $"{openApiParameter.Description}{(!string.IsNullOrEmpty(_parameterDescriptionSuffix) ? $" -- {_parameterDescriptionSuffix}" : "")}";
            openApiParameter.Schema.Type = "string";
        }
    }
}