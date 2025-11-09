using System.Security.Cryptography;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.Auth;

public sealed class RefreshToken
{
    public Guid Id { get; private set; } =  Guid.CreateVersion7();
    
    public Guid UserId { get; private set; }
    
    public string Token { get; private set; }
    
    public string? DeviceName { get; private set; }
    
    public string? IpAddress { get; private set; }
    
    public string? UserAgent { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    public DateTime ExpiresAt { get; private set; }
    
    public DateTime? RevokedAt { get; private set; }
    
    public Guid? ReplacedByTokenId { get; private set; }
    
    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
    
    public User User { get; private set; }
    
    public RefreshToken? ReplacedByToken { get; private set; }
    
    public ICollection<RefreshToken> ReplacedTokens { get; private set; } = new List<RefreshToken>();

    private RefreshToken(Guid userId, string token, DateTime expiresAt, string? deviceName, string? ipAddress,
        string? userAgent)
    {
        UserId = userId;
        Token = token;
        DeviceName = deviceName;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        ExpiresAt = expiresAt;
    }

    public static RefreshToken Create(Guid userId, DateTime expiresAt, string? deviceName = null,
        string? ipAddress = null, string? userAgent = null)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        
        return new RefreshToken(userId, token, expiresAt, deviceName, ipAddress, userAgent);
    }

    public void Revoke(Guid? replacedByTokenId = null)
    {
        RevokedAt = DateTime.UtcNow;
        ReplacedByTokenId = replacedByTokenId;
    }
}