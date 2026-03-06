using ProgresiumToDo.Domain.Waitlist;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Domain.Waitlist;

public class WaitlistTests
{
    [Theory]
    [InlineData("test@gmail.com")]
    public void Create_Should_SetPropertyValues(string email)
    {
        var entry = WaitlistEntry.Create(email);

        entry.ShouldNotBeNull();
        entry.Email.ShouldBe(email);
    }

    [Fact]
    public void Create_Should_GenerateId()
    {
        var entry = WaitlistEntry.Create("mail@gmail.com");
        
        entry.Id.ShouldNotBe(Guid.Empty);
    }
}