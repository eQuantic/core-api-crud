using System.Reflection;
using eQuantic.Core.Domain.Attributes;
using Humanizer;
using Microsoft.OpenApi;

namespace eQuantic.Core.Api.Crud.Extensions;

internal static class TypeExtensions
{
    public static string GetEntityName(this MemberInfo entityType)
    {
        var entityAttr = entityType.GetCustomAttribute<EntityAttribute>();
        return entityAttr != null ? entityAttr.Name : entityType.Name;
    }
    
    public static string GetReferenceName(this Type type)
    {
        return $"{type.GetEntityName().Camelize()}Id";
    }
    
    public static OpenApiSchema ToSchema(this Type type)
    {
        var schema = new OpenApiSchema();
        if (type == typeof(DateTime))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "date-time";
            return schema;
        }
        if (type == typeof(DateOnly))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "date";
            return schema;
        }
        if (type == typeof(Guid))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "uuid";
            return schema;
        }
        if (type == typeof(short))
        {
            schema.Type = JsonSchemaType.Integer;
            return schema;
        }
        if (type == typeof(int))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Format = "int32";
            return schema;
        }
        if (type == typeof(long))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Format = "int64";
            return schema;
        }
        if (type == typeof(float))
        {
            schema.Type = JsonSchemaType.Number;
            schema.Format = "float";
            return schema;
        }
        if (type == typeof(double))
        {
            schema.Type = JsonSchemaType.Number;
            schema.Format = "double";
            return schema;
        }
        if (type == typeof(decimal))
        {
            schema.Type = JsonSchemaType.Number;
            return schema;
        }
        if (type == typeof(bool))
        {
            schema.Type = JsonSchemaType.Boolean;
            return schema;
        }

        schema.Type = JsonSchemaType.String;
        return schema;
    }
}