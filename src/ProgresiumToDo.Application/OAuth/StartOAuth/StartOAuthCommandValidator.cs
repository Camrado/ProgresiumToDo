using FluentValidation;

namespace ProgresiumToDo.Application.OAuth.StartOAuth;

internal sealed class StartOAuthCommandValidator : AbstractValidator<StartOAuthCommand>
{
    public StartOAuthCommandValidator()
    {
        RuleFor(soac => soac.Provider)
            .NotEmpty().WithMessage("Provider is required.")
            .Must(provider => string.Equals(provider, "google", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Provider must be one of the following: 'google'.");
    }
}