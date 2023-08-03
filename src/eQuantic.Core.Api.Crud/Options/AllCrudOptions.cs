using System.Reflection;

namespace eQuantic.Core.Api.Crud.Options;

public class AllCrudOptions
{
    private Assembly? _assembly = null;
    private readonly Dictionary<Type, Action<ICrudOptions>> _options = new();

    public AllCrudOptions FromAssembly(Assembly assembly)
    {
        _assembly = assembly;
        return this;
    }
    
    public EntityCrudOptions<TEntity> For<TEntity>()
    {
        return new EntityCrudOptions<TEntity>(this);
    }

    internal Assembly? GetAssembly() => _assembly;
    internal Dictionary<Type, Action<ICrudOptions>> GetOptions() => _options;
    
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