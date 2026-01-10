using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tags;

namespace ProgresiumToDo.Application.Tags.Commands.CreateTag;

internal sealed class CreateTagCommandHandler : ICommandHandler<CreateTagCommand, CreateTagCommandResponse>
{
    private readonly ITagRepository _tagRepository;
    
    public CreateTagCommandHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }
    
    public Task<Result<CreateTagCommandResponse>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = Tag.Create(request.Name, request.Color, request.ProjectId);
        
        _tagRepository.Add(tag);
        
        var tagDto = new CreatedTagDto(tag.Id, tag.Name, tag.Color, tag.ProjectId, tag.CreatedAt);
        
        return Task.FromResult<Result<CreateTagCommandResponse>>(
            new CreateTagCommandResponse("Tag created successfully.", tagDto));
    }
}