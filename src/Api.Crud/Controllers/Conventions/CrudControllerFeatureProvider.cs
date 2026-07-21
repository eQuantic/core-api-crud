using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace eQuantic.Core.Api.Crud.Controllers.Conventions;

/// <summary>
/// Registers the closed generic CRUD controller types with MVC. The default feature provider ignores
/// generic type definitions, so closed generics must be added explicitly here.
/// </summary>
internal sealed class CrudControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    private readonly IReadOnlyCollection<Type> _controllerTypes;

    public CrudControllerFeatureProvider(IEnumerable<Type> controllerTypes)
    {
        _controllerTypes = controllerTypes.ToArray();
    }

    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        foreach (var type in _controllerTypes)
        {
            var typeInfo = type.GetTypeInfo();
            if (!feature.Controllers.Contains(typeInfo))
            {
                feature.Controllers.Add(typeInfo);
            }
        }
    }
}
