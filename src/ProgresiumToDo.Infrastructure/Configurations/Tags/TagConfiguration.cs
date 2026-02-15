using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgresiumToDo.Domain.Tags;

namespace ProgresiumToDo.Infrastructure.Configurations.Tags;

internal sealed class TagConfiguration : SoftDeleteEntityConfiguration<Tag>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");
        
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired();

        builder.HasIndex(t => t.Name)
            .IsUnique()
            .HasFilter("\"deleted_at\" IS NULL");
        
        builder.Property(t => t.UserId)
            .IsRequired();
        
        builder
            .HasOne(t => t.User)
            .WithMany(u => u.Tags)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasMany(t => t.TaskItems)
            .WithMany(tk => tk.Tags)
            .UsingEntity(j => j.ToTable("task_item_tags"));
    }
}