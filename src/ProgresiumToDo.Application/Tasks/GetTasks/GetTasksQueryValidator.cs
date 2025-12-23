using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.GetTasks;

internal sealed class GetTasksQueryValidator : AbstractValidator<GetTasksQuery>
{
    public GetTasksQueryValidator(IProjectRepository projectRepository, IUserContext userContext)
    {
        RuleFor(gtq => gtq.DueDateTo)
            .NotEmpty()
            .WithMessage("'DueDateTo' must be provided.");
        
        RuleFor(gtq => gtq.DueDateFrom)
            .NotEmpty()
            .WithMessage("'DueDateFrom' must be provided.")
            .LessThanOrEqualTo(gtq => gtq.DueDateTo)
            .WithMessage("'DueDateFrom' must be less than or equal to 'DueDateTo'.");
        
        RuleFor(gtq => gtq.ProjectId)
            .MustAsync(async (command, projectId, cancellationToken) =>
            {
                if (!projectId.HasValue)
                    return true;
                
                var project = await projectRepository.GetByIdAndUserIdAsync(projectId.Value, userContext.UserId, cancellationToken);
                return project != null;
            })
            .WithMessage("Project not found.");

        When(gtq => gtq.Page.HasValue || gtq.PageSize.HasValue, () =>
        {
            RuleFor(gtq => gtq.Page)
                .NotNull()
                .WithMessage("Page must be provided if PageSize is provided.")
                .GreaterThan(0)
                .WithMessage("Page must be greater than 0.");

            RuleFor(gtq => gtq.PageSize)
                .NotNull()
                .WithMessage("PageSize must be provided if Page is provided.")
                .GreaterThan(0)
                .WithMessage("PageSize must be greater than 0.");
        });
        
        RuleFor(gtq => gtq.SortOrder)
            .Must(so => so == null || so.Equals("ASC", StringComparison.OrdinalIgnoreCase) || so.Equals("DESC", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SortOrder must be 'ASC' or 'DESC'.");
        
        RuleFor(gtq => gtq.SortBy)
            .Must(sb =>
            {
                if (string.IsNullOrEmpty(sb))
                    return true;

                return new[] { "DueDate", "OrderIndex", "CreatedAt", "Priority" }
                    .Any(valid => valid.Equals(sb, StringComparison.OrdinalIgnoreCase));
            })
            .WithMessage("SortBy must be one of 'DueDate', 'OrderIndex', 'CreatedAt', or 'Priority'.");
    }
}