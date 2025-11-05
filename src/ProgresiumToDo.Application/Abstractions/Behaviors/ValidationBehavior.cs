using FluentValidation;
using MediatR;

namespace ProgresiumToDo.Application.Abstractions.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }
        
        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var validationFailures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .Distinct()
            .ToList();

        if (validationFailures.Any())
        {
            throw new ValidationException(validationFailures);
        }
        
        return await next(cancellationToken);
    }
}