using Microsoft.AspNetCore.Authorization;

namespace ProgresiumToDo.Infrastructure.Auth.Authentication;

public class AuthorizeVerifiedAttribute : AuthorizeAttribute
{
    public AuthorizeVerifiedAttribute()
    {
        Policy = AuthorizationPolicies.EmailVerified;
    }
}