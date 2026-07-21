using eQuantic.Core.Api.Crud.Extensions;
using eQuantic.Core.Api.Crud.Options;
using eQuantic.Core.Application.Crud.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace eQuantic.Core.Api.Crud.Controllers.Conventions;

/// <summary>
/// Applies routing, naming, verb filtering and authorization to the generated CRUD controllers,
/// mirroring the route patterns and options used by the Minimal API endpoints.
/// </summary>
internal sealed class CrudControllerModelConvention : IApplicationModelConvention
{
    private readonly Dictionary<Type, CrudControllerDescriptor> _descriptors;

    public CrudControllerModelConvention(IEnumerable<CrudControllerDescriptor> descriptors)
    {
        _descriptors = descriptors.ToDictionary(d => d.ControllerType);
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            if (_descriptors.TryGetValue(controller.ControllerType.AsType(), out var descriptor))
            {
                ApplyController(controller, descriptor);
            }
        }
    }

    private static void ApplyController(ControllerModel controller, CrudControllerDescriptor descriptor)
    {
        // Unique, clean controller name (closed generics otherwise share the same `Name`).
        controller.ControllerName = descriptor.EntityType.Name;

        var actionsToRemove = new List<ActionModel>();
        foreach (var action in controller.Actions)
        {
            if (!TryResolveEndpoint(action.ActionName, descriptor.Options, out var endpoint, out var verb, out var withId))
            {
                continue;
            }

            if ((descriptor.Options.Verbs & verb) != verb)
            {
                actionsToRemove.Add(action);
                continue;
            }

            ApplyAction(action, descriptor, endpoint, withId);
        }

        foreach (var action in actionsToRemove)
        {
            controller.Actions.Remove(action);
        }
    }

    private static void ApplyAction(ActionModel action, CrudControllerDescriptor descriptor, EndpointOptions endpoint, bool withId)
    {
        var pattern = RoutePatternBuilder.GetPattern(
            descriptor.EntityType,
            descriptor.KeyType,
            descriptor.Options.RouteFormat,
            withId,
            endpoint.Reference);

        var template = pattern.TrimStart('/');
        if (!string.IsNullOrEmpty(descriptor.Options.Prefix))
        {
            template = $"{descriptor.Options.Prefix!.Trim('/')}/{template}";
        }

        foreach (var selector in action.Selectors)
        {
            // Route name is intentionally left unset: the Minimal API endpoints use the same
            // Get{Entity}/Create{Entity}/... names, and endpoint names must be globally unique.
            // Link generation (e.g. the created location) resolves by action + controller name.
            selector.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(template));
        }

        if (endpoint.RequireAuth == true)
        {
            action.Filters.Add(new AuthorizeFilter());
        }
    }

    private static bool TryResolveEndpoint(
        string actionName,
        ICrudOptions options,
        out EndpointOptions endpoint,
        out CrudEndpointVerbs verb,
        out bool withId)
    {
        // Action names match the public methods on the controller base classes.
        switch (actionName)
        {
            case "GetById":
                endpoint = options.Get;
                verb = CrudEndpointVerbs.OnlyGetById;
                withId = true;
                return true;
            case "GetPagedList":
                endpoint = options.List;
                verb = CrudEndpointVerbs.OnlyGetPaged;
                withId = false;
                return true;
            case "Create":
                endpoint = options.Create;
                verb = CrudEndpointVerbs.OnlyCreate;
                withId = false;
                return true;
            case "Update":
                endpoint = options.Update;
                verb = CrudEndpointVerbs.OnlyUpdate;
                withId = true;
                return true;
            case "Delete":
                endpoint = options.Delete;
                verb = CrudEndpointVerbs.OnlyDelete;
                withId = true;
                return true;
            default:
                endpoint = null!;
                verb = default;
                withId = false;
                return false;
        }
    }
}
