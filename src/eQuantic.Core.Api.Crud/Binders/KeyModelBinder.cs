using Humanizer;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace eQuantic.Core.Api.Crud.Binders;

public class KeyModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var keyType = bindingContext.ModelType;
        if (keyType == typeof(string) || keyType == typeof(Guid) || keyType.IsPrimitive)
        {
            var id = bindingContext.HttpContext.Request.RouteValues["id"];
            var converted = Convert.ChangeType(id, keyType);
            bindingContext.Result = ModelBindingResult.Success(converted);
            return Task.CompletedTask;
        }

        var values = new List<object>();
        foreach(var prop in keyType.GetProperties().Select(o => o.Name.Camelize()!))
        {
            var value = bindingContext.HttpContext.Request.RouteValues[prop];
            values.Add(value!);
        }
        var key = Activator.CreateInstance(keyType, values.ToArray());
        bindingContext.Result = ModelBindingResult.Success(key);
        return Task.CompletedTask;
    }
}