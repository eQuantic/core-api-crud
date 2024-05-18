using eQuantic.Core.Api.Error.Results;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace eQuantic.Core.Api.Crud.Filters;

public class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var obj = context.Arguments.FirstOrDefault(x => x?.GetType() == typeof(T)) as T;

        if (obj is null)
        {
            return Results.BadRequest();
        }

        var validationResult = await validator.ValidateAsync(obj);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new ErrorResult($"Invalid request of {typeof(T).Name}", validationResult.ToDictionary()));
        }

        return await next(context);
    }
}