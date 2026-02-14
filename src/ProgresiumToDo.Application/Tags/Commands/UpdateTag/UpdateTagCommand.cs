using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tags;

namespace ProgresiumToDo.Application.Tags.Commands.UpdateTag;

public sealed record UpdateTagCommand(string? Name) : ICommand<UpdateTagCommandResponse>
{
    public Guid TagId { get; set; }
    
    internal Tag? Tag { get; set; }
}