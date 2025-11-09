using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProgresiumToDo.Infrastructure.Configurations.FeatureUsage;

internal sealed class FeatureUsageConfiguration : IEntityTypeConfiguration<Domain.FeatureUsage.FeatureUsage>
{
    public void Configure(EntityTypeBuilder<Domain.FeatureUsage.FeatureUsage> builder)
    {
        builder.ToTable("feature_usages");

        builder.HasKey(fu => fu.Id);

        builder.Property(fu => fu.UserId)
            .IsRequired();

        builder.Property(fu => fu.FeatureId)
            .IsRequired();

        builder.Property(fu => fu.UsageDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(fu => fu.UsageCount)
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasIndex(fu => new { fu.UserId, fu.FeatureId, fu.UsageDate })
            .IsUnique();

        builder.HasOne(fu => fu.User)
            .WithMany(u => u.FeatureUsages)
            .HasForeignKey(fu => fu.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fu => fu.Feature)
            .WithMany(f => f.FeatureUsages)
            .HasForeignKey(fu => fu.FeatureId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
