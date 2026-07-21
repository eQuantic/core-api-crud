using eQuantic.Core.Api.Crud.Options;

namespace eQuantic.Core.Api.Crud.Controllers.Conventions;

/// <summary>
/// Describes a controller materialized by <see cref="Extensions.MvcBuilderExtensions.AddCrudControllers(Microsoft.Extensions.DependencyInjection.IMvcBuilder, System.Action{AllCrudOptions}?)"/>,
/// carrying everything the <see cref="CrudControllerModelConvention"/> needs to route and name it.
/// </summary>
internal sealed class CrudControllerDescriptor
{
    /// <summary>The closed generic controller type added to the controller feature.</summary>
    public required Type ControllerType { get; init; }

    /// <summary>The entity type the controller serves.</summary>
    public required Type EntityType { get; init; }

    /// <summary>The entity key type (primitive or complex).</summary>
    public required Type KeyType { get; init; }

    /// <summary>The resolved per-entity options (route format, group, verbs, references, auth).</summary>
    public required ICrudOptions Options { get; init; }
}
