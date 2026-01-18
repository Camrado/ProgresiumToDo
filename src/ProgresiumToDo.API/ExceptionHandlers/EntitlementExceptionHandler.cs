using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Abstractions.Auth.Entitlement;

public sealed class EntitlementExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<EntitlementExceptionHandler> _logger;
    
    public EntitlementExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<EntitlementExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, 
        CancellationToken cancellationToken)
    {
        if (exception is not EntitlementException entitlementException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

        var extensions = new Dictionary<string, object?>
        {
            { "entitlementErrors", entitlementException.Errors }
        };

        _logger.LogWarning(exception,
            "Entitlement check failed. \n" +
            "Status Code: {StatusCode} \n" +
            "Request Method: {RequestMethod}\n" +
            "Request Path: {RequestPath}\n" +
            "Request Id: {RequestId}\n" +
            "Trace Id: {TraceId}\n" +
            "Error Codes: {ErrorCodes}",
            httpContext.Response.StatusCode,
            httpContext.Request.Method,
            httpContext.Request.Path,
            httpContext.TraceIdentifier,
            httpContext.Features.Get<IHttpActivityFeature>()?.Activity.Id,
            string.Join(", ", entitlementException.Errors.Select(e => e.Code)));

        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Type = "https://httpstatuses.com/403",
                Title = "Entitlement Check Failed",
                Status = StatusCodes.Status403Forbidden,
                Detail = "You do not have permission to access these features or have exceeded your usage limits.",
                Extensions = extensions
            }
        });

        return true;
    }
}