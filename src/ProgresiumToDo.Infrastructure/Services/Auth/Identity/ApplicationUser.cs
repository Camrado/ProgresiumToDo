using Microsoft.AspNetCore.Identity;

namespace ProgresiumToDo.Infrastructure.Services.Auth.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? EmailVerificationCode { get; set; }
    public DateTime? EmailVerificationCodeExpiresOn { get; set; }
    public DateTime? LastVerificationEmailSentTime { get; set; }
}