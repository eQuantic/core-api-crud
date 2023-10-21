using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eQuantic.Core.Extensions;

namespace eQuantic.Core.Persistence.Extensions;

public static class EntityTypeBuilderExtensions
{
    /// <summary>
    /// Has the default primary key column name using the specified entity type builder
    /// </summary>
    /// <typeparam name="TEntity">The entity</typeparam>
    /// <typeparam name="TProperty">The property</typeparam>
    /// <param name="entityTypeBuilder">The entity type builder</param>
    /// <param name="expression">The expression</param>
    /// <returns>The entity type builder</returns>
    public static EntityTypeBuilder<TEntity> HasDefaultPrimaryKeyColumnName<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder, 
        Expression<Func<TEntity, TProperty>> expression) 
        where TEntity : class
    {
        entityTypeBuilder.Property(expression).HasColumnName($"{typeof(TEntity).Name.TrimEnd("Data")}Id");
        return entityTypeBuilder;
    }
    
    /// <summary>
    /// Has JSON data
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="resourceName"></param>
    /// <param name="assembly"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static EntityTypeBuilder<T> HasJsonData<T>(this EntityTypeBuilder<T> builder, string resourceName, Assembly assembly) 
        where T : class
    {
        var entities = ReadJsonResources<T>(resourceName, assembly);
        if(entities != null)
            builder.HasData(entities);
        return builder;
    }
    
    private static T[]? ReadJsonResources<T>(string resourceName, Assembly assembly)
    {
        var prefix = assembly.GetName().Name;
        using var stream = assembly.GetManifestResourceStream($"{prefix}.{resourceName}");
        if (stream == null) return default;
        using var reader = new StreamReader(stream);
        var jsonString = reader.ReadToEnd();
        if (string.IsNullOrEmpty(jsonString)) return default;
        return JsonSerializer.Deserialize<T[]>(jsonString, new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}
