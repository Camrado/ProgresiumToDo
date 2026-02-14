using Microsoft.AspNetCore.Authorization;

namespace ProgresiumToDo.Infrastructure.Services.Auth.Authentication;

public class AuthorizeVerifiedAttribute : AuthorizeAttribute
{
    public AuthorizeVerifiedAttribute()
    {
        Policy = AuthorizationPolicies.EmailVerified;
    }
}