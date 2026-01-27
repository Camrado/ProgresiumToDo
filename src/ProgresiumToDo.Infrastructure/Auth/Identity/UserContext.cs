using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Infrastructure.Auth.Identity;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private Guid? _userId;
    private string? _email;
    private bool? _isEmailVerified;
    private PlanType? _currentPlan;
    
    public UserContext(IHttpContextAccessor httpContextAccessor) {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Guid UserId
    {
        get
        {
            if (_userId.HasValue)
                return _userId.Value;
                
            var claim = _httpContextAccessor.HttpContext?.User
                            .FindFirst(ClaimTypes.NameIdentifier)?.Value 
                        ?? throw new ApplicationException("User ID is unavailable");
                
            _userId = Guid.Parse(claim);
            return _userId.Value;
        }
    }
    
    public string Email
    {
        get
        {
            if (_email is not null)
                return _email;
                
            _email = _httpContextAccessor.HttpContext?.User
                         .FindFirst(ClaimTypes.Email)?.Value 
                     ?? throw new ApplicationException("User email is unavailable");
                
            return _email;
        }
    }
    
    public bool IsEmailVerified
    {
        get
        {
            if (_isEmailVerified.HasValue)
                return _isEmailVerified.Value;
                
            var claim = _httpContextAccessor.HttpContext?.User
                            .FindFirst(nameof(CustomClaim.EmailVerified))?.Value 
                        ?? throw new ApplicationException("User email verification status is unavailable");
                
            _isEmailVerified = claim.Equals("true", StringComparison.OrdinalIgnoreCase);
            return _isEmailVerified.Value;
        }
    }
    
    public PlanType CurrentPlan
    {
        get
        {
            if (_currentPlan.HasValue)
                return _currentPlan.Value;
                
            var claim = _httpContextAccessor.HttpContext?.User
                            .FindFirst(nameof(CustomClaim.CurrentPlanName))?.Value 
                        ?? throw new ApplicationException("User current plan is unavailable");
                
            _currentPlan = Enum.Parse<PlanType>(claim);
            return _currentPlan.Value;
        }
    }
}