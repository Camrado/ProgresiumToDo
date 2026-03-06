using ProgresiumToDo.Domain.Auth;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Domain.Auth;

public class VerificationCodeTests
{
    [Fact]
    public void CreateEmailVerificationCode_Should_SetPropertyValues()
    {
        var appUserId = Guid.NewGuid();
        var codeHash = "hashedCode123";
        var expiresOn = DateTime.UtcNow.AddHours(1);
        var before = DateTime.UtcNow;

        var code = VerificationCode.CreateEmailVerificationCode(appUserId, codeHash, expiresOn);

        code.ShouldNotBeNull();
        code.ApplicationUserId.ShouldBe(appUserId);
        code.Type.ShouldBe(VerificationCodeType.EmailVerification);
        code.CodeHash.ShouldBe(codeHash);
        code.ExpiresOn.ShouldBe(expiresOn);
        code.CreatedAt.ShouldBeGreaterThanOrEqualTo(before);
        code.LastSentAt.ShouldNotBeNull();
        code.LastSentAt.Value.ShouldBeGreaterThanOrEqualTo(before);
    }

    [Fact]
    public void CreatePasswordResetCode_Should_SetPropertyValues()
    {
        var appUserId = Guid.NewGuid();
        var codeHash = "pwresetHashxyz";
        var expiresOn = DateTime.UtcNow.AddMinutes(15);

        var code = VerificationCode.CreatePasswordResetCode(appUserId, codeHash, expiresOn);

        code.ShouldNotBeNull();
        code.ApplicationUserId.ShouldBe(appUserId);
        code.Type.ShouldBe(VerificationCodeType.PasswordReset);
        code.CodeHash.ShouldBe(codeHash);
        code.ExpiresOn.ShouldBe(expiresOn);
    }

    [Fact]
    public void Create_Should_GenerateId()
    {
        var code = VerificationCode.CreateEmailVerificationCode(Guid.NewGuid(), "hash", DateTime.UtcNow.AddHours(1));

        code.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void IsExpired_ShouldBeFalse_WhenExpiresOnIsInFuture()
    {
        var code = VerificationCode.CreateEmailVerificationCode(Guid.NewGuid(), "hash", DateTime.UtcNow.AddHours(1));

        code.IsExpired.ShouldBeFalse();
    }

    [Fact]
    public void IsExpired_ShouldBeTrue_WhenExpiresOnIsInPast()
    {
        var code = VerificationCode.CreateEmailVerificationCode(Guid.NewGuid(), "hash", DateTime.UtcNow.AddHours(-1));

        code.IsExpired.ShouldBeTrue();
    }

    [Fact]
    public void UpdateCode_Should_ChangeCodeHashAndExpiresOn_And_RefreshLastSentAt()
    {
        var code = VerificationCode.CreateEmailVerificationCode(Guid.NewGuid(), "oldHash", DateTime.UtcNow.AddHours(1));
        var originalLastSentAt = code.LastSentAt;

        var newHash = "newHash";
        var newExpiresOn = DateTime.UtcNow.AddHours(2);

        // Sleep briefly to ensure LastSentAt is distinctly newer (resolving potential CI timing issues)
        System.Threading.Thread.Sleep(50);
        
        var beforeUpdate = DateTime.UtcNow;
        code.UpdateCode(newHash, newExpiresOn);

        code.CodeHash.ShouldBe(newHash);
        code.ExpiresOn.ShouldBe(newExpiresOn);
        code.LastSentAt.ShouldNotBeNull();
        code.LastSentAt.Value.ShouldBeGreaterThanOrEqualTo(beforeUpdate);
        code.LastSentAt.Value.ShouldBeGreaterThan(originalLastSentAt!.Value);
    }
}
