using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Projects.Repositories;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.Queries.GetTasks;

internal sealed class GetTasksQueryValidator : AbstractValidator<GetTasksQuery>
{
    public GetTasksQueryValidator(IProjectRepository projectRepository, IUserContext userContext)
    {
        RuleFor(gtq => gtq)
            .Must(query =>
            {
                if (query.DueDateFrom.HasValue && query.DueDateTo.HasValue)
                    return query.DueDateFrom.Value <= query.DueDateTo.Value;
                
                return true;
            })
            .WithMessage("'DueDateFrom' must be less than or equal to 'DueDateTo'.");
        
        RuleFor(gtq => gtq.ProjectId)
            .MustAsync(async (command, projectId, cancellationToken) =>
            {
                if (!projectId.HasValue || projectId == Guid.Empty)
                    return true;
                
                var project = await projectRepository.GetByIdAndUserIdAsync(projectId.Value, userContext.UserId, cancellationToken);
                return project != null;
            })
            .WithMessage("ProjectDetails not found.");
        
        RuleFor(gtq => gtq.OrderType)
            .Must(orderType => string.IsNullOrEmpty(orderType) || Enum.TryParse<OrderType>(orderType, ignoreCase: true, out _))
            .WithMessage("Invalid OrderType. Valid values are: ByDueDate, ByProject.");

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