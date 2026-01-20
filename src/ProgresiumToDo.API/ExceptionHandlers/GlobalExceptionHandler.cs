using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace ProgresiumToDo.API.ExceptionHandlers;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    
    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        IWebHostEnvironment webHostEnvironment,
        ILogger<GlobalExceptionHandler> logger)
    {
        _webHostEnvironment = webHostEnvironment;
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        
        _logger.LogError(exception,
            "A global exception occured. \n" +
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
            ProblemDetails = new ProblemDetails
            {
                Type = "https://httpstatuses.com/500",
                Title = $"{exception.GetType().Name}: An error occurred while processing your request.",
                Status = StatusCodes.Status500InternalServerError,
                Detail = _webHostEnvironment.IsProduction()
                    ? "An unexpected error occurred. Please try again later."
                    : exception.Message
            }
        });

        return true;
    }
}