using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Infrastructure.Configurations;

public abstract class SoftDeleteEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> 
    where TEntity : BaseEntity
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasQueryFilter(e => e.DeletedAt == null);
        
        builder.HasKey(e => e.Id);

        builder.Property(e => e.DeletedAt)
            .IsRequired(false);

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        ConfigureEntity(builder);
    }
    
    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}