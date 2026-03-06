using ProgresiumToDo.Domain.Tasks;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Domain.Tasks;

public class TaskAttachmentTests
{
    [Theory]
    [InlineData("https://storage.example.com/file.pdf", "file.pdf")]
    [InlineData("https://cdn.example.com/image.png", "image.png")]
    public void Create_Should_SetPropertyValues(string fileUrl, string fileName)
    {
        var taskId = Guid.NewGuid();

        var attachment = TaskAttachment.Create(fileUrl, fileName, taskId);

        attachment.ShouldNotBeNull();
        attachment.FileUrl.ShouldBe(fileUrl);
        attachment.FileName.ShouldBe(fileName);
        attachment.TaskId.ShouldBe(taskId);
    }
    
    [Fact]
    public void Create_Should_GenerateId()
    {
        var attachment = TaskAttachment.Create("https://example.com/file.pdf", "file.pdf", Guid.NewGuid());

        attachment.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Create_Should_SetUploadedAt()
    {
        var before = DateTime.UtcNow;

        var attachment = TaskAttachment.Create("https://example.com/file.pdf", "file.pdf", Guid.NewGuid());

        attachment.UploadedAt.ShouldBeGreaterThanOrEqualTo(before);
        attachment.UploadedAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);
    }
}