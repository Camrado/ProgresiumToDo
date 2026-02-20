using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Projects.Repositories;
using ProgresiumToDo.Application.Tasks.Repositories;
using ProgresiumToDo.Domain.Tasks;
using TaskStatus = ProgresiumToDo.Domain.Tasks.TaskStatus;

namespace ProgresiumToDo.Application.Tasks.Commands.UpdateTask;

internal sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator(ITaskItemRepository taskItemRepository, IUserContext userContext, IProjectRepository projectRepository)
    {
        RuleFor(utc => utc.TaskId)
            .NotNull()
            .MustAsync(async (command, taskId, cancellationToken) =>
            {
                TaskItem? taskItem;

                if (command.Tags is not null)
                    taskItem = await taskItemRepository.GetByIdWithTagsIncludedAsync(taskId, userContext.UserId, trackChanges: true, cancellationToken);
                else
                    taskItem = await taskItemRepository.GetByIdAsync(taskId, userContext.UserId, trackChanges: true, cancellationToken);
                
                command.TaskItem = taskItem;
                
                return taskItem != null;
            })
            .WithMessage("TaskItem not found.");
        
        RuleFor(utc => utc)
            .Must(IsValidUpdateCombination)
            .WithMessage("Invalid update combination. Either update Status only, OrderIndex with OrderType only, or regular fields only.");
        
        When(utc => IsStatusUpdated(utc) && !IsOrderUpdated(utc) && !IsRegularFieldUpdated(utc), () =>
        {
            RuleFor(utc => utc.Status)
                .Must(status => string.IsNullOrEmpty(status) || Enum.TryParse<TaskStatus>(status, ignoreCase: true, out _))
                .WithMessage("Invalid status. Valid values are: pending, inprogress, completed, cancelled.");
        });

        When(utc => IsOrderCorrectlyUpdated(utc) && !IsStatusUpdated(utc) && !IsRegularFieldUpdated(utc), () =>
        {
            RuleFor(utc => utc)
                .Must(command => command.NextTaskOrderIndex.HasValue || command.PreviousTaskOrderIndex.HasValue)
                .WithMessage("Either NextTaskId or PreviousTaskId must be provided when updating order.");

            RuleFor(utc => utc.OrderType)
                .Must(orderType => Enum.TryParse<OrderType>(orderType, ignoreCase: true, out _))
                .WithMessage("Invalid OrderType. Valid values are: ByDueDate, ByProject, ByParentTask.");
        });

        When(utc => IsRegularFieldUpdated(utc) && !IsOrderUpdated(utc) && !IsStatusUpdated(utc), () =>
        {
            RuleFor(utc => utc.Title)
                .MaximumLength(256)
                .WithMessage("Title must not exceed 256 characters.");
        
            RuleFor(utc => utc.Priority)
                .Must(priority => string.IsNullOrEmpty(priority) || Enum.TryParse<Priority>(priority, ignoreCase: true, out _))
                .WithMessage("Invalid priority. Valid values are: none, low, medium, high.");

            RuleFor(utc => utc.ProjectId)
                .MustAsync(async (command, projectId, cancellationToken) =>
                {
                    if (!projectId.HasValue)
                        return true;
                
                    var project = await projectRepository
                        .GetByIdAndUserIdAsync(projectId.Value, userContext.UserId, cancellationToken: cancellationToken);
                    return project != null;
                }).WithMessage("Project not found.");

            When(utc => utc.TaskItem is not null, () =>
            {
                // Validate StartTime and EndTime logic
                RuleFor(utc => utc)
                    .Must(command =>
                    {
                        if (command.EndTime.HasValue && !command.StartTime.HasValue && command.TaskItem!.StartTime.HasValue)
                            return command.EndTime > command.TaskItem!.StartTime;

                        if (command.StartTime.HasValue && !command.EndTime.HasValue && command.TaskItem!.EndTime.HasValue)
                            return command.StartTime < command.TaskItem!.EndTime;
                    
                        if (command.StartTime.HasValue && command.EndTime.HasValue)
                            return command.EndTime > command.StartTime;

                        return true;
                    }).WithMessage("EndTime must be later than StartTime.");
            });

            When(utc => utc.Tags is not null, () =>
            {
                RuleForEach(utc => utc.Tags)
                    .NotEmpty()
                    .WithMessage("Tag name cannot be empty.")
                    .MaximumLength(255)
                    .WithMessage("Tag name must not exceed 255 characters.");
            });
        });
    }
    private static bool IsValidUpdateCombination(UpdateTaskCommand command)
    {
        var isStatusUpdate = IsStatusUpdated(command);
        var isOrderUpdate = (command.NextTaskOrderIndex.HasValue || command.PreviousTaskOrderIndex.HasValue) && !string.IsNullOrEmpty(command.OrderType);
        var isMixedOrder = (command.NextTaskOrderIndex.HasValue || command.PreviousTaskOrderIndex.HasValue) ^ !string.IsNullOrEmpty(command.OrderType);

        if (isMixedOrder)
            return false;

        if (isStatusUpdate)
        {
            return
                command.Title is null &&
                command.Description is null &&
                command.Priority is null &&
                command.DueDate is null &&
                command.StartTime is null &&
                command.EndTime is null &&
                command.ProjectId is null &&
                command.Tags is null &&
                command.NextTaskOrderIndex is null &&
                command.PreviousTaskOrderIndex is null &&
                string.IsNullOrEmpty(command.OrderType);
        }

        if (isOrderUpdate)
        {
            return
                command.Title is null &&
                command.Description is null &&
                command.Priority is null &&
                command.DueDate is null &&
                command.StartTime is null &&
                command.EndTime is null &&
                command.ProjectId is null &&
                command.Tags is null &&
                string.IsNullOrEmpty(command.Status);
        }

        return true;
    }
    
    private static bool IsStatusUpdated(UpdateTaskCommand command)
    {
        return !string.IsNullOrEmpty(command.Status);
    }
    
    private static bool IsOrderCorrectlyUpdated(UpdateTaskCommand command)
    {
        return (command.NextTaskOrderIndex.HasValue || command.PreviousTaskOrderIndex.HasValue) && !string.IsNullOrEmpty(command.OrderType);
    }
    
    private static bool IsOrderUpdated(UpdateTaskCommand command)
    {
        return command.NextTaskOrderIndex.HasValue || command.PreviousTaskOrderIndex.HasValue || !string.IsNullOrEmpty(command.OrderType);
    }
    
    private static bool IsRegularFieldUpdated(UpdateTaskCommand command)
    {
        return command.Title is not null ||
               command.Description is not null ||
               command.Priority is not null ||
               command.DueDate is not null ||
               command.StartTime is not null ||
               command.EndTime is not null ||
               command.ProjectId is not null ||
               command.Tags is not null;
    }
}