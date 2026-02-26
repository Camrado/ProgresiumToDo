namespace ProgresiumToDo.Domain.Auth;

public class VerificationCode
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid ApplicationUserId { get; private set; }

    public VerificationCodeType Type { get; private set; }
    public string CodeHash { get; private set; } = string.Empty;

    public DateTime ExpiresOn { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastSentAt { get; private set; }

    public bool IsExpired => ExpiresOn <= DateTime.UtcNow;

    private VerificationCode()
    {
    }

    private VerificationCode(Guid applicationUserId, VerificationCodeType type, string codeHash, DateTime expiresOn)
    {
        ApplicationUserId = applicationUserId;
        Type = type;
        CodeHash = codeHash;
        ExpiresOn = expiresOn;
        CreatedAt = DateTime.UtcNow;
        LastSentAt = DateTime.UtcNow;
    }
    
    public static VerificationCode CreateEmailVerificationCode(Guid applicationUserId, string codeHash, DateTime expiresOn)
    {
        return new VerificationCode(applicationUserId, VerificationCodeType.EmailVerification, codeHash, expiresOn);
    }
    
    public static VerificationCode CreatePasswordResetCode(Guid applicationUserId, string codeHash, DateTime expiresOn)
    {
        return new VerificationCode(applicationUserId, VerificationCodeType.PasswordReset, codeHash, expiresOn);
    }
    
    public void UpdateCode(string newCodeHash, DateTime newExpiresOn)
    {
        CodeHash = newCodeHash;
        ExpiresOn = newExpiresOn;
        LastSentAt = DateTime.UtcNow;
    }
}
