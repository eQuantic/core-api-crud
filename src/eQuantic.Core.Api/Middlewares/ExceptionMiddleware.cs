using System.Net.Mime;
using eQuantic.Core.Api.Error.Results;
using eQuantic.Core.Api.Options;
using eQuantic.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace eQuantic.Core.Api.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ExceptionFilterOptions _options;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<ExceptionMiddleware> _logger;
    
    public ExceptionMiddleware(ExceptionFilterOptions options, ILoggerFactory loggerFactory, IHostEnvironment environment)
    {
        _options = options;
        _environment = environment;
        _logger = loggerFactory.CreateLogger<ExceptionMiddleware>();
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try 
        { 
            await next(context);
        } 
        catch (Exception ex) 
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = exception switch
        {
            EntityNotFoundException => StatusCodes.Status404NotFound,
            DependencyNotFoundException => StatusCodes.Status404NotFound,
            InvalidEntityReferenceException => StatusCodes.Status400BadRequest,
            InvalidEntityRequestException => StatusCodes.Status400BadRequest,
            HttpRequestException ex => (int?)ex.StatusCode ?? StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        var exceptionType = exception.GetType();
        var message = exception.Message;
        var options = _options.GetOptions();
        if (options.Any())
        {
            foreach (var (type, opt) in options)
            {
                if(!(exceptionType == type || type.IsSubclassOf(exceptionType)))
                    continue;

                context.Response.StatusCode = opt.HttpStatusCode;
                if (!string.IsNullOrEmpty(opt.Message))
                    message = opt.Message;
            }
        }

        var response = new ErrorResult(message, _environment.IsDevelopment() ? exception.StackTrace : null);

        if(context.Response.StatusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(0, exception, message);
        
        await context.Response.WriteAsJsonAsync(response);
    }
}