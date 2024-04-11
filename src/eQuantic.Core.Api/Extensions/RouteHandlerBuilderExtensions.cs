using eQuantic.Core.Application.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace eQuantic.Core.Api.Extensions;

public static class RouteHandlerBuilderExtensions
{
    public static void AddEndpointFilter(this RouteHandlerBuilder endpoint, Type filterType)
    {
        typeof(RouteHandlerBuilderExtensions)
            .InvokePrivateStaticMethod(nameof(AddEndpointFilter), filterType, [endpoint]);
    }
    
    private static void AddEndpointFilter<TFilterType>(RouteHandlerBuilder endpoint) where TFilterType : IEndpointFilter
    {
        endpoint.AddEndpointFilter<TFilterType>();
    }
}