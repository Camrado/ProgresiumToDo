using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgresiumToDo.Domain.Auth;
using ProgresiumToDo.Infrastructure.Services.Auth.Identity;

namespace ProgresiumToDo.Infrastructure.Configurations.Auth;

internal sealed class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
{
    public void Configure(EntityTypeBuilder<VerificationCode> builder)
    {
        builder.ToTable("verification_codes");

        builder.HasKey(vc => vc.Id);

        builder.Property(vc => vc.CodeHash)
            .IsRequired();

        builder.Property(vc => vc.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(vc => vc.ExpiresOn)
            .IsRequired();

        builder.Property(vc => vc.CreatedAt)
            .IsRequired();

        builder.Ignore(vc => vc.IsExpired);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(vc => vc.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
