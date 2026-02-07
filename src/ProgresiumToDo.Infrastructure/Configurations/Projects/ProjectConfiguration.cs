using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgresiumToDo.Domain.Projects;

namespace ProgresiumToDo.Infrastructure.Configurations.Projects;

internal sealed class ProjectConfiguration : SoftDeleteEntityConfiguration<Project>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");
        
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired();

        builder.Property(p => p.Description)
            .IsRequired(false);

        builder.Property(p => p.UserId)
            .IsRequired();
        
        builder.HasIndex(p => new { p.UserId, p.Name })
            .IsUnique()
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasOne(p => p.User)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
