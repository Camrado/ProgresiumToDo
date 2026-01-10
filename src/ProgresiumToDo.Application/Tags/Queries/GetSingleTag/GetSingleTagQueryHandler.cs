using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Tags.Queries.GetSingleTag;

internal sealed class GetSingleTagQueryHandler : IQueryHandler<GetSingleTagQuery, GetSingleTagQueryResponse>
{
    public Task<Result<GetSingleTagQueryResponse>> Handle(GetSingleTagQuery request, CancellationToken cancellationToken)
    {
        var tag = request.Tag!;
        var tagDto = new TagDto(tag.Id, tag.Name, tag.Color, tag.ProjectId, request.Project!.Name, tag.CreatedAt, tag.UpdatedAt);
        
        return Task.FromResult<Result<GetSingleTagQueryResponse>>(
            new GetSingleTagQueryResponse("Tag retrieved successfully.", tagDto));
    }
}