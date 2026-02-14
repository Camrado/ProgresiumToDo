using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Tags.Commands.UpdateTag;

internal sealed class UpdateTagCommandHandler : ICommandHandler<UpdateTagCommand, UpdateTagCommandResponse>
{
    public Task<Result<UpdateTagCommandResponse>> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = request.Tag!;
        
        tag.Update(request.Name);

        return Task.FromResult<Result<UpdateTagCommandResponse>>(
            new UpdateTagCommandResponse("Tag updated successfully."));
    }
}