using System.Reflection;
using eQuantic.Core.Api.Middlewares;
using eQuantic.Core.Api.Options;
using eQuantic.Core.Api.Resources;
using eQuantic.Core.Api.Swagger;
using eQuantic.Linq.Filter;
using eQuantic.Linq.Sorter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace eQuantic.Core.Api.Extensions;

/// <summary>
/// The Service Collection Extensions
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add the api documentation
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="options">The documentation options</param>
    /// <returns>The registry</returns>
    public static IServiceCollection AddApiDocumentation(this IServiceCollection services, Action<DocumentationOptions>? options = null)
    {
        var docOptions = new DocumentationOptions();
        options?.Invoke(docOptions);
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = docOptions.Title, Version = "v1" });
            c.DescribeAllParametersInCamelCase();
            
            if(!string.IsNullOrEmpty(docOptions.XmlCommentsFile))
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, docOptions.XmlCommentsFile));
            
            c.EnableAnnotations();
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = @"JWT Authorization header using the Bearer scheme.<br />
                Enter 'Bearer' [space] and then your token in the text input below.<br />
                Example: ""Bearer 12345abcdef""",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            var filteringDesc = string.Format(
                ApiResource.FilteringDescription,
                $"property:{string.Join("|", FilterOperatorValues.Values.Select(x => x.Value))}(value)",
                $"{string.Join("|", CompositeOperatorValues.Values.Select(x => x.Value))}(property:eq(value1), property:eq(value2))");
            
            c.MapType<IFiltering[]>(() => new OpenApiSchema());
            c.MapType<ISorting[]>(() => new OpenApiSchema());
            c.OperationFilter<ExpressionOperationFilter<IFiltering[]>>(filteringDesc);
            c.OperationFilter<ExpressionOperationFilter<ISorting[]>>(ApiResource.SortingDescription);
            c.SchemaFilter<ExpressionSchemaFilter<IFiltering[]>>();
            c.SchemaFilter<ExpressionSchemaFilter<ISorting[]>>();
        });
        return services;
    }

    /// <summary>
    /// Add exception filter
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddExceptionFilter(this IServiceCollection services,
        Action<ExceptionFilterOptions>? options = null)
    {
        var filterOptions = new ExceptionFilterOptions();
        options?.Invoke(filterOptions);

        services.AddSingleton(filterOptions);
        services.AddTransient<ExceptionMiddleware>();
        return services;
    }
}