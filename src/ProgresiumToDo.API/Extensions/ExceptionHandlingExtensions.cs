using ProgresiumToDo.API.ExceptionHandlers;

namespace ProgresiumToDo.API.Extensions;

public static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });

        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<EntitlementExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}