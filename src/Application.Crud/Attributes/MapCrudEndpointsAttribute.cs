using eQuantic.Core.Application.Crud.Enums;

namespace eQuantic.Core.Application.Crud.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class MapCrudEndpointsAttribute(CrudEndpointVerbs endpointVerbs = CrudEndpointVerbs.All) : Attribute
{
    public CrudEndpointVerbs EndpointVerbs { get; } = endpointVerbs;
    public Type? ReferenceType { get; set; }
    public Type? ReferenceKeyType { get; set; }
    public string? ReferenceName { get; set; }
}