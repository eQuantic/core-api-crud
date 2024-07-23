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
    
    public EntityCrudOptions<TEntity> For<TEntity>() => new(this);
    public EntityCrudOptions<TEntity1, TEntity2> For<TEntity1, TEntity2>() => new(this);
    public EntityCrudOptions<TEntity1, TEntity2, TEntity3> For<TEntity1, TEntity2, TEntity3>() => new(this);
    public EntityCrudOptions<TEntity1, TEntity2, TEntity3, TEntity4> For<TEntity1, TEntity2, TEntity3, TEntity4>() => new(this);
    public EntityCrudOptions<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5> For<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>() => new(this);
    public EntityCrudOptions<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6> For<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>() => new(this);
    public EntityCrudOptions<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7> For<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>() => new(this);
    public EntityCrudOptions<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8> For<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8>() => new(this);
    public EntityCrudOptions<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9> For<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9>() => new(this);
    public EntityCrudOptions<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9, TEntity10> For<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9, TEntity10>() => new(this);

    internal Assembly? GetAssembly() => _assembly;
    internal Dictionary<Type, Action<ICrudOptions>> GetOptions() => _options;
    internal bool? GetRequireAuth() => _requireAuth;
    internal RouteFormat? GetRouteFormat() => _routeFormat;
    internal bool? GetValidation() => _withValidation;
    
    public class EntityCrudOptions<TEntity>(AllCrudOptions allCrudOptions)
    {
        public AllCrudOptions UseOptions(Action<ICrudOptions> options)
        {
            allCrudOptions._options.Add(typeof(TEntity), options);
            return allCrudOptions;
        }
    }
    
    public class EntityCrudOptions<TEntity1, TEntity2>(AllCrudOptions allCrudOptions)
    {
        public AllCrudOptions UseOptions(Action<ICrudOptions> options)
        {
            allCrudOptions._options.Add(typeof(TEntity1), options);
            allCrudOptions._options.Add(typeof(TEntity2), options);
            return allCrudOptions;
        }
    }
    
    public class EntityCrudOptions<TEntity1, TEntity2, TEntity3>(AllCrudOptions allCrudOptions)
    {
        public AllCrudOptions UseOptions(Action<ICrudOptions> options)
        {
            allCrudOptions._options.Add(typeof(TEntity1), options);
            allCrudOptions._options.Add(typeof(TEntity2), options);
            allCrudOptions._options.Add(typeof(TEntity3), options);
            return allCrudOptions;
        }
    }
    
    public class EntityCrudOptions<TEntity1, TEntity2, TEntity3, TEntity4>(AllCrudOptions allCrudOptions)
    {
        public AllCrudOptions UseOptions(Action<ICrudOptions> options)
        {
            allCrudOptions._options.Add(typeof(TEntity1), options);
            allCrudOptions._options.Add(typeof(TEntity2), options);
            allCrudOptions._options.Add(typeof(TEntity3), options);
            allCrudOptions._options.Add(typeof(TEntity4), options);
            return allCrudOptions;
        }
    }
    
    public class EntityCrudOptions<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(AllCrudOptions allCrudOptions)
    {
        public AllCrudOptions UseOptions(Action<ICrudOptions> options)
        {
            allCrudOptions._options.Add(typeof(TEntity1), options);
            allCrudOptions._options.Add(typeof(TEntity2), options);
            allCrudOptions._options.Add(typeof(TEntity3), options);
            allCrudOptions._options.Add(typeof(TEntity4), options);
            allCrudOptions._options.Add(typeof(TEntity5), options);
            return allCrudOptions;
        }
    }
    
    public class EntityCrudOptions<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(AllCrudOptions allCrudOptions)
    {
        public AllCrudOptions UseOptions(Action<ICrudOptions> options)
        {
            allCrudOptions._options.Add(typeof(TEntity1), options);
            allCrudOptions._options.Add(typeof(TEntity2), options);
            allCrudOptions._options.Add(typeof(TEntity3), options);
            allCrudOptions._options.Add(typeof(TEntity4), options);
            allCrudOptions._options.Add(typeof(TEntity5), options);
            allCrudOptions._options.Add(typeof(TEntity6), options);
            return allCrudOptions;
        }
    }
    
    public class EntityCrudOptions<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(AllCrudOptions allCrudOptions)
    {
        public AllCrudOptions UseOptions(Action<ICrudOptions> options)
        {
            allCrudOptions._options.Add(typeof(TEntity1), options);
            allCrudOptions._options.Add(typeof(TEntity2), options);
            allCrudOptions._options.Add(typeof(TEntity3), options);
            allCrudOptions._options.Add(typeof(TEntity4), options);
            allCrudOptions._options.Add(typeof(TEntity5), options);
            allCrudOptions._options.Add(typeof(TEntity6), options);
            allCrudOptions._options.Add(typeof(TEntity7), options);
            return allCrudOptions;
        }
    }
    
    public class EntityCrudOptions<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8>(AllCrudOptions allCrudOptions)
    {
        public AllCrudOptions UseOptions(Action<ICrudOptions> options)
        {
            allCrudOptions._options.Add(typeof(TEntity1), options);
            allCrudOptions._options.Add(typeof(TEntity2), options);
            allCrudOptions._options.Add(typeof(TEntity3), options);
            allCrudOptions._options.Add(typeof(TEntity4), options);
            allCrudOptions._options.Add(typeof(TEntity5), options);
            allCrudOptions._options.Add(typeof(TEntity6), options);
            allCrudOptions._options.Add(typeof(TEntity7), options);
            allCrudOptions._options.Add(typeof(TEntity8), options);
            return allCrudOptions;
        }
    }
    
    public class EntityCrudOptions<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9>(AllCrudOptions allCrudOptions)
    {
        public AllCrudOptions UseOptions(Action<ICrudOptions> options)
        {
            allCrudOptions._options.Add(typeof(TEntity1), options);
            allCrudOptions._options.Add(typeof(TEntity2), options);
            allCrudOptions._options.Add(typeof(TEntity3), options);
            allCrudOptions._options.Add(typeof(TEntity4), options);
            allCrudOptions._options.Add(typeof(TEntity5), options);
            allCrudOptions._options.Add(typeof(TEntity6), options);
            allCrudOptions._options.Add(typeof(TEntity7), options);
            allCrudOptions._options.Add(typeof(TEntity8), options);
            allCrudOptions._options.Add(typeof(TEntity9), options);
            return allCrudOptions;
        }
    }
    
    public class EntityCrudOptions<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7, TEntity8, TEntity9, TEntity10>(AllCrudOptions allCrudOptions)
    {
        public AllCrudOptions UseOptions(Action<ICrudOptions> options)
        {
            allCrudOptions._options.Add(typeof(TEntity1), options);
            allCrudOptions._options.Add(typeof(TEntity2), options);
            allCrudOptions._options.Add(typeof(TEntity3), options);
            allCrudOptions._options.Add(typeof(TEntity4), options);
            allCrudOptions._options.Add(typeof(TEntity5), options);
            allCrudOptions._options.Add(typeof(TEntity6), options);
            allCrudOptions._options.Add(typeof(TEntity7), options);
            allCrudOptions._options.Add(typeof(TEntity8), options);
            allCrudOptions._options.Add(typeof(TEntity9), options);
            allCrudOptions._options.Add(typeof(TEntity10), options);
            return allCrudOptions;
        }
    }
}