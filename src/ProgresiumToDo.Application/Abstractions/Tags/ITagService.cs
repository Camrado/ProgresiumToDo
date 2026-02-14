using ProgresiumToDo.Domain.Tags;

namespace ProgresiumToDo.Application.Abstractions.Tags;

public interface ITagService
{
    Task<List<Tag>> GetOrCreateTagsAsync(List<string> tagNames, CancellationToken cancellationToken = default);
}