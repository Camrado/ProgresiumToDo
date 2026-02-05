using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgresiumToDo.Domain.Waitlist;

namespace ProgresiumToDo.Infrastructure.Configurations.Waitlist;

internal sealed class WaitlistEntryConfiguration : IEntityTypeConfiguration<WaitlistEntry>
{
    public void Configure(EntityTypeBuilder<WaitlistEntry> builder)
    {
        builder.ToTable("waitlist_entries");
        
        builder.HasKey(wle => wle.Id);

        builder.Property(wle => wle.Email)
            .IsRequired();
        
        builder.HasIndex(wle => wle.Email)
            .IsUnique();

        builder.Property(wle => wle.CreatedAt)
            .IsRequired();
    }
}