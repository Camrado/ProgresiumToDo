using FluentValidation;

namespace ProgresiumToDo.Application.Auth.RefreshTokens;

public class RefreshTokensCommandValidator : AbstractValidator<RefreshTokensCommand>
{
    public RefreshTokensCommandValidator()
    {
        RuleFor(x => x.OldRefreshToken).NotEmpty();
    }
}