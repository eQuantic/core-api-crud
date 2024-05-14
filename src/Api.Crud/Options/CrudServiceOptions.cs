using System.Reflection;

namespace eQuantic.Core.Api.Crud.Options;

public class CrudServiceOptions
{
    private Assembly? _assembly = null;
    
    public CrudServiceOptions FromAssembly(Assembly assembly)
    {
        _assembly = assembly;
        return this;
    }
    
    internal Assembly? GetAssembly() => _assembly;
}