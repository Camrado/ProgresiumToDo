using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Waitlist.Commands.JoinWaitlist;

public sealed record JoinWaitlistCommand(string Email) : ICommand<JoinWaitlistCommandResponse>;