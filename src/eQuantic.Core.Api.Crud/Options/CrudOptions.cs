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

    ICrudOptions RequireAuthorization();
    ICrudOptions WithRouteFormat(RouteFormat format);
    ICrudOptions WithGroup(string prefix);
    ICrudOptions WithParameter(OpenApiParameter parameter);
    ICrudOptions WithVerbs(CrudEndpointVerbs verbs);
    ICrudOptions WithReference(Type referenceType, Type referenceKeyType);
    ICrudOptions WithReference<TReferenceEntity, TReferenceKey>();
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
    
    public ICrudOptions WithParameter(OpenApiParameter parameter)
    {
        Get.WithParameter(parameter);
        List.WithParameter(parameter);
        Create.WithParameter(parameter);
        Update.WithParameter(parameter);
        Delete.WithParameter(parameter);
        return this;
    }

    public ICrudOptions WithVerbs(CrudEndpointVerbs verbs)
    {
        Verbs = verbs;
        return this;
    }

    public ICrudOptions RequireAuthorization()
    {
        Get.RequireAuthorization();
        List.RequireAuthorization();
        Create.RequireAuthorization();
        Update.RequireAuthorization();
        Delete.RequireAuthorization();
        return this;
    }

    /// <summary>
    /// Options with referenced entity
    /// </summary>
    /// <param name="referenceType"></param>
    /// <param name="referenceKeyType"></param>
    /// <returns></returns>
    public ICrudOptions WithReference(Type referenceType, Type referenceKeyType)
    {
        Get.WithReference(referenceType, referenceKeyType);
        List.WithReference(referenceType, referenceKeyType);
        Create.WithReference(referenceType, referenceKeyType);
        Update.WithReference(referenceType, referenceKeyType);
        Delete.WithReference(referenceType, referenceKeyType);
        return this;
    }

    /// <summary>
    /// Options with referenced entity
    /// </summary>
    /// <typeparam name="TReferenceEntity"></typeparam>
    /// <typeparam name="TReferenceKey"></typeparam>
    /// <returns></returns>
    public ICrudOptions WithReference<TReferenceEntity, TReferenceKey>()
    {
        Get.WithReference<TReferenceEntity, TReferenceKey>();
        List.WithReference<TReferenceEntity, TReferenceKey>();
        Create.WithReference<TReferenceEntity, TReferenceKey>();
        Update.WithReference<TReferenceEntity, TReferenceKey>();
        Delete.WithReference<TReferenceEntity, TReferenceKey>();
        return this;
    }

    public ICrudOptions WithFilter<TFilterType>()
    {
        Get.WithFilter<TFilterType>();
        List.WithFilter<TFilterType>();
        Create.WithFilter<TFilterType>();
        Update.WithFilter<TFilterType>();
        Delete.WithFilter<TFilterType>();
        return this;
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
    /// <returns></returns>
    public EndpointOptions WithReference(Type referenceType, Type referenceKeyType)
    {
        ReferenceType = referenceType;
        ReferenceKeyType = referenceKeyType;
        return this;
    }

    /// <summary>
    /// Options with reference
    /// </summary>
    /// <typeparam name="TReferenceEntity"></typeparam>
    /// <typeparam name="TReferenceKey"></typeparam>
    /// <returns></returns>
    public EndpointOptions WithReference<TReferenceEntity, TReferenceKey>()
    {
        ReferenceType = typeof(TReferenceEntity);
        ReferenceKeyType = typeof(TReferenceKey);
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

    public EndpointOptions RequireAuthorization()
    {
        RequireAuth = true;
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
    /// The reference entity type
    /// </summary>
    internal Type? ReferenceType { get; private set; }
    
    /// <summary>
    /// The reference key type
    /// </summary>
    internal Type? ReferenceKeyType { get; private set; }

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
    internal string[] Tags { get; private set; } = Array.Empty<string>();
    
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