using eQuantic.Core.Api.Crud.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;

namespace eQuantic.Core.Api.Crud.Extensions;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithReferenceId(this RouteHandlerBuilder endpoint, EndpointOptions options)
    {
        return endpoint.WithOpenApi(op =>
        {
            op.Parameters.Insert(0, GetReferenceParameter(options));
            return op;
        });
    }
    
    private static OpenApiParameter GetReferenceParameter(EndpointOptions options)
    {
        return new OpenApiParameter
        {
            Required = true,
            Name = options.ReferenceType?.GetReferenceName() ?? "referenceId",
            In = ParameterLocation.Path,
            Schema = options.ReferenceKeyType?.ToSchema()
        };
    }
}