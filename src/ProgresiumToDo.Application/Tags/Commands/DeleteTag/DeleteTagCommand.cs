using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tags;

namespace ProgresiumToDo.Application.Tags.Commands.DeleteTag;

public sealed record DeleteTagCommand(Guid TagId) : ICommand<DeleteTagCommandResponse>
{
    public Guid ProjectId { get; set; }
    
    internal Tag? Tag { get; set; }
}