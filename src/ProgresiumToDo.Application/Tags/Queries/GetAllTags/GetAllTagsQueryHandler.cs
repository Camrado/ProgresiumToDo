using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Tags.Queries.GetAllTags;

internal sealed class GetAllTagsQueryHandler : IQueryHandler<GetAllTagsQuery, GetAllTagsQueryResponse>
{
    private readonly ITagRepository _tagRepository;
    
    public GetAllTagsQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }
    
    public async Task<Result<GetAllTagsQueryResponse>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.GetAllAsync(cancellationToken);
        
        var tagDtos = tags
            .Select(tag => new TagListItemDto(tag.Id, tag.Name, tag.Color, tag.CreatedAt, tag.UpdatedAt))
            .ToList();
        
        return new GetAllTagsQueryResponse("Tags retrieved successfully.", tagDtos);
    }
}