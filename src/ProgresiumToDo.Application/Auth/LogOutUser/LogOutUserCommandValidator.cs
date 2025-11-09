using FluentValidation;

namespace ProgresiumToDo.Application.Auth.LogOutUser;

internal sealed class LogOutUserCommandValidator : AbstractValidator<LogOutUserCommand>
{
    public LogOutUserCommandValidator()
    {
        RuleFor(suc => suc.RefreshToken).NotEmpty();
    }
}