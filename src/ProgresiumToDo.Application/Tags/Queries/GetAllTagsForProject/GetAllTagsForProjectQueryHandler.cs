using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Tags.Queries.GetAllTagsForProject;

internal sealed class GetAllTagsForProjectQueryHandler : IQueryHandler<GetAllTagsForProjectQuery, GetAllTagsForProjectQueryResponse>
{
    private readonly ITagRepository _tagRepository;
    
    public GetAllTagsForProjectQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }
    
    public async Task<Result<GetAllTagsForProjectQueryResponse>> Handle(GetAllTagsForProjectQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);
        
        var tagDtos = tags
            .Select(tag => new TagListItemDto(tag.Id, tag.Name, tag.Color, tag.CreatedAt, tag.UpdatedAt))
            .ToList();
        
        return new GetAllTagsForProjectQueryResponse("Tags retrieved successfully.", tagDtos);
    }
}