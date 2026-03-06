using ProgresiumToDo.Domain.Auth;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Domain.Auth;

public class RefreshTokenTests
{
    private static RefreshToken CreateDefaultToken(
        Guid? userId = null,
        DateTime? expiresAt = null,
        string? deviceName = "TestDevice",
        string? ipAddress = "127.0.0.1",
        string? userAgent = "TestAgent")
    {
        return RefreshToken.Create(
            userId ?? Guid.NewGuid(),
            expiresAt ?? DateTime.UtcNow.AddDays(7),
            deviceName,
            ipAddress,
            userAgent);
    }

    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        var userId = Guid.NewGuid();
        var expiresAt = DateTime.UtcNow.AddDays(7);
        var deviceName = "MyPhone";
        var ipAddress = "192.168.1.1";
        var userAgent = "Postman";

        var token = RefreshToken.Create(userId, expiresAt, deviceName, ipAddress, userAgent);

        token.ShouldNotBeNull();
        token.UserId.ShouldBe(userId);
        token.Token.ShouldNotBeNullOrWhiteSpace();
        token.ExpiresAt.ShouldBe(expiresAt);
        token.DeviceName.ShouldBe(deviceName);
        token.IpAddress.ShouldBe(ipAddress);
        token.UserAgent.ShouldBe(userAgent);
        token.RevokedAt.ShouldBeNull();
        token.ReplacedByTokenId.ShouldBeNull();
    }

    [Fact]
    public void Create_Should_GenerateId()
    {
        var token = CreateDefaultToken();

        token.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Create_Should_SetCreatedAt()
    {
        var before = DateTime.UtcNow;

        var token = CreateDefaultToken();

        token.CreatedAt.ShouldBeGreaterThanOrEqualTo(before);
        token.CreatedAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);
    }

    [Fact]
    public void Create_Should_GenerateUniqueBase64Tokens()
    {
        var token1 = CreateDefaultToken();
        var token2 = CreateDefaultToken();

        token1.Token.ShouldNotBe(token2.Token);
        Should.NotThrow(() => Convert.FromBase64String(token1.Token));
    }

    [Fact]
    public void IsActive_ShouldBeTrue_WhenNotRevokedAndNotExpired()
    {
        var token = CreateDefaultToken(expiresAt: DateTime.UtcNow.AddDays(1));

        token.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void IsActive_ShouldBeFalse_WhenExpired()
    {
        var token = CreateDefaultToken(expiresAt: DateTime.UtcNow.AddDays(-1));

        token.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void IsActive_ShouldBeFalse_WhenRevoked()
    {
        var token = CreateDefaultToken(expiresAt: DateTime.UtcNow.AddDays(1));
        
        token.Revoke();

        token.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Revoke_Should_SetRevokedAt_And_ReplacedByTokenId()
    {
        var token = CreateDefaultToken();
        var replacedById = Guid.NewGuid();
        var before = DateTime.UtcNow;

        token.Revoke(replacedById);

        token.RevokedAt.ShouldNotBeNull();
        token.RevokedAt.Value.ShouldBeGreaterThanOrEqualTo(before);
        token.ReplacedByTokenId.ShouldBe(replacedById);
    }
}
