using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Tasks.Repositories;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.Commands.CreateSubtask;

internal sealed class CreateSubtaskCommandValidator : AbstractValidator<CreateSubtaskCommand>
{
    public CreateSubtaskCommandValidator(ITaskItemRepository taskItemRepository, IUserContext userContext)
    {
        RuleFor(csc => csc.ParentTaskId)
            .NotEmpty()
            .WithMessage("ParentTaskId is required.")
            .MustAsync(async (command, parentTaskId, cancellationToken) =>
            {
                var parentTask = await taskItemRepository.GetByIdAsync(parentTaskId, userContext.UserId, cancellationToken: cancellationToken);
                return parentTask is not null;
            })
            .WithMessage("Parent task not found.");
        
        RuleFor(csc => csc.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(256)
            .WithMessage("Title must not exceed 256 characters.");;
        
        RuleFor(csc => csc.Priority)
            .Must(priority => string.IsNullOrEmpty(priority) || Enum.TryParse<Priority>(priority, ignoreCase: true, out _))
            .WithMessage("Invalid priority. Valid values are: none, low, medium, high.");
        
        RuleFor(csc => csc)
            .Must(command =>
            {
                if (command.EndTime.HasValue && !command.StartTime.HasValue)
                    return true;
                
                if (!command.EndTime.HasValue && command.StartTime.HasValue)
                    return true;

                if (command.StartTime.HasValue && command.EndTime.HasValue)
                    return command.EndTime.Value > command.StartTime.Value;

                return true;
            }).WithMessage("Invalid time configuration. If endTime is provided, startTime must also be provided and endTime must be after startTime.");
    }
}