using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Users.UpdateProfile;

public sealed record UpdateProfileCommand(string FirstName, string LastName) : ICommand<UpdateProfileCommandResponse>;