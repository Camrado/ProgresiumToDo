using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery() : IQuery<GetCurrentUserQueryResponse>;