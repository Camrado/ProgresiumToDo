using ProgresiumToDo.Application.Billing.SubscribeToPlan;

namespace ProgresiumToDo.Application.Users.GetCurrentUser;

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