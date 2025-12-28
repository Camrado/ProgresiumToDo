using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Infrastructure.Configurations.Tasks;

internal sealed class TaskItemConfiguration : SoftDeleteEntityConfiguration<TaskItem>
{
    protected override void ConfigureEntity(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("task_items");
        
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired();

        builder.Property(t => t.Description)
            .IsRequired(false);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(t => t.DueDate)
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(t => t.Duration)
            .IsRequired(false);

        builder.Property(t => t.StartTime)
            .IsRequired(false);

        builder.Property(t => t.EndTime)
            .IsRequired(false);

        builder.Property(t => t.ClosedAt)
            .IsRequired(false);

        builder.Property(t => t.UserId)
            .IsRequired();

        builder.Property(t => t.ProjectId)
            .IsRequired();

        builder.Property(t => t.ParentTaskItemId)
            .IsRequired(false);

        // Relationships
        builder.HasOne(t => t.User)
            .WithMany(u => u.TaskItems)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(t => t.Project)
            .WithMany(p => p.TaskItems)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(t => t.ParentTaskItem)
            .WithMany(t => t.SubTaskItems)
            .HasForeignKey(t => t.ParentTaskItemId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
