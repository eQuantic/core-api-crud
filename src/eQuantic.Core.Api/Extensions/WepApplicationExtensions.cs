using eQuantic.Core.Api.Options;
using Microsoft.AspNetCore.Builder;

namespace eQuantic.Core.Api.Extensions;

/// <summary>
/// The wep application extensions class
/// </summary>
public static class WepApplicationExtensions
{
    /// <summary>
    /// Uses the api documentation using the specified app
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
}