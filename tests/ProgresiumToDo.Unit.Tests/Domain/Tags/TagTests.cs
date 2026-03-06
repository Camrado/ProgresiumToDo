using ProgresiumToDo.Domain.Tags;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Domain.Tags;

public class TagTests
{
    [Theory]
    [InlineData("Urgent")]
    [InlineData("Work")]
    public void Create_Should_SetPropertyValues(string name)
    {
        var userId = Guid.NewGuid();

        var tag = Tag.Create(name, userId);

        tag.ShouldNotBeNull();
        tag.Name.ShouldBe(name);
        tag.UserId.ShouldBe(userId);
    }

    [Fact]
    public void Create_Should_GenerateId()
    {
        var tag = Tag.Create("TestTag", Guid.NewGuid());

        tag.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Create_Should_SetCreatedAtAndUpdatedAt()
    {
        var before = DateTime.UtcNow;

        var tag = Tag.Create("TestTag", Guid.NewGuid());

        tag.CreatedAt.ShouldBeGreaterThanOrEqualTo(before);
        tag.UpdatedAt.ShouldBeGreaterThanOrEqualTo(before);
    }

    [Fact]
    public void Create_Should_InitializeEmptyTaskItems()
    {
        var tag = Tag.Create("TestTag", Guid.NewGuid());

        tag.TaskItems.ShouldNotBeNull();
        tag.TaskItems.ShouldBeEmpty();
    }

    // Update

    [Fact]
    public void Update_WithName_Should_ChangeName()
    {
        var tag = Tag.Create("Original", Guid.NewGuid());

        tag.Update("Updated");

        tag.Name.ShouldBe("Updated");
    }

    [Fact]
    public void Update_WithNull_Should_NotChangeName()
    {
        var tag = Tag.Create("Original", Guid.NewGuid());

        tag.Update(null);

        tag.Name.ShouldBe("Original");
    }

    // BaseEntity methods

    [Fact]
    public void MarkAsDeleted_Should_SetDeletedAt()
    {
        var tag = Tag.Create("TestTag", Guid.NewGuid());
        tag.DeletedAt.ShouldBeNull();

        tag.MarkAsDeleted();

        tag.DeletedAt.ShouldNotBeNull();
    }

    [Fact]
    public void MarkAsUpdated_Should_RefreshUpdatedAt()
    {
        var tag = Tag.Create("TestTag", Guid.NewGuid());
        var originalUpdatedAt = tag.UpdatedAt;

        tag.MarkAsUpdated();

        tag.UpdatedAt.ShouldBeGreaterThanOrEqualTo(originalUpdatedAt);
    }
}
