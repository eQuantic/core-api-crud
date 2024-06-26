using eQuantic.Core.Api.Crud.Options;
using Microsoft.AspNetCore.Http;

namespace eQuantic.Core.Api.Crud.Extensions;

internal static class HttpContextExtensions
{
    public static TReferenceKey? GetReference<TReferenceKey>(this HttpContext context, EndpointOptions options)
    {
        var referenceName = options.Reference?.Name ?? "referenceId";
        var type = typeof(TReferenceKey);
        if (!context.Request.RouteValues.TryGetValue(referenceName, out var value))
            return default;
        
        if (type == typeof(string))
        {
            return (TReferenceKey)(object)value!.ToString()!;
        }
        if (type == typeof(DateTime))
        {
            return (TReferenceKey)(object)Convert.ToDateTime(value);
        }
        if (type == typeof(DateOnly))
        {
            return (TReferenceKey)(object)Convert.ToDateTime(value);
        }
        if (type == typeof(Guid))
        {
            return (TReferenceKey)(object)Guid.Parse(value!.ToString()!);
        }
        if (type == typeof(short))
        {
            return (TReferenceKey)(object)Convert.ToInt16(value);
        }
        if (type == typeof(int))
        {
            return (TReferenceKey)(object)Convert.ToInt32(value);
        }
        if (type == typeof(long))
        {
            return (TReferenceKey)(object)Convert.ToInt64(value);
        }
        if (type == typeof(float))
        {
            return (TReferenceKey)(object)Convert.ToSingle(value);
        }
        if (type == typeof(double))
        {
            return (TReferenceKey)(object)Convert.ToDouble(value);
        }
        if (type == typeof(decimal))
        {
            return (TReferenceKey)(object)Convert.ToDecimal(value);
        }
        if (type == typeof(bool))
        {
            return (TReferenceKey)(object)Convert.ToBoolean(value);
        }

        throw new NotSupportedException($"Not supported entity reference type of {type.Name}");
    }
}