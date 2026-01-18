using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Users.Queries.GetEntitlementStatus;

public sealed record GetEntitlementStatusQuery() : IQuery<GetEntitlementStatusQueryResponse>;