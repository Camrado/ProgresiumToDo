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
                var parameters = context.ActionDescriptor.Parameters
                    .Select(p => p.Name)
                    .ToHashSet();

                var stateErrors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToList();
                
                var errors = stateErrors
                    .Where(e => stateErrors.Count == 1 || !parameters.Contains(e.Key))
                    .ToDictionary(
                        _ => "general",
                        kvp => kvp.Value!.Errors.Select(e => 
                        {
                            if (e.Exception is JsonException || 
                                e.ErrorMessage.Contains("The JSON value could not be converted"))
                            {
                                return "The provided JSON body is of invalid format.";
                            }
                            
                            return e.ErrorMessage;
                        }).ToArray()
                    );

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