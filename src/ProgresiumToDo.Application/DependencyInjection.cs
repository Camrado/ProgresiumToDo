using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProgresiumToDo.Application.Abstractions.Behaviors;

namespace ProgresiumToDo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
        
        return services;
    }
}