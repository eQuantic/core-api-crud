using eQuantic.Core.Api.Crud.Extensions;
using eQuantic.Core.Application.Crud.Enums;
using Humanizer;
using Microsoft.OpenApi.Models;

namespace eQuantic.Core.Api.Crud.Options;

public interface ICrudOptions
{
    RouteFormat RouteFormat { get; }
    string? Prefix { get; }
    CrudEndpointVerbs Verbs { get; }
    EndpointOptions Create { get; }
    EndpointOptions Update { get; }
    EndpointOptions Get { get; }
    EndpointOptions List { get; }
    EndpointOptions Delete { get; }

    ICrudOptions RequireAuthorization(CrudEndpointVerbs verbs, bool required = true);
    ICrudOptions RequireAuthorization(bool required = true);
    ICrudOptions WithRouteFormat(RouteFormat format);
    ICrudOptions WithGroup(string prefix);
    ICrudOptions WithParameter(CrudEndpointVerbs verbs, OpenApiParameter parameter);
    ICrudOptions WithParameter(OpenApiParameter parameter);
    ICrudOptions WithVerbs(CrudEndpointVerbs verbs);
    ICrudOptions WithReference(CrudEndpointVerbs verbs, Type referenceType, Type referenceKeyType, string? referenceName = null);
    ICrudOptions WithReference(Type referenceType, Type referenceKeyType, string? referenceName = null);
    ICrudOptions WithReference<TReferenceEntity, TReferenceKey>(CrudEndpointVerbs verbs, string? referenceName = null);
    ICrudOptions WithReference<TReferenceEntity, TReferenceKey>(string? referenceName = null);
    ICrudOptions WithFilter<TFilterType>(CrudEndpointVerbs verbs);
    ICrudOptions WithFilter<TFilterType>();
    ICrudOptions WithValidation(bool? withValidation = true);
}
/// <summary>
/// CRUD options
/// </summary>
public class CrudOptions<TEntity> : ICrudOptions
{
    public ICrudOptions WithRouteFormat(RouteFormat format)
    {
        RouteFormat = format;
        return this;
    }
    
    public ICrudOptions WithGroup(string prefix)
    {
        Prefix = prefix;
        return this;
    }
    
