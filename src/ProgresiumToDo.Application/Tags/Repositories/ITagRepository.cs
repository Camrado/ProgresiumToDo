using ProgresiumToDo.Domain.Tags;

namespace ProgresiumToDo.Application.Tags.Repositories;

public interface ITagRepository
{
    void Add(Tag tag);
    
    void Delete(Tag tag);
    
    Task<Tag?> GetByNameAsync(string name, Guid userId, CancellationToken cancellationToken = default);

    Task<List<Tag>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Tag?> GetByIdAndUserIdAsync(Guid tagId, Guid userId, CancellationToken cancellationToken = default);

    Task<List<Tag>> GetByNamesAsync(List<string> names, Guid userId, CancellationToken cancellationToken = default);
}