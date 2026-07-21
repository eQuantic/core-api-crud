using System.Text.Json;
using System.Text.Json.Serialization;
using eQuantic.Core.Api.Crud.Extensions;
using eQuantic.Core.Api.Sample;
using eQuantic.Core.Api.Sample.Entities;
using eQuantic.Core.Api.Sample.Services;
using eQuantic.Core.Application;
using eQuantic.Core.Application.Extensions;
using eQuantic.Core.Data.EntityFramework.Repository.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

builder.Services.AddDateTimeProviderService();

builder.Services.AddDbContext<ExampleDbContext>(opt =>
    opt.UseInMemoryDatabase("ExampleDb"));
        
builder.Services.AddQueryableRepositories<ExampleUnitOfWork>(opt =>
{
    opt.FromAssembly(assembly)
        .AddLifetime(ServiceLifetime.Scoped);
});

builder.Services
    .AddMappers(opt => opt.FromAssembly(assembly))
    .AddHttpContextAccessor()
    .AddTransient<IApplicationContext<int>, ApplicationContext>()
    .AddTransient<IExampleService, ExampleService>()
    .AddTransient<IChildExampleService, ChildExampleService>()
    .AddTransient<IExampleWithComplexKeyService, ExampleWithComplexKeyService>()
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    })
    // FilteringCollection/SortingCollection bind natively in v3 (TryParse) — no custom model binder needed.
    // Auto-register the "traditional" controllers, mirroring MapAllCrud, under the "mvc" group
    // so they coexist with the Minimal API endpoints mapped at the root.
    .AddCrudControllers(opt => opt
        .FromAssembly(assembly)
        .WithGroup("mvc"));

builder.Services
    .AddEndpointsApiExplorer()
    .AddApiCrudDocumentation(opt => opt.WithTitle("Example API"));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseApiCrudDocumentation();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.MapAllCrud(opt => opt
    .FromAssembly(assembly)
    .AllRequireAuthorization()
    .For<ExampleWithComplexKey>().UseOptions(o => o.List.RequireAuthorization(false)));

app.Run();

// Exposed so the integration test project can bootstrap the app via WebApplicationFactory<Program>.
public partial class Program;