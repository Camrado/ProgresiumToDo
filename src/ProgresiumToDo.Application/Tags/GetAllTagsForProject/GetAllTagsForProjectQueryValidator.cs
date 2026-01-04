using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tags.GetAllTagsForProject;

internal sealed class GetAllTagsForProjectQueryValidator : AbstractValidator<GetAllTagsForProjectQuery>
{
    public GetAllTagsForProjectQueryValidator(IProjectRepository projectRepository, IUserContext userContext)
    {
        RuleFor(gat => gat.ProjectId)
            .NotEmpty()
            .WithMessage("ProjectId must not be empty.")
            .MustAsync(async (query, projectId, cancellationToken) =>
            {
                var project = await projectRepository.GetByIdAndUserIdAsync(projectId, userContext.UserId, cancellationToken);
                return project is not null;
            })
            .WithMessage("Project does not exist.");
    }
}