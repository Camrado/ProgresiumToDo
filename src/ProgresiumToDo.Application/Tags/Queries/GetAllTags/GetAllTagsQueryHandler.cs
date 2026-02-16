using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Tags.Queries.GetAllTags;

internal sealed class GetAllTagsQueryHandler : IQueryHandler<GetAllTagsQuery, GetAllTagsQueryResponse>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUserContext _userContext;
    
    public GetAllTagsQueryHandler(ITagRepository tagRepository, IUserContext userContext)
    {
        _tagRepository = tagRepository;
        _userContext = userContext;
    }
    
    public async Task<Result<GetAllTagsQueryResponse>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.GetAllAsync(_userContext.UserId, cancellationToken: cancellationToken);
        
        var tagDtos = tags
            .Select(tag => new TagListItemDto(tag.Id, tag.Name, tag.CreatedAt, tag.UpdatedAt))
            .ToList();
        
        return new GetAllTagsQueryResponse("Tags retrieved successfully.", tagDtos);
    }
}