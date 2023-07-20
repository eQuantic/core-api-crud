using eQuantic.Core.Application.Crud.Enums;
using Humanizer;

namespace eQuantic.Core.Api.Crud.Options;

public interface ICrudOptions
{
    CrudEndpointVerbs Verbs { get; }
    EndpointOptions Create { get; }
    EndpointOptions Update { get; }
    EndpointOptions Get { get; }
    EndpointOptions List { get; }
    EndpointOptions Delete { get; }
    
    ICrudOptions WithVerbs(CrudEndpointVerbs verbs);
    ICrudOptions WithReference(Type referenceType);
    ICrudOptions WithReference<TReferenceEntity>();
}
/// <summary>
/// CRUD options
/// </summary>
public class CrudOptions<TEntity> : ICrudOptions
{
    public ICrudOptions WithVerbs(CrudEndpointVerbs verbs)
    {
        Verbs = verbs;
        return this;
    }
    
    /// <summary>
    /// Options with referenced entity
    /// </summary>
    /// <param name="referenceType"></param>
    /// <returns></returns>
    public ICrudOptions WithReference(Type referenceType)
    {
        Get.WithReference(referenceType);
        List.WithReference(referenceType);
        Create.WithReference(referenceType);
        Update.WithReference(referenceType);
        Delete.WithReference(referenceType);
        return this;
    }
    
    /// <summary>
    /// Options with referenced entity
    /// </summary>
    /// <typeparam name="TReferenceEntity"></typeparam>
    /// <returns></returns>
    public ICrudOptions WithReference<TReferenceEntity>()
    {
        Get.WithReference<TReferenceEntity>();
        List.WithReference<TReferenceEntity>();
        Create.WithReference<TReferenceEntity>();
        Update.WithReference<TReferenceEntity>();
        Delete.WithReference<TReferenceEntity>();
        return this;
    }

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
    /// <returns></returns>
    public EndpointOptions WithReference(Type referenceType)
    {
        ReferenceType = referenceType;
        return this;
    }
    
    /// <summary>
    /// Options with reference
    /// </summary>
    /// <typeparam name="TReferenceEntity"></typeparam>
    /// <returns></returns>
    public EndpointOptions WithReference<TReferenceEntity>()
    {
        ReferenceType = typeof(TReferenceEntity);
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
    
    /// <summary>
    /// The reference entity type
    /// </summary>
    public Type? ReferenceType { get; private set; }

    /// <summary>
    /// The endpoint name
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    
    /// <summary>
    /// The endpoint description
    /// </summary>
    public string? Description { get; private set; }
    
    /// <summary>
    /// The endpoint summary
    /// </summary>
    public string? Summary { get; private set; }

    /// <summary>
    /// The endpoint tags
    /// </summary>
    public string[] Tags { get; private set; } = Array.Empty<string>();
}