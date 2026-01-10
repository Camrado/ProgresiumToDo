using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace ProgresiumToDo.API.ExceptionHandlers;

public class ValidationExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<ValidationExceptionHandler> _logger;

    public ValidationExceptionHandler(IProblemDetailsService problemDetailsService,
        ILogger<ValidationExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }
        
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        
        var validationErrors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());
        
        _logger.LogError(exception,
            "A validation exception occured. \n" +
            "Status Code: {StatusCode} \n" +
            "Request Method: {RequestMethod}\n" +
            "Request Path: {RequestPath}\n" +
            "Request Id: {RequestId}\n" +
            "Trace Id: {TraceId}\n" +
            "Error Message: {ErrorMessage}",
            httpContext.Response.StatusCode,
            httpContext.Request.Method,
            httpContext.Request.Path,
            httpContext.TraceIdentifier,
            httpContext.Features.Get<IHttpActivityFeature>()?.Activity.Id,
            exception.Message);
        
        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ValidationProblemDetails
            {
                Type = "https://httpstatuses.com/400",
                Title = $"{exception.GetType().Name}: Validation failed.",
                Detail = "One or more validation errors occurred.",
                Errors = validationErrors
            }
        });

        return true;
    }
}