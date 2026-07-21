using eQuantic.Core.Api.Crud.Extensions;
using Humanizer;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace eQuantic.Core.Api.Crud.Binders;

/// <summary>
/// Binds the route value(s) into the entity key for MVC controllers.
/// Primitive keys (int/Guid/string/...) are read from the <c>id</c> route value; complex keys are
/// rebuilt from one route value per property (e.g. <c>{code}/{location}</c>).
/// </summary>
public class KeyModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var keyType = bindingContext.ModelType;
        if (RoutePatternBuilder.IsPrimitiveKey(keyType))
        {
            var id = bindingContext.HttpContext.Request.RouteValues["id"];
            bindingContext.Result = ModelBindingResult.Success(ConvertValue(id, keyType));
            return Task.CompletedTask;
        }

        var properties = keyType.GetProperties();
        var values = new object?[properties.Length];
        for (var i = 0; i < properties.Length; i++)
        {
            var property = properties[i];
            var value = bindingContext.HttpContext.Request.RouteValues[property.Name.Camelize()!];
            values[i] = ConvertValue(value, property.PropertyType);
        }

        var key = Activator.CreateInstance(keyType, values);
        bindingContext.Result = ModelBindingResult.Success(key);
        return Task.CompletedTask;
    }

    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value == null)
            return null;

        if (targetType == typeof(string))
            return value.ToString();

        if (targetType == typeof(Guid))
            return Guid.Parse(value.ToString()!);

        return Convert.ChangeType(value, targetType);
    }
}
