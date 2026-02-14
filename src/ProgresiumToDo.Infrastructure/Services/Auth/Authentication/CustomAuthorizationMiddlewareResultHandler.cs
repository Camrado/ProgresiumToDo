using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;

namespace ProgresiumToDo.Infrastructure.Services.Auth.Authentication;

internal sealed class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
    
    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (!authorizeResult.Forbidden)
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        
        var emailRequirementFailed = authorizeResult.AuthorizationFailure?.FailedRequirements
            .OfType<ClaimsAuthorizationRequirement>()
            .Any(r => r.ClaimType == CustomClaims.EmailVerified);

        if (emailRequirementFailed == true)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/problem+json";
            
            var problemDetails = new ProblemDetails
            {
                Title = "Email Verification Required",
                Status = StatusCodes.Status403Forbidden,
                Detail = "You must verify your email address to access this resource.",
                Instance = context.Request.Path
            };
            
            await context.Response.WriteAsJsonAsync(problemDetails);
            return;
        }
    }
}