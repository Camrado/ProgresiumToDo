using FluentValidation;

namespace ProgresiumToDo.Application.Auth.Commands.LogOutUser;

internal sealed class LogOutUserCommandValidator : AbstractValidator<LogOutUserCommand>
{
    public LogOutUserCommandValidator()
    {
        RuleFor(suc => suc.RefreshToken).NotEmpty();
    }
}