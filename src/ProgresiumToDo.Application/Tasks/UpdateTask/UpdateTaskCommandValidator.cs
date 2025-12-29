using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Domain.Tasks;
using TaskStatus = ProgresiumToDo.Domain.Tasks.TaskStatus;

namespace ProgresiumToDo.Application.Tasks.UpdateTask;

internal sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator(ITaskItemRepository taskItemRepository, IUserContext userContext, IProjectRepository projectRepository)
    {
        RuleFor(utc => utc.TaskId)
            .NotNull()
            .MustAsync(async (command, taskId, cancellationToken) =>
            {
                var taskItem = await taskItemRepository.GetByIdAsync(taskId,  userContext.UserId, cancellationToken);
                command.TaskItem = taskItem;
                
                return taskItem != null;
            })
            .WithMessage("TaskItem not found.");
        
        RuleFor(utc => utc.Title)
            .MaximumLength(256)
            .WithMessage("Title must not exceed 256 characters.");
        
        RuleFor(utc => utc.Priority)
            .Must(priority => string.IsNullOrEmpty(priority) || Enum.TryParse<Priority>(priority, ignoreCase: true, out _))
            .WithMessage("Invalid priority. Valid values are: none, low, medium, high.");

        RuleFor(utc => utc.Status)
            .Must(status => string.IsNullOrEmpty(status) || Enum.TryParse<TaskStatus>(status, ignoreCase: true, out _))
            .WithMessage("Invalid status. Valid values are: pending, inprogress, completed, cancelled.");
        
        RuleFor(utc => utc.OrderIndex)
            .GreaterThanOrEqualTo(0)
            .WithMessage("OrderIndex must be greater than or equal to 0.");

        RuleFor(utc => utc.ProjectId)
            .MustAsync(async (command, projectId, cancellationToken) =>
            {
                if (!projectId.HasValue)
                    return true;
                
                var project = await projectRepository
                    .GetByIdAndUserIdAsync(projectId.Value, userContext.UserId, cancellationToken);
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
    }
}