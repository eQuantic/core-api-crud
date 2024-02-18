using System.Net.Mime;
using System.Security.Authentication;
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
        var exceptionResult = CreateExceptionResultFromOptions(exception) ?? CreateExceptionResult(exception);
        
        context.Response.StatusCode = exceptionResult.HttpStatusCode;
        
        var response = CreateErrorResult(exceptionResult, exception);

        if(context.Response.StatusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(0, exception, exceptionResult.Message);
        
        await context.Response.WriteAsJsonAsync(response);
    }

    private static ExceptionResult CreateExceptionResult(Exception exception)
    {
        return exception switch
        {
            AuthenticationException ex => new ExceptionResult(StatusCodes.Status401Unauthorized, ex.Message),
            AggregateException ex => CreateExceptionResultFromAggregate(ex),
            EntityNotFoundException ex => new ExceptionResult(StatusCodes.Status404NotFound, ex.Message),
            NoDataFoundException ex => new ExceptionResult(StatusCodes.Status404NotFound, ex.Message),
            DependencyNotFoundException ex => new ExceptionResult(StatusCodes.Status404NotFound, ex.Message),
            ForbiddenAccessException ex => new ExceptionResult(StatusCodes.Status403Forbidden, ex.Message),
            InvalidEntityReferenceException ex => new ExceptionResult(StatusCodes.Status400BadRequest, ex.Message),
            InvalidEntityRequestException ex => new ExceptionResult(StatusCodes.Status400BadRequest, ex.Message),
            HttpRequestException ex => new ExceptionResult((int?)ex.StatusCode ?? StatusCodes.Status500InternalServerError, ex.Message),
            _ => new ExceptionResult(StatusCodes.Status500InternalServerError, exception.Message)
        };
    }

    private static ExceptionResult CreateExceptionResultFromAggregate(AggregateException aggregateException)
    {
        var exception = aggregateException.GetBaseException();
        return exception is not AggregateException ? 
            CreateExceptionResult(exception) : 
            new ExceptionResult(StatusCodes.Status500InternalServerError, aggregateException.Message);
    }

    private ExceptionResult? CreateExceptionResultFromOptions(Exception exception)
    {
        var exceptionType = exception.GetType();
        var options = _options.GetOptions();
        if (!options.Any()) 
            return null;
        
        foreach (var (type, opt) in options)
        {
            if(!(exceptionType == type || type.IsSubclassOf(exceptionType)))
                continue;

            opt.SetResult(exception);
            return opt.GetResult();
        }

        return null;
    }
    
    private ErrorResult CreateErrorResult(ExceptionResult result, Exception exception)
    {
        var details = _environment.IsDevelopment() ? exception.StackTrace : null;
        
        return result.Errors?.Any() == true ? 
            new ValidationErrorResult(result.Message, details, result.Errors) : 
            new ErrorResult(result.Message, details);
    }
}