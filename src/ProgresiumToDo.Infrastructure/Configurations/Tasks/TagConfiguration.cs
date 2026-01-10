using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgresiumToDo.Domain.Tags;

namespace ProgresiumToDo.Infrastructure.Configurations.Tasks;

internal sealed class TagConfiguration : SoftDeleteEntityConfiguration<Tag>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");
        
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired();

        builder.Property(t => t.Color)
            .IsRequired();

        builder.Property(t => t.ProjectId)
            .IsRequired();
        
        builder.HasIndex(t => new { t.ProjectId, t.Name })
            .IsUnique()
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tags)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasMany(t => t.TaskItems)
            .WithMany(tk => tk.Tags)
            .UsingEntity(j => j.ToTable("task_item_tags"));
    }
}