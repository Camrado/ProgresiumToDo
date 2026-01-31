using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;

namespace ProgresiumToDo.Infrastructure.Auth.Identity;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private Guid? _userId;
    private string? _email;
    private bool? _isEmailVerified;
    
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
                            .FindFirst(nameof(CustomClaims.email_verified))?.Value 
                        ?? throw new ApplicationException("Email verification status is unavailable");
                
            _isEmailVerified = bool.Parse(claim);
            return _isEmailVerified.Value;
        }
    }
}