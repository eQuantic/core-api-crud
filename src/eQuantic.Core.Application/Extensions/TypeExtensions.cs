using System.Reflection;

namespace eQuantic.Core.Application.Extensions;

public static class TypeExtensions
{
    public static void InvokePrivateStaticMethod(this Type obj, string methodName, Type genericType, params object[] args)
    {
        obj.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)?
            .MakeGenericMethod(genericType)
            .Invoke(null, args);
    }
}