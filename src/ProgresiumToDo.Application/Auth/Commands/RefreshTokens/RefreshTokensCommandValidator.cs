using FluentValidation;

namespace ProgresiumToDo.Application.Auth.Commands.RefreshTokens;

public class RefreshTokensCommandValidator : AbstractValidator<RefreshTokensCommand>
{
    public RefreshTokensCommandValidator()
    {
        RuleFor(x => x.OldRefreshToken).NotEmpty();
    }
}