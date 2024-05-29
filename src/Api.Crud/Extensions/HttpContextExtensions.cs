using eQuantic.Core.Api.Crud.Options;
using Microsoft.AspNetCore.Http;

namespace eQuantic.Core.Api.Crud.Extensions;

internal static class HttpContextExtensions
{
    public static TReferenceKey GetReference<TReferenceKey>(this HttpContext context, EndpointOptions options)
    {
        var referenceName = options.ReferenceType?.GetReferenceName() ?? "referenceId";
        var referenceId = (TReferenceKey)Convert.ChangeType(context.Request.RouteValues[referenceName], typeof(TReferenceKey))!;
        return referenceId;
    }
}