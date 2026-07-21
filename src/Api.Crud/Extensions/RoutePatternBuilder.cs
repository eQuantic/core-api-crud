using eQuantic.Core.Api.Crud.Options;
using Humanizer;

namespace eQuantic.Core.Api.Crud.Extensions;

/// <summary>
/// Builds the route patterns shared by the Minimal API endpoints and the MVC controllers,
/// so both expose identical routes (pluralized + case formatted names, key constraints,
/// complex keys split into segments and referenced parent prefixes).
/// </summary>
internal static class RoutePatternBuilder
{
    /// <summary>
    /// Build the route pattern for an entity.
    /// </summary>
    public static string GetPattern<TEntity, TKey>(
        RouteFormat format,
        bool withId = false,
        EndpointReferenceOptions? reference = null)
        where TKey : notnull
        => GetPattern(typeof(TEntity), typeof(TKey), format, withId, reference);

    /// <summary>
    /// Build the route pattern for an entity using runtime types (used by the controller convention).
    /// </summary>
    public static string GetPattern(
        Type entityType,
        Type keyType,
        RouteFormat format,
        bool withId = false,
        EndpointReferenceOptions? reference = null)
    {
        var entityName = entityType.GetEntityName();
        var prefix = entityName.ChangeCase(format);
        var pattern = $"/{prefix}";
        if (reference != null)
        {
            pattern = $"/{reference.EntityType.GetEntityName().ChangeCase(format)}/{{{reference.Name}}}{pattern}";
        }

        if (!withId)
            return pattern;

        pattern = $"{pattern}/{GetIdSegment(keyType)}";

        return pattern;
    }

    /// <summary>
    /// Build the identifier route segment(s) without the entity prefix.
    /// Primitive keys become a single constrained segment (e.g. <c>{id:int}</c>); complex keys are
    /// split into one segment per property (e.g. <c>{code}/{location}</c>).
    /// </summary>
    public static string GetIdSegment(Type keyType)
        => IsPrimitiveKey(keyType)
            ? $"{{id{GetRouteConstraint(keyType)}}}"
            : string.Join("/", GetRoutesFromComplexKey(keyType).Select(r => $"{{{r}}}"));

    public static string GetRouteConstraint<TKey>() where TKey : notnull => GetRouteConstraint(typeof(TKey));

    public static string GetRouteConstraint(Type keyType)
    {
        var typeDict = new Dictionary<Type, string>
        {
            { typeof(int), ":int" },
            { typeof(Guid), ":guid" },
        };

        return typeDict.TryGetValue(keyType, out var routeConstraint)
            ? routeConstraint
            : string.Empty;
    }

    public static string ChangeCase(this string name, RouteFormat format)
    {
        var route = name.Pluralize();
        return format switch
        {
            RouteFormat.CamelCase => route.Camelize(),
            RouteFormat.PascalCase => route.Pascalize(),
            RouteFormat.SnakeCase => route.Kebaberize(),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    public static bool IsPrimitiveKey<TKey>() where TKey : notnull => IsPrimitiveKey(typeof(TKey));

    public static bool IsPrimitiveKey(Type keyType)
        => keyType == typeof(string) || keyType == typeof(Guid) || keyType.IsPrimitive;

    public static string[] GetRoutesFromComplexKey<TKey>() where TKey : notnull
        => GetRoutesFromComplexKey(typeof(TKey));

    public static string[] GetRoutesFromComplexKey(Type keyType)
        => keyType.GetProperties().Select(o => o.Name.Camelize()!).ToArray();
}
