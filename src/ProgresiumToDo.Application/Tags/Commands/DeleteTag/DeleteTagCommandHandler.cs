using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Tags.Commands.DeleteTag;

internal sealed class DeleteTagCommandHandler : ICommandHandler<DeleteTagCommand, DeleteTagCommandResponse>
{
    private readonly ITagRepository _tagRepository;
    
    public DeleteTagCommandHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }
    
    public Task<Result<DeleteTagCommandResponse>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = request.Tag!;
        
        _tagRepository.Delete(tag);
        
        return Task.FromResult<Result<DeleteTagCommandResponse>>(
            new DeleteTagCommandResponse("Tag deleted successfully."));
    }
}