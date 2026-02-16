using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Tags;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Domain.Tags;

namespace ProgresiumToDo.Infrastructure.Services.Tags;

internal sealed class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly IUserContext _userContext;
    
    public TagService(ITagRepository tagRepository, IUserContext userContext)
    {
        _tagRepository = tagRepository;
        _userContext = userContext;
    }
    
    public async Task<List<Tag>> GetOrCreateTagsAsync(List<string> tagNames, CancellationToken cancellationToken = default)
    {
        tagNames = tagNames.Distinct().ToList();
            
        var tagsExistingInDb = await _tagRepository.GetByNamesAsync(tagNames, _userContext.UserId, cancellationToken: cancellationToken);
        var tagNamesNotExistingInDb = tagNames
            .Where(name => !tagsExistingInDb
                .Select(t => t.Name)
                .Contains(name))
            .ToList();
            
        var tagsAddedToDb = new List<Tag>(tagNamesNotExistingInDb.Count);
        foreach (var name in tagNamesNotExistingInDb)
        {
            var newTag = Tag.Create(name, _userContext.UserId);
            _tagRepository.Add(newTag);
            tagsAddedToDb.Add(newTag);
        }

        return tagsExistingInDb.Concat(tagsAddedToDb).ToList();
    }
}