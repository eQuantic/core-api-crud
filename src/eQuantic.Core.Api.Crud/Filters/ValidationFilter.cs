using eQuantic.Core.Api.Error.Results;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace eQuantic.Core.Api.Crud.Filters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var obj = context.Arguments.FirstOrDefault(x => x?.GetType() == typeof(T)) as T;

        if (obj is null)
        {
            return Results.BadRequest();
        }

        var validationResult = await _validator.ValidateAsync(obj);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new ValidationErrorResult($"Invalid request of {typeof(T).Name}", null, validationResult.ToDictionary()));
        }

        return await next(context);
    }
}