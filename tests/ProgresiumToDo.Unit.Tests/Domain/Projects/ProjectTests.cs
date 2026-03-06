using ProgresiumToDo.Domain.Projects;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Domain.Projects;

public class ProjectTests
{
    private static Project CreateDefaultProject(
        string name = "Test Project",
        string? description = "Test Description",
        Guid? userId = null)
    {
        return Project.Create(name, description, userId ?? Guid.NewGuid());
    }

    // Create

    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        var userId = Guid.NewGuid();

        var project = Project.Create("My Project", "My Description", userId);

        project.ShouldNotBeNull();
        project.Name.ShouldBe("My Project");
        project.Description.ShouldBe("My Description");
        project.UserId.ShouldBe(userId);
    }

    [Fact]
    public void Create_WithNullDescription_Should_SetNull()
    {
        var project = CreateDefaultProject(description: null);

        project.Description.ShouldBeNull();
    }

    [Fact]
    public void Create_Should_GenerateId()
    {
        var project = CreateDefaultProject();

        project.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Create_Should_SetCreatedAtAndUpdatedAt()
    {
        var before = DateTime.UtcNow;

        var project = CreateDefaultProject();

        project.CreatedAt.ShouldBeGreaterThanOrEqualTo(before);
        project.UpdatedAt.ShouldBeGreaterThanOrEqualTo(before);
    }

    [Fact]
    public void Create_Should_InitializeEmptyTaskItems()
    {
        var project = CreateDefaultProject();

        project.TaskItems.ShouldNotBeNull();
        project.TaskItems.ShouldBeEmpty();
    }

    // Update

    [Fact]
    public void Update_Should_ChangeProvidedFields()
    {
        var project = CreateDefaultProject();

        project.Update("New Name", "New Description");

        project.Name.ShouldBe("New Name");
        project.Description.ShouldBe("New Description");
    }

    [Fact]
    public void Update_WithAllNulls_Should_NotChangeAnyFields()
    {
        var project = CreateDefaultProject(name: "Original", description: "Original Desc");

        project.Update(null, null);

        project.Name.ShouldBe("Original");
        project.Description.ShouldBe("Original Desc");
    }

    [Fact]
    public void Update_WithNullName_Should_OnlyChangeDescription()
    {
        var project = CreateDefaultProject(name: "Keep", description: "Old Desc");

        project.Update(null, "New Desc");

        project.Name.ShouldBe("Keep");
        project.Description.ShouldBe("New Desc");
    }

    [Fact]
    public void Update_WithNullDescription_Should_OnlyChangeName()
    {
        var project = CreateDefaultProject(name: "Old Name", description: "Keep");

        project.Update("New Name", null);

        project.Name.ShouldBe("New Name");
        project.Description.ShouldBe("Keep");
    }

    // BaseEntity methods

    [Fact]
    public void MarkAsDeleted_Should_SetDeletedAt()
    {
        var project = CreateDefaultProject();
        project.DeletedAt.ShouldBeNull();

        project.MarkAsDeleted();

        project.DeletedAt.ShouldNotBeNull();
    }

    [Fact]
    public void MarkAsUpdated_Should_RefreshUpdatedAt()
    {
        var project = CreateDefaultProject();
        var originalUpdatedAt = project.UpdatedAt;

        project.MarkAsUpdated();

        project.UpdatedAt.ShouldBeGreaterThanOrEqualTo(originalUpdatedAt);
    }
}
