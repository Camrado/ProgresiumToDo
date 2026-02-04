using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Users.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(string? FirstName, string? LastName, string? Email) : ICommand<UpdateProfileCommandResponse>;