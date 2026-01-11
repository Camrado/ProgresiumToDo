using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;

namespace ProgresiumToDo.Infrastructure.Auth.Identity;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public UserContext(IHttpContextAccessor httpContextAccessor) {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Guid UserId => Guid.Parse(_httpContextAccessor.HttpContext?.User
                                         .Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value 
                                     ?? throw new ApplicationException("User ID is unavailable"));
    
    public string Email => _httpContextAccessor.HttpContext?.User
                               .Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value 
                           ?? throw new ApplicationException("User email is unavailable");
}