    public ICrudOptions WithParameter(CrudEndpointVerbs verbs, OpenApiParameter parameter)
    {
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyGetById))
            Get.WithParameter(parameter);
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyGetPaged))
            List.WithParameter(parameter);
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyCreate))
            Create.WithParameter(parameter);
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyUpdate))
            Update.WithParameter(parameter);
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyDelete))
            Delete.WithParameter(parameter);
        return this;
    }
    
    public ICrudOptions WithParameter(OpenApiParameter parameter)
    {
        return WithParameter(CrudEndpointVerbs.All, parameter);
    }

    public ICrudOptions WithVerbs(CrudEndpointVerbs verbs)
    {
        Verbs = verbs;
        return this;
    }

    public ICrudOptions RequireAuthorization(CrudEndpointVerbs verbs, bool required = true)
    {
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyGetById))
            Get.RequireAuthorization(required);
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyGetPaged))
            List.RequireAuthorization(required);
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyCreate))
            Create.RequireAuthorization(required);
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyUpdate))
            Update.RequireAuthorization(required);
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyDelete))
            Delete.RequireAuthorization(required);
        
        return this;
    }
    
    public ICrudOptions RequireAuthorization(bool required = true)
    {
        return RequireAuthorization(CrudEndpointVerbs.All, required);
    }

    /// <summary>
    /// Options with referenced entity
    /// </summary>
    /// <param name="verbs">CRUD endpoint verbs</param>
    /// <param name="referenceType"></param>
    /// <param name="referenceKeyType"></param>
    /// <param name="referenceName"></param>
    /// <returns></returns>
    public ICrudOptions WithReference(CrudEndpointVerbs verbs, Type referenceType, Type referenceKeyType, string? referenceName = null)
    {
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyGetById))
            Get.WithReference(referenceType, referenceKeyType, referenceName);
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyGetPaged))
            List.WithReference(referenceType, referenceKeyType, referenceName);
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyCreate))
            Create.WithReference(referenceType, referenceKeyType, referenceName);
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyUpdate))
            Update.WithReference(referenceType, referenceKeyType, referenceName);
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyDelete))
            Delete.WithReference(referenceType, referenceKeyType, referenceName);
        return this;
    }

    /// <summary>
    /// Options with referenced entity
    /// </summary>
    /// <param name="referenceType"></param>
    /// <param name="referenceKeyType"></param>
    /// <param name="referenceName"></param>
    /// <returns></returns>
    public ICrudOptions WithReference(Type referenceType, Type referenceKeyType, string? referenceName = null)
    {
        return WithReference(CrudEndpointVerbs.All, referenceType, referenceKeyType, referenceName);
    }

    /// <summary>
    /// Options with referenced entity
    /// </summary>
    /// <typeparam name="TReferenceEntity"></typeparam>
    /// <typeparam name="TReferenceKey"></typeparam>
    /// <param name="verbs"></param>
    /// <param name="referenceName"></param>
    /// <returns></returns>
    public ICrudOptions WithReference<TReferenceEntity, TReferenceKey>(CrudEndpointVerbs verbs, string? referenceName = null)
    {
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyGetById))
            Get.WithReference<TReferenceEntity, TReferenceKey>(referenceName);
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyGetPaged))
            List.WithReference<TReferenceEntity, TReferenceKey>(referenceName);
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyCreate))
            Create.WithReference<TReferenceEntity, TReferenceKey>(referenceName);
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyUpdate))
            Update.WithReference<TReferenceEntity, TReferenceKey>(referenceName);
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyDelete))
            Delete.WithReference<TReferenceEntity, TReferenceKey>(referenceName);
        
        return this;
    }
    
    /// <summary>
    /// Options with referenced entity
    /// </summary>
    /// <typeparam name="TReferenceEntity"></typeparam>
    /// <typeparam name="TReferenceKey"></typeparam>
    /// <param name="referenceName"></param>
    /// <returns></returns>
    public ICrudOptions WithReference<TReferenceEntity, TReferenceKey>(string? referenceName = null)
    {
        return WithReference<TReferenceEntity, TReferenceKey>(CrudEndpointVerbs.All, referenceName);
    }
    
    /// <summary>
    /// Options with filter
    /// </summary>
    /// <param name="verbs"></param>
    /// <typeparam name="TFilterType"></typeparam>
    /// <returns></returns>
    public ICrudOptions WithFilter<TFilterType>(CrudEndpointVerbs verbs)
    {
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyGetById))
            Get.WithFilter<TFilterType>();
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyGetPaged))
            List.WithFilter<TFilterType>();
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyCreate))
            Create.WithFilter<TFilterType>();
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyUpdate))
            Update.WithFilter<TFilterType>();
        
        if(verbs.HasFlag(CrudEndpointVerbs.OnlyDelete))
            Delete.WithFilter<TFilterType>();
        return this;
    }

    /// <summary>
    /// Options with filter
    /// </summary>
    /// <typeparam name="TFilterType"></typeparam>
    /// <returns></returns>
    public ICrudOptions WithFilter<TFilterType>()
    {
        return WithFilter<TFilterType>(CrudEndpointVerbs.All);
    }

    public ICrudOptions WithValidation(bool? withValidation = true)
    {
        Create.WithValidation(withValidation);
        Update.WithValidation(withValidation);
        return this;
    }
    
    public RouteFormat RouteFormat { get; private set; } = RouteFormat.CamelCase;
    public string? Prefix { get; private set; }
    public CrudEndpointVerbs Verbs { get; private set; } = CrudEndpointVerbs.All;

    /// <summary>
    /// The create endpoint options
    /// </summary>
    public EndpointOptions Create { get; } = new EndpointOptions()
        .WithName($"Create{typeof(TEntity).Name}")
        .WithTags(typeof(TEntity).Name.Pluralize());

    /// <summary>
    /// The update endpoint options
    /// </summary>
    public EndpointOptions Update { get; } = new EndpointOptions()
        .WithName($"Update{typeof(TEntity).Name}")
        .WithTags(typeof(TEntity).Name.Pluralize());

    /// <summary>
    /// The get endpoint options
    /// </summary>
    public EndpointOptions Get { get; } = new EndpointOptions()
        .WithName($"Get{typeof(TEntity).Name}")
        .WithTags(typeof(TEntity).Name.Pluralize());

    /// <summary>
    /// The list endpoint options
    /// </summary>
    public EndpointOptions List { get; } = new EndpointOptions()
        .WithName($"Get{typeof(TEntity).Name}PagedList")
        .WithTags(typeof(TEntity).Name.Pluralize());

    /// <summary>
    /// The delete endpoint options
    /// </summary>
    public EndpointOptions Delete { get; } = new EndpointOptions()
        .WithName($"Delete{typeof(TEntity).Name}")
        .WithTags(typeof(TEntity).Name.Pluralize());
}

