using eQuantic.Core.Api.Middlewares;
using eQuantic.Core.Api.Options;
using Microsoft.AspNetCore.Builder;

namespace eQuantic.Core.Api.Extensions;

/// <summary>
/// The wep application extensions class
/// </summary>
public static class WepApplicationExtensions
{
    /// <summary>
    /// Use the api documentation
    /// </summary>
    /// <param name="app">The app</param>
    /// <param name="options">The documentation options</param>
    /// <returns>The app</returns>
    public static WebApplication UseApiDocumentation(this WebApplication app, Action<DocumentationOptions>? options = null)
    {
        var docOptions = new DocumentationOptions();
        options?.Invoke(docOptions);
        
        app.UseSwagger();
        app.UseSwaggerUI(c =>
            c.SwaggerEndpoint("/swagger/v1/swagger.json", docOptions.Title));
        return app;
    }

    /// <summary>
    /// Use the exception filter
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseExceptionFilter(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        return app;
    }
}