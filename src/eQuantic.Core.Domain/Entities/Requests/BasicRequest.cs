using System.Reflection;

namespace eQuantic.Core.Domain.Entities.Requests;

public class BasicRequest
{
    internal bool IsReferencedRequest()
    {
        return GetType().GetInterfaces().Any(o => o.GetGenericTypeDefinition() == typeof(IReferencedRequest<>));
    }

    internal object? GetReferenceValue()
    {
        return GetReferenceProperty()?.GetValue(this);
    }

    internal Type? GetReferenceType()
    {
        return GetReferenceProperty()?.PropertyType;
    }

    private PropertyInfo? GetReferenceProperty()
    {
        return IsReferencedRequest() ? GetType().GetProperty("ReferenceId") : null;
    }
    
    internal static bool IsReferencedRequest(BasicRequest request)
    {
        return request.GetType().GetInterfaces().Any(o => o.GetGenericTypeDefinition() == typeof(IReferencedRequest<>));
    }

    internal static object? GetReferenceValue(BasicRequest request)
    {
        return request.GetReferenceProperty()?.GetValue(request);
    }

    internal static Type? GetReferenceType(BasicRequest request)
    {
        return request.GetReferenceProperty()?.PropertyType;
    }
}