using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.API.ExceptionHandlers;
using FluentValidation;

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
        
        // Customize the automatic 400 responses for model validation errors
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var hasJsonFormatError = context.ModelState.Values.Any(v => 
                    v.Errors.Any(e => 
                        e.Exception is JsonException || 
                        e.ErrorMessage.Contains("The JSON value could not be converted") ||
                        e.ErrorMessage.Contains("The input does not contain any JSON tokens")));

                Dictionary<string, string[]> errors;
 
                if (hasJsonFormatError)
                {
                    errors = new Dictionary<string, string[]>
                    {
                        { "general", ["The provided JSON body is of invalid format."] }
                    };
                }
                else
                {
                    errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => string.IsNullOrWhiteSpace(kvp.Key) 
                                ? "general" 
                                : JsonNamingPolicy.CamelCase.ConvertName(kvp.Key),
                            kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                }
                
                var problemDetails = new ValidationProblemDetails
                {
                    Type = "https://httpstatuses.com/400",
                    Title = $"{nameof(ValidationException)}: Validation Failed",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "One or more validation errors occurred.",
                    Errors = errors
                };
                
                problemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);

                return new BadRequestObjectResult(problemDetails);
            };
        });

        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<EntitlementExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}