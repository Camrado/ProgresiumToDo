using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Application.Projects.Repositories;
using ProgresiumToDo.Application.Tags.Repositories;

namespace ProgresiumToDo.Application.Tags.Commands.CreateTag;

internal sealed class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator(ITagRepository tagRepository, IProjectRepository projectRepository, IUserContext userContext)
    {
        RuleFor(ctc => ctc)
            .MustAsync(async (command, cancellationToken) =>
            {
                var existingTag = await tagRepository.GetByProjectIdAndNameAsync(command.ProjectId, command.Name, cancellationToken);
                return existingTag is null;
            })
            .WithMessage("A tag with the same name already exists in the specified project.");
        
        RuleFor(ctc => ctc.ProjectId)
            .NotEmpty()
            .WithMessage("ProjectId is required.")
            .MustAsync(async (command, projectId, cancellationToken) =>
            {
                var project = await projectRepository.GetByIdAndUserIdAsync(projectId, userContext.UserId, cancellationToken);
                return project is not null;
            })
            .WithMessage("Project does not exist.");

        RuleFor(ctc => ctc.Name)
            .NotEmpty()
            .WithMessage("Tag name is required.")
            .MaximumLength(255)
            .WithMessage("Tag name must not exceed 255 characters.");
        
        RuleFor(ctc => ctc.Color)
            .NotEmpty()
            .WithMessage("Color is required.")
            .Matches("^#[0-9A-Fa-f]{6}$")
            .WithMessage("Color must be a valid hex value like #000000");
    }
}