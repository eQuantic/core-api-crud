using eQuantic.Core.Api.Crud.Options;
using Microsoft.AspNetCore.Builder;
#if NET10_0_OR_GREATER
using Microsoft.OpenApi;
#endif

namespace eQuantic.Core.Api.Crud.Extensions;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithReferenceId(this RouteHandlerBuilder endpoint, EndpointReferenceOptions options)
    {
#if NET10_0_OR_GREATER
        // Minimal-API OpenAPI enrichment relies on Microsoft.OpenApi 2.x, which only aligns with the
        // Microsoft.AspNetCore.OpenApi that ships with net10; on net8 the reference parameter is still
        // documented through the Swashbuckle operation filter.
        return endpoint.WithOpenApi(op =>
        {
            op.Parameters.Insert(0, GetReferenceParameter(options));
            return op;
        });
#else
        _ = options;
        return endpoint;
#endif
    }

#if NET10_0_OR_GREATER
    private static OpenApiParameter GetReferenceParameter(EndpointReferenceOptions options) =>
        new()
        {
            Required = true,
            Name = options.Name,
            In = ParameterLocation.Path,
            Schema = options.KeyType.ToSchema()
        };
#endif
}
