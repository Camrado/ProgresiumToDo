using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Projects.DeleteProject;

internal sealed class DeleteProjectCommandHandler : ICommandHandler<DeleteProjectCommand, DeleteProjectCommandResponse>
{
    private readonly IProjectRepository _projectRepository;

    public DeleteProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public Task<Result<DeleteProjectCommandResponse>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = request.Project!;
        
        _projectRepository.Delete(project);
        
        return Task.FromResult<Result<DeleteProjectCommandResponse>>(
            new DeleteProjectCommandResponse("Project deleted successfully."));
    }
}
