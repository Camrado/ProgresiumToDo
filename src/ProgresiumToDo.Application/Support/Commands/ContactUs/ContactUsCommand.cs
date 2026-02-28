using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Support.Commands.ContactUs;

public sealed record ContactUsCommand(
    string Email, 
    string Name, 
    string Subject, 
    string Message) : ICommand<ContactUsCommandResponse>, INonTransactionalCommand;