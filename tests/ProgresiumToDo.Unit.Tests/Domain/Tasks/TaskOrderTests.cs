using ProgresiumToDo.Domain.Tasks;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Domain.Tasks;

public class TaskOrderTests
{
    [Theory]
    [InlineData(OrderType.ByDueDate, 1.5)]
    [InlineData(OrderType.ByProject, 2)]
    [InlineData(OrderType.ByParentTask, 24)]
    public void Create_Should_SetPropertyValues(OrderType orderType, decimal orderIndex)
    {
        var taskId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var dueDate = new DateOnly(2026, 6, 15);
        var parentTaskId = Guid.NewGuid();

        var order = TaskOrder.Create(taskId, orderType, orderIndex, projectId, dueDate, parentTaskId);

        order.ShouldNotBeNull();
        order.TaskId.ShouldBe(taskId);
        order.OrderType.ShouldBe(orderType);
        order.OrderIndex.ShouldBe(orderIndex);
        order.ProjectId.ShouldBe(projectId);
        order.DueDate.ShouldBe(dueDate);
        order.ParentTaskId.ShouldBe(parentTaskId);
    }

    [Fact]
    public void Create_WithNullOptionalFields_Should_SetNulls()
    {
        var taskId = Guid.NewGuid();

        var order = TaskOrder.Create(taskId, OrderType.ByDueDate, 0m, null, null, null);

        order.ProjectId.ShouldBeNull();
        order.DueDate.ShouldBeNull();
        order.ParentTaskId.ShouldBeNull();
    }

    [Fact]
    public void Create_Should_GenerateId()
    {
        var order = TaskOrder.Create(Guid.NewGuid(), OrderType.ByDueDate, 1m, null, null, null);

        order.Id.ShouldNotBe(Guid.Empty);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(5.5)]
    [InlineData(100.25)]
    public void UpdateOrderIndex_Should_ChangeOrderIndex(double newIndex)
    {
        var order = TaskOrder.Create(Guid.NewGuid(), OrderType.ByDueDate, 1m, null, null, null);

        order.UpdateOrderIndex((decimal)newIndex);

        order.OrderIndex.ShouldBe((decimal)newIndex);
    }

    [Fact]
    public void UpdateProjectId_Should_ChangeProjectId()
    {
        var order = TaskOrder.Create(Guid.NewGuid(), OrderType.ByProject, 1m, null, null, null);
        var newProjectId = Guid.NewGuid();

        order.UpdateProjectId(newProjectId);

        order.ProjectId.ShouldBe(newProjectId);
    }

    [Fact]
    public void UpdateProjectId_WithNull_Should_ClearProjectId()
    {
        var projectId = Guid.NewGuid();
        var order = TaskOrder.Create(Guid.NewGuid(), OrderType.ByProject, 1m, projectId, null, null);

        order.UpdateProjectId(null);

        order.ProjectId.ShouldBeNull();
    }

    [Fact]
    public void UpdateDueDate_Should_ChangeDueDate()
    {
        var order = TaskOrder.Create(Guid.NewGuid(), OrderType.ByDueDate, 1m, null, null, null);
        var newDate = new DateOnly(2026, 12, 25);

        order.UpdateDueDate(newDate);

        order.DueDate.ShouldBe(newDate);
    }

    [Fact]
    public void UpdateDueDate_WithNull_Should_ClearDueDate()
    {
        var order = TaskOrder.Create(Guid.NewGuid(), OrderType.ByDueDate, 1m, null, new DateOnly(2026, 1, 1), null);

        order.UpdateDueDate(null);

        order.DueDate.ShouldBeNull();
    }

    [Fact]
    public void UpdateParentTaskId_Should_ChangeParentTaskId()
    {
        var order = TaskOrder.Create(Guid.NewGuid(), OrderType.ByParentTask, 1m, null, null, null);
        var newParentId = Guid.NewGuid();

        order.UpdateParentTaskId(newParentId);

        order.ParentTaskId.ShouldBe(newParentId);
    }

    [Fact]
    public void UpdateParentTaskId_WithNull_Should_ClearParentTaskId()
    {
        var parentId = Guid.NewGuid();
        var order = TaskOrder.Create(Guid.NewGuid(), OrderType.ByParentTask, 1m, null, null, parentId);

        order.UpdateParentTaskId(null);

        order.ParentTaskId.ShouldBeNull();
    }
}