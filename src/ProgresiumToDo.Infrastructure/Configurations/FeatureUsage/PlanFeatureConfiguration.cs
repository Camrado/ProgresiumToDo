using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgresiumToDo.Domain.FeatureUsage;

namespace ProgresiumToDo.Infrastructure.Configurations.FeatureUsage;

internal sealed class PlanFeaturesConfiguration : IEntityTypeConfiguration<PlanFeature>
{
    public void Configure(EntityTypeBuilder<PlanFeature> builder)
    {
        builder.ToTable("plan_features");

        builder.HasKey(pf => new { pf.PlanId, pf.FeatureId });

        builder.Property(pf => pf.PlanId)
            .IsRequired();

        builder.Property(pf => pf.FeatureId)
            .IsRequired();

        builder.Property(pf => pf.DailyLimit)
            .IsRequired();

        builder.Property(pf => pf.MonthlyLimit)
            .IsRequired();

        builder.HasOne(pf => pf.Plan)
            .WithMany(p => p.PlanFeatures)
            .HasForeignKey(pf => pf.PlanId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(pf => pf.Feature)
            .WithMany(f => f.PlanFeatures)
            .HasForeignKey(pf => pf.FeatureId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
