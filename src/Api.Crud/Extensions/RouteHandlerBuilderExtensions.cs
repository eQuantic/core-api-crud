using eQuantic.Core.Api.Crud.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;

namespace eQuantic.Core.Api.Crud.Extensions;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithReferenceId(this RouteHandlerBuilder endpoint, EndpointReferenceOptions options)
    {
        return endpoint.WithOpenApi(op =>
        {
            op.Parameters.Insert(0, GetReferenceParameter(options));
            return op;
        });
    }
    
    private static OpenApiParameter GetReferenceParameter(EndpointReferenceOptions options)
    {
        return new OpenApiParameter
        {
            Required = true,
            Name = options.Name,
            In = ParameterLocation.Path,
            Schema = options.KeyType.ToSchema()
        };
    }
}