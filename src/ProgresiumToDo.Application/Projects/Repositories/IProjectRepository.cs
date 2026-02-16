using ProgresiumToDo.Domain.Projects;

namespace ProgresiumToDo.Application.Projects.Repositories;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, bool trackChanges = false, CancellationToken cancellationToken = default);
    
    Task<Project?> GetByNameAndUserIdAsync(string name, Guid userId, bool trackChanges = false, CancellationToken cancellationToken = default);
    
    Task<Project?> GetByIdAndUserIdAsync(Guid projectId, Guid userId, bool trackChanges = false, CancellationToken cancellationToken = default);
    
    Task<List<Project>> GetAllByUserIdAsync(Guid userId, bool trackChanges = false, CancellationToken cancellationToken = default);
    
    void Add(Project project);

    void Delete(Project project);
}