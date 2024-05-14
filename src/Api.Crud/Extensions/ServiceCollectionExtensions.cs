using eQuantic.Core.Api.Crud.Options;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

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
}