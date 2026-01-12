using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Infrastructure.Configurations.Billing;

internal sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.StartDate)
            .IsRequired();

        builder.Property(s => s.EndDate)
            .IsRequired();

        builder.Property(s => s.IsAutoRenew);

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.HasIndex(s => s.UserId)
            .IsUnique()
            .HasFilter("\"status\" = 'Active'");

        builder.HasOne(s => s.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.PlanPricing)
            .WithMany(pp => pp.Subscriptions)
            .HasForeignKey(s => s.PlanPricingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
