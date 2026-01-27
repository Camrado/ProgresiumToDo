using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Application.Abstractions.Auth.Identity;

public interface IUserContext
{
    Guid UserId { get; }
    
    string Email { get; }
    
    bool IsEmailVerified { get; }
    
    PlanType CurrentPlan { get; }
}