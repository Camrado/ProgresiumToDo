using ProgresiumToDo.Domain.Tags;

namespace ProgresiumToDo.Application.Tags.Repositories;

public interface ITagRepository
{
    void Add(Tag tag);
    
    void Delete(Tag tag);
    
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<List<Tag>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<Tag>> GetBySeveralIdsAsync(List<Guid> tagIds,
        CancellationToken cancellationToken = default);
}