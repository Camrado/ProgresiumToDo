using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Users.GetCurrentUser;

public sealed record GetCurrentUserQuery() : IQuery<GetCurrentUserQueryResponse>;