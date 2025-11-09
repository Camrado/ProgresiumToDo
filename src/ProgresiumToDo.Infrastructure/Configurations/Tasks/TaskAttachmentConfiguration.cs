using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Infrastructure.Configurations.Tasks;

internal sealed class TaskAttachmentConfiguration : IEntityTypeConfiguration<TaskAttachment>
{
    public void Configure(EntityTypeBuilder<TaskAttachment> builder)
    {
        builder.ToTable("task_attachments");

        builder.HasKey(ta => ta.Id);

        builder.Property(ta => ta.FileUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(ta => ta.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ta => ta.UploadedAt)
            .IsRequired();

        builder.Property(ta => ta.TaskId)
            .IsRequired();

        builder.HasOne(ta => ta.TaskItem)
            .WithMany(t => t.TaskAttachments)
            .HasForeignKey(ta => ta.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
