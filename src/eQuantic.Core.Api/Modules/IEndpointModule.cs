using Microsoft.AspNetCore.Routing;

namespace eQuantic.Core.Api.Modules;

public interface IEndpointModule
{
    void MapEndpoints(IEndpointRouteBuilder endpoints);
}