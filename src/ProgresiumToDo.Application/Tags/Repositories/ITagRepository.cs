using ProgresiumToDo.Domain.Tags;

namespace ProgresiumToDo.Application.Tags.Repositories;

public interface ITagRepository
{
    void Add(Tag tag);
    
    void Delete(Tag tag);
    
    Task<Tag?> GetByProjectIdAndNameAsync(Guid projectId, string name, CancellationToken cancellationToken = default);

    Task<List<Tag>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);

    Task<Tag?> GetByIdAndProjectIdAsync(Guid id, Guid projectId, CancellationToken cancellationToken = default);

    Task<List<Tag>> GetBySeveralIdsAndProjectIdAsync(List<Guid> tagIds, Guid projectId,
        CancellationToken cancellationToken = default);
}