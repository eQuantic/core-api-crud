using eQuantic.Core.Application.Crud.Enums;

namespace eQuantic.Core.Application.Crud.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class MapCrudEndpointsAttribute : Attribute
{
    public CrudEndpointVerbs EndpointVerbs { get; }
    public Type? ReferenceType { get; set; }
    public Type? ReferenceKeyType { get; set; }
    
    public MapCrudEndpointsAttribute(CrudEndpointVerbs endpointVerbs = CrudEndpointVerbs.All)
    {
        EndpointVerbs = endpointVerbs;
    }
}