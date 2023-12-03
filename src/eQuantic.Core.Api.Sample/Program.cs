using System.Text.Json;
using System.Text.Json.Serialization;
using eQuantic.Core.Api.Crud.Extensions;
using eQuantic.Core.Api.Extensions;
using eQuantic.Core.Api.Sample;
using eQuantic.Core.Api.Sample.Services;
using eQuantic.Core.Application;
using eQuantic.Core.Data.EntityFramework.Repository.Extensions;
using eQuantic.Core.Mvc.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

builder.Services.AddDbContext<ExampleDbContext>(opt =>
    opt.UseInMemoryDatabase("ExampleDb"));
        
builder.Services.AddQueryableRepositories<ExampleUnitOfWork>(opt =>
{
    opt.FromAssembly(assembly)
        .AddLifetime(ServiceLifetime.Scoped);
});

builder.Services
    .AddMappers(opt => opt.FromAssembly(assembly))
    .AddTransient<IApplicationContext<int>, ApplicationContext>()
    .AddTransient<IExampleService, ExampleService>()
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    })
    .AddFilterModelBinder()
    .AddSortModelBinder();

builder.Services
    .AddEndpointsApiExplorer()
    .AddApiDocumentation(opt => opt.WithTitle("Example API"));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseApiDocumentation();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.MapAllCrud(opt => opt.FromAssembly(assembly));

app.Run();