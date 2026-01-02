using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Infrastructure.Configurations.Tasks;

internal sealed class TaskOrderConfiguration : IEntityTypeConfiguration<TaskOrder>
{
    public void Configure(EntityTypeBuilder<TaskOrder> builder)
    {
        builder.HasKey(to => to.Id);

        builder.Property(to => to.TaskId)
            .IsRequired();
        
        builder.Property(to => to.OrderType)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(to => to.OrderIndex)
            .IsRequired();

        builder.Property(to => to.ProjectId)
            .IsRequired(false);
        
        builder.Property(to => to.DueDate)
            .HasColumnType("date")
            .IsRequired(false);
        
        builder.Property(to => to.ParentTaskId)
            .IsRequired(false);
        
        builder.HasOne(to => to.TaskItem)
            .WithMany(t => t.TaskOrders)
            .HasForeignKey(to => to.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(to => to.Project)
            .WithMany()
            .HasForeignKey(to => to.ProjectId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne<TaskItem>()
            .WithMany()
            .HasForeignKey(to => to.ParentTaskId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}