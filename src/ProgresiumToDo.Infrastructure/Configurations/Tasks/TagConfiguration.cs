using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgresiumToDo.Domain.Tasks;
using Task = System.Threading.Tasks.Task;

namespace ProgresiumToDo.Infrastructure.Configurations.Tasks;

internal sealed class TagConfiguration : SoftDeleteEntityConfiguration<Tag>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");

        builder.Property(t => t.Name)
            .IsRequired();

        builder.Property(t => t.Color)
            .IsRequired();

        builder.Property(t => t.ProjectId)
            .IsRequired();

        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tags)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasMany(t => t.TaskItems)
            .WithMany(tk => tk.Tags)
            .UsingEntity(j => j.ToTable("task_item_tags"));
    }
}