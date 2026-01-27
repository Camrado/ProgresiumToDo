using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgresiumToDo.Domain.Auth;
using ProgresiumToDo.Infrastructure.Auth.Identity;

namespace ProgresiumToDo.Infrastructure.Configurations.Auth;

internal sealed class UserConfiguration : SoftDeleteEntityConfiguration<User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Email)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("\"deleted_at\" IS NULL");

        builder.Property(u => u.FirstName)
            .IsRequired();

        builder.Property(u => u.LastName)
            .IsRequired();

        builder.Property(u => u.IsEmailVerified)
            .HasDefaultValue(false);

        builder.HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<User>(u => u.ApplicationUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}