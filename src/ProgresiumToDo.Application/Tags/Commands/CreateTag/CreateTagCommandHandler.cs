using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tags;

namespace ProgresiumToDo.Application.Tags.Commands.CreateTag;

internal sealed class CreateTagCommandHandler : ICommandHandler<CreateTagCommand, CreateTagCommandResponse>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUserContext _userContext;
    
    public CreateTagCommandHandler(ITagRepository tagRepository, IUserContext userContext)
    {
        _tagRepository = tagRepository;
        _userContext = userContext;
    }
    
    public Task<Result<CreateTagCommandResponse>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = Tag.Create(request.Name, _userContext.UserId);
        
        _tagRepository.Add(tag);
        
        var tagDto = new CreatedTagDto(tag.Id, tag.Name, tag.CreatedAt);
        
        return Task.FromResult<Result<CreateTagCommandResponse>>(
            new CreateTagCommandResponse("Tag created successfully.", tagDto));
    }
}