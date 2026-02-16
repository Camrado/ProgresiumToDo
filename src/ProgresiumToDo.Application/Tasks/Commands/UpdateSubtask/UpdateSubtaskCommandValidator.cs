using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Tasks.Repositories;
using ProgresiumToDo.Domain.Tasks;
using TaskStatus = ProgresiumToDo.Domain.Tasks.TaskStatus;

namespace ProgresiumToDo.Application.Tasks.Commands.UpdateSubtask;
 
internal sealed class UpdateSubtaskCommandValidator : AbstractValidator<UpdateSubtaskCommand>
{
    public UpdateSubtaskCommandValidator(ITaskItemRepository taskItemRepository, IUserContext userContext)
    {
        RuleFor(usc => usc)
            .NotNull()
            .MustAsync(async (command, cancellationToken) =>
            {
                var subtaskItem = await taskItemRepository.GetByIdAsync(command.SubtaskId,  userContext.UserId, trackChanges: true, cancellationToken);
                command.SubtaskItem = subtaskItem;

                if (subtaskItem is null)
                    return false;
                
                if (subtaskItem.ParentTaskItemId != command.ParentTaskId)
                    return false;
                
                return true;
            })
            .WithMessage("TaskItemWithOrder not found.");
        
        RuleFor(utc => utc)
            .Must(IsValidUpdateCombination)
            .WithMessage("Invalid update combination. Either update Status only, OrderIndex only, or regular fields only.");

        When(usc => IsStatusUpdated(usc) && !IsOrderUpdated(usc) && !IsRegularFieldUpdated(usc), () =>
        {
            RuleFor(ucs => ucs.Status)
                .Must(status => string.IsNullOrEmpty(status) || Enum.TryParse<TaskStatus>(status, ignoreCase: true, out _))
                .WithMessage("Invalid status. Valid values are: pending, inprogress, completed, cancelled.");
        });

        When(usc => IsOrderUpdated(usc) && !IsStatusUpdated(usc) && !IsRegularFieldUpdated(usc), () =>
        {
            RuleFor(utc => utc.OrderIndex)
                .GreaterThanOrEqualTo(0)
                .WithMessage("OrderIndex must be greater than or equal to 0.");
        });

        When(usc => IsRegularFieldUpdated(usc) && !IsStatusUpdated(usc) && !IsOrderUpdated(usc), () =>
        {
            RuleFor(usc => usc.Title)
                .MaximumLength(256)
                .WithMessage("Title must not exceed 256 characters.");
        
            RuleFor(ucs => ucs.Priority)
                .Must(priority => string.IsNullOrEmpty(priority) || Enum.TryParse<Priority>(priority, ignoreCase: true, out _))
                .WithMessage("Invalid priority. Valid values are: none, low, medium, high.");

            When(ucs => ucs.SubtaskItem is not null, () =>
            {
                // Validate StartTime and EndTime logic
                RuleFor(utc => utc)
                    .Must(command =>
                    {
                        if (command.EndTime.HasValue && !command.StartTime.HasValue && command.SubtaskItem!.StartTime.HasValue)
                            return command.EndTime > command.SubtaskItem!.StartTime;

                        if (command.StartTime.HasValue && !command.EndTime.HasValue && command.SubtaskItem!.EndTime.HasValue)
                            return command.StartTime < command.SubtaskItem!.EndTime;
                    
                        if (command.StartTime.HasValue && command.EndTime.HasValue)
                            return command.EndTime > command.StartTime;

                        return true;
                    }).WithMessage("EndTime must be later than StartTime.");
            });
        });
    }
    
    private static bool IsValidUpdateCombination(UpdateSubtaskCommand command)
    {
        var isStatusUpdate = IsStatusUpdated(command);
        var isOrderUpdate = IsOrderUpdated(command);

        if (isStatusUpdate)
        {
            return
                command.Title is null &&
                command.Description is null &&
                command.Priority is null &&
                command.StartTime is null &&
                command.EndTime is null &&
                command.OrderIndex is null;
        }

        if (isOrderUpdate)
        {
            return
                command.Title is null &&
                command.Description is null &&
                command.Priority is null &&
                command.StartTime is null &&
                command.EndTime is null &&
                string.IsNullOrEmpty(command.Status);
        }

        return true;
    }
    
    private static bool IsStatusUpdated(UpdateSubtaskCommand command)
    {
        return !string.IsNullOrEmpty(command.Status);
    }
    
    private static bool IsOrderUpdated(UpdateSubtaskCommand command)
    {
        return command.OrderIndex.HasValue;
    }
    
    private static bool IsRegularFieldUpdated(UpdateSubtaskCommand command)
    {
        return command.Title is not null ||
               command.Description is not null ||
               command.Priority is not null ||
               command.StartTime is not null ||
               command.EndTime is not null;
    }
}