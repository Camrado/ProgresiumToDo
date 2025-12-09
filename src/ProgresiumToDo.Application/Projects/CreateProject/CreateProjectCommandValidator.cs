using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Projects.CreateProject;

internal sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    private readonly IUserContext _userContext;
    private readonly IProjectRepository _projectRepository;
    
    public CreateProjectCommandValidator(IUserContext userContext, IProjectRepository projectRepository)
    {
        _userContext = userContext;
        _projectRepository = projectRepository;
        
        RuleFor(cpc => cpc.Name)
            .NotEmpty()
            .MustAsync(async (name, cancellationToken) =>
            {
                var existingProject = await _projectRepository.GetByNameAndUserIdAsync(name, _userContext.UserId, cancellationToken);
                return existingProject == null;
            }).WithMessage("A project with this name already exists.");
    }
}