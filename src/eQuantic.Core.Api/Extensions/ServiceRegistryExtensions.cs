using System.Reflection;
using eQuantic.Core.Api.Modules;
using Lamar;

namespace eQuantic.Core.Api.Extensions;

public static class ServiceRegistryExtensions
{
    public static ServiceRegistry AddEndpointModules(this ServiceRegistry services, Assembly? assembly = null)
    {
        services.Scan(scan =>
        {
            if(assembly != null)
                scan.Assembly(assembly);
            else
                scan.AssembliesFromApplicationBaseDirectory();
            
            scan.AddAllTypesOf<IEndpointModule>();
            scan.RegisterConcreteTypesAgainstTheFirstInterface();
        });
            
        return services;
    }
}