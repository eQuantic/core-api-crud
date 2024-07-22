using eQuantic.Core.Api.Crud.Binders;
using eQuantic.Core.Api.Crud.Options;
using eQuantic.Core.Api.Extensions;
using eQuantic.Core.Api.Options;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace eQuantic.Core.Api.Crud.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services, Action<CrudServiceOptions>? options)
    {
        var crudServiceOptions = new CrudServiceOptions();
        options?.Invoke(crudServiceOptions);
        services.AddValidatorsFromAssembly(crudServiceOptions.GetAssembly());
        
        services.AddFluentValidationAutoValidation(fv => {
            fv.DisableDataAnnotationsValidation = true;
        });
        return services;
    }

    public static IServiceCollection AddApiDocumentation(
        this IServiceCollection services,
        Action<DocumentationOptions>? options = null,
        Action<SwaggerGenOptions>? swaggerGenOptions = null)
    {
        return eQuantic.Core.Api.Extensions.ServiceCollectionExtensions.AddApiDocumentation(services, options, opt =>
        {
            opt.AddFilteringOperationFilter<FilteringCollection>();
            swaggerGenOptions?.Invoke(opt);
        });
    }
}