/// <summary>
/// Endpoint options
/// </summary>
public class EndpointOptions
{
    /// <summary>
    /// Options with reference
    /// </summary>
    /// <param name="referenceType"></param>
    /// <param name="referenceKeyType"></param>
    /// <param name="referenceName"></param>
    /// <returns></returns>
    public EndpointOptions WithReference(Type referenceType, Type referenceKeyType, string? referenceName = null)
    {
        Reference = new EndpointReferenceOptions(referenceType, referenceKeyType, referenceName ?? referenceType.GetReferenceName());
        return this;
    }

    /// <summary>
    /// Options with reference
    /// </summary>
    /// <typeparam name="TReferenceEntity"></typeparam>
    /// <typeparam name="TReferenceKey"></typeparam>
    /// <returns></returns>
    public EndpointOptions WithReference<TReferenceEntity, TReferenceKey>(string? referenceName = null)
    {
        var referenceType = typeof(TReferenceEntity);
        var referenceKeyType = typeof(TReferenceKey);
        Reference = new EndpointReferenceOptions(referenceType, referenceKeyType, referenceName ?? referenceType.GetReferenceName());
        return this;
    }
    
    /// <summary>
    /// Options with name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public EndpointOptions WithName(string name)
    {
        Name = name;
        return this;
    }
    
    public EndpointOptions WithParameter(OpenApiParameter parameter)
    {
        Parameters.Add(parameter);
        return this;
    }
    
    /// <summary>
    /// Options with tags
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    public EndpointOptions WithTags(params string[] tags)
    {
        Tags = tags;
        return this;
    }
    
    /// <summary>
    /// Options with description
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    public EndpointOptions WithDescription(string description)
    {
        Description = description;
        return this;
    }
    
    /// <summary>
    /// Options with summary
    /// </summary>
    /// <param name="summary"></param>
    /// <returns></returns>
    public EndpointOptions WithSummary(string summary)
    {
        Summary = summary;
        return this;
    }

    public EndpointOptions RequireAuthorization(bool required = true)
    {
        RequireAuth = required;
        return this;
    }
    
    public EndpointOptions WithFilter<TFilterType>()
    {
        FilterType = typeof(TFilterType);
        return this;
    }

    public EndpointOptions WithValidation(bool? withValidation = true)
    {
        HasValidation = withValidation;
        return this;
    }
    
    /// <summary>
    /// The reference options
    /// </summary>
    internal EndpointReferenceOptions? Reference { get; private set; }

    /// <summary>
    /// The endpoint name
    /// </summary>
    internal string Name { get; private set; } = string.Empty;
    
    /// <summary>
    /// The endpoint description
    /// </summary>
    internal string? Description { get; private set; }
    
    /// <summary>
    /// The endpoint summary
    /// </summary>
    internal string? Summary { get; private set; }

    /// <summary>
    /// The endpoint tags
    /// </summary>
    internal string[] Tags { get; private set; } = [];
    
    /// <summary>
    /// The endpoint authorization
    /// </summary>
    internal bool? RequireAuth { get; private set; }
    
    /// <summary>
    /// The filter type
    /// </summary>
    internal Type? FilterType { get; private set; }
    
    
    internal bool? HasValidation { get; private set; }
    
    internal List<OpenApiParameter> Parameters { get; private set; } = new();
}

public record EndpointReferenceOptions(Type EntityType, Type KeyType, string? Name);