using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tags.GetAllTagsForProject;

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