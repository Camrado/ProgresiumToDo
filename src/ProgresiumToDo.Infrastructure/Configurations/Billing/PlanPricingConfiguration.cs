using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Infrastructure.Configurations.Billing;

internal sealed class PlanPricingConfiguration : SoftDeleteEntityConfiguration<PlanPricing>
{
    protected override void ConfigureEntity(EntityTypeBuilder<PlanPricing> builder)
    {
        builder.ToTable("plan_pricings");

        builder.HasKey(pp => pp.Id);

        builder.Property(pp => pp.Price)
            .IsRequired()
            .HasColumnType("numeric(10,2)");

        builder.Property(pp => pp.BillingPeriod)
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne(pp => pp.Plan)
            .WithMany(p => p.PlanPricings)
            .HasForeignKey(pp => pp.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pp => pp.Region)
            .WithMany(r => r.PlanPricings)
            .HasForeignKey(pp => pp.RegionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
