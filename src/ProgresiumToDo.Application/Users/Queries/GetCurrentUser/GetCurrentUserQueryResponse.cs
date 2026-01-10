using ProgresiumToDo.Application.Billing.Commands.SubscribeToPlan;

namespace ProgresiumToDo.Application.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQueryResponse(
    string Message,
    CurrentUserDto User);
    
public sealed record CurrentUserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    bool IsEmailVerified,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    SubscriptionDto Subscription);