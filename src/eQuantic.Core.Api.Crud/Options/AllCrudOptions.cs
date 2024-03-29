using System.Reflection;

namespace eQuantic.Core.Api.Crud.Options;

public class AllCrudOptions
{
    private Assembly? _assembly = null;
    private readonly Dictionary<Type, Action<ICrudOptions>> _options = new();
    private bool? _requireAuth = null;
    private bool? _withValidation = null;
    private RouteFormat? _routeFormat;
    
    public AllCrudOptions FromAssembly(Assembly assembly)
    {
        _assembly = assembly;
        return this;
    }
    
    public AllCrudOptions AllRequireAuthorization()
    {
        _requireAuth = true;
        return this;
    }

    public AllCrudOptions WithRouteFormat(RouteFormat format)
    {
        _routeFormat = format;
        return this;
    }
    
    public AllCrudOptions WithValidation(bool withValidation = true)
    {
        _withValidation = withValidation;
        return this;
    }
    
    public EntityCrudOptions<TEntity> For<TEntity>()
    {
        return new EntityCrudOptions<TEntity>(this);
    }

    internal Assembly? GetAssembly() => _assembly;
    internal Dictionary<Type, Action<ICrudOptions>> GetOptions() => _options;
    internal bool? GetRequireAuth() => _requireAuth;
    internal RouteFormat? GetRouteFormat() => _routeFormat;
    internal bool? GetValidation() => _withValidation;
    
    public class EntityCrudOptions<TEntity>
    {
        private readonly AllCrudOptions _allCrudOptions;

        public EntityCrudOptions(AllCrudOptions allCrudOptions)
        {
            _allCrudOptions = allCrudOptions;
        }

        public AllCrudOptions UseOptions(Action<ICrudOptions> options)
        {
            _allCrudOptions._options.Add(typeof(TEntity), options);
            return _allCrudOptions;
        }
    }
}