using eQuantic.Core.Api.Sample.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace eQuantic.Core.Api.Crud.Tests;

/// <summary>
/// Boots the Sample app and replaces the EF-backed services with in-memory fakes, so the tests
/// exercise the generated/explicit controllers without depending on the data layer.
/// </summary>
public sealed class ControllerApiFactory : WebApplicationFactory<Program>
{
    public FakeExampleService Examples { get; } = new();
    public FakeChildExampleService Children { get; } = new();
    public FakeExampleWithComplexKeyService Complex { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IExampleService>();
            services.AddSingleton<IExampleService>(Examples);

            services.RemoveAll<IChildExampleService>();
            services.AddSingleton<IChildExampleService>(Children);

            services.RemoveAll<IExampleWithComplexKeyService>();
            services.AddSingleton<IExampleWithComplexKeyService>(Complex);
        });
    }
}
