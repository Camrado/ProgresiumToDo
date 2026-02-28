using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;

namespace ProgresiumToDo.API.Extensions.RateLimiting;

public static class RateLimitingExtensions
{
    public const string ContactUsPolicyName = "ContactUsPolicy";
    public const string ResetPasswordPolicyName = "ResetPasswordPolicy";
    
    public static IServiceCollection AddCustomRateLimitings(this IServiceCollection services, IConfiguration configuration)
    {
        var contactUsSettings = configuration
            .GetSection("RateLimiting:ContactUsPolicy")
            .Get<RateLimitSettings>() ?? throw new InvalidOperationException("ContactUsPolicy settings are missing in configuration.");
        
        var resetPasswordSettings = configuration
            .GetSection("RateLimiting:ResetPasswordPolicy")
            .Get<RateLimitSettings>() ?? throw new InvalidOperationException("ResetPasswordPolicy settings are missing in configuration.");
        
        services.AddPolicyRateLimiting(ContactUsPolicyName, contactUsSettings.PermitLimit, TimeSpan.FromMinutes(contactUsSettings.WindowInMinutes));
        services.AddPolicyRateLimiting(ResetPasswordPolicyName, resetPasswordSettings.PermitLimit, TimeSpan.FromMinutes(resetPasswordSettings.WindowInMinutes));

        return services;
    }

    private static IServiceCollection AddPolicyRateLimiting(this IServiceCollection services, string policyName, int permitLimit, TimeSpan window)
    {
        services.AddRateLimiter(options =>
        {
            options.AddPolicy(policyName, context =>
            {
                var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(remoteIp, _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        Window = window,
                        QueueLimit = 0
                    });
            });

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/problem+json";

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString(CultureInfo.InvariantCulture);
                }

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status429TooManyRequests,
                    Title = "Too Many Requests",
                    Detail = $"You are sending requests too quickly. Please wait {retryAfter.Minutes.ToString()} minutes before trying again.",
                    Type = "https://tools.ietf.org/html/rfc6585#section-4"
                };

                await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, token);
            };
        });

        return services;
    }
}