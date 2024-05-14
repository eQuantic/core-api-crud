using Microsoft.Extensions.DependencyInjection;

namespace eQuantic.Core.Api.Crud.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCrudHttpClient<TClient, TClientImplementation, TEntity, TRequest, TKey>(
        this IServiceCollection services, Action<HttpClient>? client = null) 
        where TClient : class, ICrudClient<TEntity, TRequest, TKey>
        where TClientImplementation : class, TClient
    {
        if(client != null)
            services.AddHttpClient<TClient, TClientImplementation>(client);
        else 
            services.AddHttpClient<TClient, TClientImplementation>();
        
        return services;
    }
}