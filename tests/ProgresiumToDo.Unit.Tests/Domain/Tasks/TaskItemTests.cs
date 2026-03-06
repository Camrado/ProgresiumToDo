using ProgresiumToDo.Domain.Tags;
using ProgresiumToDo.Domain.Tasks;
using Shouldly;
using TaskStatus = ProgresiumToDo.Domain.Tasks.TaskStatus;

namespace ProgresiumToDo.Unit.Tests.Domain.Tasks;

public class TaskItemTests
{
    private static TaskItem CreateDefaultTask(
        Guid? projectId = null,
        string title = "Test Task",
        string? description = "Test Description",
        string? status = null,
        string? priority = null,
        DateOnly? dueDate = null,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null)
    {
        return TaskItem.Create(
            projectId,
            Guid.NewGuid(),
            title,
            description,
            status,
            priority,
            dueDate,
            startTime,
            endTime);
    }
    
    // Create

    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dueDate = new DateOnly(2026, 6, 15);
        var startTime = new TimeOnly(9, 0);
        var endTime = new TimeOnly(17, 0);

        var task = TaskItem.Create(
            projectId, userId, "My Task", "My Description",
            "InProgress", "High", dueDate, startTime, endTime);

        task.ShouldNotBeNull();
        task.ProjectId.ShouldBe(projectId);
        task.UserId.ShouldBe(userId);
        task.Title.ShouldBe("My Task");
        task.Description.ShouldBe("My Description");
        task.Status.ShouldBe(TaskStatus.InProgress);
        task.Priority.ShouldBe(Priority.High);
        task.DueDate.ShouldBe(dueDate);
        task.StartTime.ShouldBe(startTime);
        task.EndTime.ShouldBe(endTime);
    }

    [Fact]
    public void Create_WithNullStatus_Should_DefaultToPending()
    {
        var task = CreateDefaultTask(status: null);

        task.Status.ShouldBe(TaskStatus.Pending);
    }

    [Fact]
    public void Create_WithEmptyStatus_Should_DefaultToPending()
    {
        var task = CreateDefaultTask(status: "");

        task.Status.ShouldBe(TaskStatus.Pending);
    }

    [Fact]
    public void Create_WithNullPriority_Should_DefaultToNone()
    {
        var task = CreateDefaultTask(priority: null);

        task.Priority.ShouldBe(Priority.None);
    }

    [Fact]
    public void Create_WithEmptyPriority_Should_DefaultToNone()
    {
        var task = CreateDefaultTask(priority: "");

        task.Priority.ShouldBe(Priority.None);
    }

    [Theory]
    [InlineData("pending", TaskStatus.Pending)]
    [InlineData("InProgress", TaskStatus.InProgress)]
    [InlineData("COMPLETED", TaskStatus.Completed)]
    [InlineData("cancelled", TaskStatus.Cancelled)]
    public void Create_Should_ParseStatusCaseInsensitively(string input, TaskStatus expected)
    {
        var task = CreateDefaultTask(status: input);

        task.Status.ShouldBe(expected);
    }

    [Theory]
    [InlineData("none", Priority.None)]
    [InlineData("Low", Priority.Low)]
    [InlineData("MEDIUM", Priority.Medium)]
    [InlineData("high", Priority.High)]
    public void Create_Should_ParsePriorityCaseInsensitively(string input, Priority expected)
    {
        var task = CreateDefaultTask(priority: input);

        task.Priority.ShouldBe(expected);
    }

    [Fact]
    public void Create_WithNullProjectId_Should_SetNullProjectId()
    {
        var task = CreateDefaultTask(projectId: null);

        task.ProjectId.ShouldBeNull();
    }

    [Fact]
    public void Create_Should_InitializeEmptyCollections()
    {
        var task = CreateDefaultTask();

        task.SubTaskItems.ShouldNotBeNull();
        task.SubTaskItems.ShouldBeEmpty();
        task.Tags.ShouldNotBeNull();
        task.Tags.ShouldBeEmpty();
        task.TaskAttachments.ShouldNotBeNull();
        task.TaskAttachments.ShouldBeEmpty();
        task.TaskOrders.ShouldNotBeNull();
        task.TaskOrders.ShouldBeEmpty();
    }
    
    // Create Subtask

    [Fact]
    public void CreateSubtask_Should_SetPropertyValues()
    {
        var userId = Guid.NewGuid();
        var parentTaskId = Guid.NewGuid();
        var startTime = new TimeOnly(10, 0);
        var endTime = new TimeOnly(12, 0);

        var subtask = TaskItem.CreateSubtask(
            userId, parentTaskId, "Subtask Title", "Subtask Desc",
            startTime, endTime, "Medium", "InProgress");

        subtask.ShouldNotBeNull();
        subtask.UserId.ShouldBe(userId);
        subtask.ParentTaskItemId.ShouldBe(parentTaskId);
        subtask.Title.ShouldBe("Subtask Title");
        subtask.Description.ShouldBe("Subtask Desc");
        subtask.StartTime.ShouldBe(startTime);
        subtask.EndTime.ShouldBe(endTime);
        subtask.Priority.ShouldBe(Priority.Medium);
        subtask.Status.ShouldBe(TaskStatus.InProgress);
    }

    [Fact]
    public void CreateSubtask_WithNullOptionalStrings_Should_UseDefaults()
    {
        var subtask = TaskItem.CreateSubtask(
            Guid.NewGuid(), Guid.NewGuid(), "Title", null, null, null, null, null);

        subtask.Priority.ShouldBe(Priority.None);
        subtask.Status.ShouldBe(TaskStatus.Pending);
        subtask.Description.ShouldBeNull();
        subtask.StartTime.ShouldBeNull();
        subtask.EndTime.ShouldBeNull();
    }

    // Update

    [Fact]
    public void Update_Should_ChangeProvidedFields()
    {
        var task = CreateDefaultTask();
        var newDueDate = new DateOnly(2026, 12, 31);
        var newStartTime = new TimeOnly(8, 0);
        var newEndTime = new TimeOnly(16, 0);
        var newProjectId = Guid.NewGuid();

        task.Update("New Title", "New Desc", "Completed", "High",
            newDueDate, newStartTime, newEndTime, newProjectId);

        task.Title.ShouldBe("New Title");
        task.Description.ShouldBe("New Desc");
        task.Status.ShouldBe(TaskStatus.Completed);
        task.Priority.ShouldBe(Priority.High);
        task.DueDate.ShouldBe(newDueDate);
        task.StartTime.ShouldBe(newStartTime);
        task.EndTime.ShouldBe(newEndTime);
        task.ProjectId.ShouldBe(newProjectId);
    }

    [Fact]
    public void Update_WithAllNulls_Should_NotChangeAnyFields()
    {
        var task = CreateDefaultTask(
            title: "Original",
            description: "Original Desc",
            status: "Pending",
            priority: "Low");

        task.Update(null, null, null, null, null, null, null, null);

        task.Title.ShouldBe("Original");
        task.Description.ShouldBe("Original Desc");
        task.Status.ShouldBe(TaskStatus.Pending);
        task.Priority.ShouldBe(Priority.Low);
    }

    [Fact]
    public void Update_StatusToCompleted_Should_SetClosedAt()
    {
        var task = CreateDefaultTask();

        task.Update(null, null, "Completed", null, null, null, null, null);

        task.Status.ShouldBe(TaskStatus.Completed);
        task.ClosedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Update_StatusToCancelled_Should_SetClosedAt()
    {
        var task = CreateDefaultTask();

        task.Update(null, null, "Cancelled", null, null, null, null, null);

        task.Status.ShouldBe(TaskStatus.Cancelled);
        task.ClosedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Update_StatusBackToPending_Should_ClearClosedAt()
    {
        var task = CreateDefaultTask();
        task.Update(null, null, "Completed", null, null, null, null, null);
        task.ClosedAt.ShouldNotBeNull();

        task.Update(null, null, "Pending", null, null, null, null, null);

        task.ClosedAt.ShouldBeNull();
    }
    
    // Update Subtask
    
    [Fact]
    public void UpdateSubtask_Should_ChangeProvidedFields()
    {
        var subtask = TaskItem.CreateSubtask(
            Guid.NewGuid(), Guid.NewGuid(), "Original", null, null, null, null, null);
        var newStart = new TimeOnly(14, 0);
        var newEnd = new TimeOnly(15, 30);

        subtask.UpdateSubtask("Updated", "Updated Desc", newStart, newEnd, "High", "InProgress");

        subtask.Title.ShouldBe("Updated");
        subtask.Description.ShouldBe("Updated Desc");
        subtask.StartTime.ShouldBe(newStart);
        subtask.EndTime.ShouldBe(newEnd);
        subtask.Priority.ShouldBe(Priority.High);
        subtask.Status.ShouldBe(TaskStatus.InProgress);
    }

    [Fact]
    public void UpdateSubtask_WithAllNulls_Should_NotChangeAnyFields()
    {
        var subtask = TaskItem.CreateSubtask(
            Guid.NewGuid(), Guid.NewGuid(), "Keep", "Keep Desc",
            new TimeOnly(9, 0), new TimeOnly(10, 0), "Low", "Pending");

        subtask.UpdateSubtask(null, null, null, null, null, null);

        subtask.Title.ShouldBe("Keep");
        subtask.Description.ShouldBe("Keep Desc");
        subtask.StartTime.ShouldBe(new TimeOnly(9, 0));
        subtask.EndTime.ShouldBe(new TimeOnly(10, 0));
        subtask.Priority.ShouldBe(Priority.Low);
        subtask.Status.ShouldBe(TaskStatus.Pending);
    }

    [Fact]
    public void UpdateSubtask_StatusToCompleted_Should_SetClosedAt()
    {
        var subtask = TaskItem.CreateSubtask(
            Guid.NewGuid(), Guid.NewGuid(), "Title", null, null, null, null, null);

        subtask.UpdateSubtask(null, null, null, null, null, "Completed");

        subtask.ClosedAt.ShouldNotBeNull();
    }

    // Tags

    [Fact]
    public void AddTagIfNotExists_Should_AddTag()
    {
        var task = CreateDefaultTask();
        var tag = Tag.Create("Urgent", Guid.NewGuid());

        task.AddTagIfNotExists(tag);

        task.Tags.Count.ShouldBe(1);
        task.Tags.ShouldContain(tag);
    }

    [Fact]
    public void AddTagIfNotExists_WithDuplicateTag_Should_NotAddAgain()
    {
        var task = CreateDefaultTask();
        var tag = Tag.Create("Urgent", Guid.NewGuid());

        task.AddTagIfNotExists(tag);
        task.AddTagIfNotExists(tag);

        task.Tags.Count.ShouldBe(1);
    }

    [Fact]
    public void RemoveTagIfExists_Should_RemoveTag()
    {
        var task = CreateDefaultTask();
        var tag = Tag.Create("Urgent", Guid.NewGuid());
        task.AddTagIfNotExists(tag);

        task.RemoveTagIfExists(tag);

        task.Tags.ShouldBeEmpty();
    }

    [Fact]
    public void RemoveTagIfExists_WithNonExistingTag_Should_NotThrow()
    {
        var task = CreateDefaultTask();
        var tag = Tag.Create("Urgent", Guid.NewGuid());

        Should.NotThrow(() => task.RemoveTagIfExists(tag));
        task.Tags.ShouldBeEmpty();
    }

    [Fact]
    public void SetTags_Should_ReplaceAllTags()
    {
        var task = CreateDefaultTask();
        var userId = Guid.NewGuid();
        var oldTag = Tag.Create("Old", userId);
        task.AddTagIfNotExists(oldTag);

        var newTags = new List<Tag>
        {
            Tag.Create("New1", userId),
            Tag.Create("New2", userId)
        };

        task.SetTags(newTags);

        task.Tags.Count.ShouldBe(2);
        task.Tags.ShouldNotContain(oldTag);
    }

    [Fact]
    public void Create_Should_GenerateId()
    {
        var task = CreateDefaultTask();

        task.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Create_Should_SetCreatedAtAndUpdatedAt()
    {
        var before = DateTime.UtcNow;

        var task = CreateDefaultTask();

        task.CreatedAt.ShouldBeGreaterThanOrEqualTo(before);
        task.UpdatedAt.ShouldBeGreaterThanOrEqualTo(before);
    }

    [Fact]
    public void MarkAsDeleted_Should_SetDeletedAt()
    {
        var task = CreateDefaultTask();
        task.DeletedAt.ShouldBeNull();

        task.MarkAsDeleted();

        task.DeletedAt.ShouldNotBeNull();
    }

    [Fact]
    public void MarkAsUpdated_Should_RefreshUpdatedAt()
    {
        var task = CreateDefaultTask();
        var originalUpdatedAt = task.UpdatedAt;

        task.MarkAsUpdated();

        task.UpdatedAt.ShouldBeGreaterThanOrEqualTo(originalUpdatedAt);
    }
}