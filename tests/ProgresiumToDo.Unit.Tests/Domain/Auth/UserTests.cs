using ProgresiumToDo.Domain.Auth;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Domain.Auth;

public class UserTests
{
    private static User CreateDefaultUser(
        string email = "test@example.com",
        string firstName = "John",
        string lastName = "Doe",
        Guid? applicationUserId = null)
    {
        return User.Create(email, firstName, lastName, applicationUserId ?? Guid.NewGuid());
    }

    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        var email = "john.doe@example.com";
        var firstName = "John";
        var lastName = "Doe";
        var appUserId = Guid.NewGuid();

        var user = User.Create(email, firstName, lastName, appUserId);

        user.ShouldNotBeNull();
        user.Email.ShouldBe(email);
        user.FirstName.ShouldBe(firstName);
        user.LastName.ShouldBe(lastName);
        user.ApplicationUserId.ShouldBe(appUserId);
        user.IsEmailVerified.ShouldBeFalse();
    }

    [Fact]
    public void Create_Should_GenerateId()
    {
        var user = CreateDefaultUser();

        user.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Create_Should_SetCreatedAtAndUpdatedAt()
    {
        var before = DateTime.UtcNow;

        var user = CreateDefaultUser();

        user.CreatedAt.ShouldBeGreaterThanOrEqualTo(before);
        user.UpdatedAt.ShouldBeGreaterThanOrEqualTo(before);
    }

    [Fact]
    public void Create_Should_InitializeEmptyCollections()
    {
        var user = CreateDefaultUser();

        user.RefreshTokens.ShouldNotBeNull();
        user.RefreshTokens.ShouldBeEmpty();
        
        user.Subscriptions.ShouldNotBeNull();
        user.Subscriptions.ShouldBeEmpty();
        
        user.FeatureUsages.ShouldNotBeNull();
        user.FeatureUsages.ShouldBeEmpty();
        
        user.Projects.ShouldNotBeNull();
        user.Projects.ShouldBeEmpty();
        
        user.TaskItems.ShouldNotBeNull();
        user.TaskItems.ShouldBeEmpty();
        
        user.Tags.ShouldNotBeNull();
        user.Tags.ShouldBeEmpty();
    }

    [Fact]
    public void Update_Should_ChangeProvidedFields()
    {
        var user = CreateDefaultUser();

        user.Update("Jane", "Smith");

        user.FirstName.ShouldBe("Jane");
        user.LastName.ShouldBe("Smith");
    }

    [Fact]
    public void Update_WithNulls_Should_NotChangeFields()
    {
        var user = CreateDefaultUser(firstName: "OriginalFirst", lastName: "OriginalLast");

        user.Update(null, null);

        user.FirstName.ShouldBe("OriginalFirst");
        user.LastName.ShouldBe("OriginalLast");
    }

    [Fact]
    public void UpdateEmail_Should_ChangeEmail_And_ResetIsEmailVerified()
    {
        var user = CreateDefaultUser(email: "old@example.com");
        user.VerifyEmail();
        user.IsEmailVerified.ShouldBeTrue();

        user.UpdateEmail("new@example.com");

        user.Email.ShouldBe("new@example.com");
        user.IsEmailVerified.ShouldBeFalse();
    }

    [Fact]
    public void VerifyEmail_Should_SetIsEmailVerifiedToTrue()
    {
        var user = CreateDefaultUser();
        user.IsEmailVerified.ShouldBeFalse();

        user.VerifyEmail();

        user.IsEmailVerified.ShouldBeTrue();
    }
}
