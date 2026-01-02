using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Infrastructure.Interceptors;

public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAuditRules(eventData);
        return base.SavingChanges(eventData, result);
    }
    
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditRules(eventData);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    
    private void ApplyAuditRules(DbContextEventData eventData)
    {
        if (eventData.Context is null)
        {
            return;
        }

        var context = eventData.Context;
        var processedEntities = new HashSet<object>();

        // Convert all BaseEntity deletions to soft deletes
        foreach (EntityEntry<BaseEntity> entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.MarkAsDeleted();
                processedEntities.Add(entry.Entity);
            }
            else if (entry.State == EntityState.Modified && entry.Entity.DeletedAt == null)
            {
                entry.Entity.MarkAsUpdated();
            }
        }

        // Now cascade delete to related entities
        var entriesToCascade = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified && 
                        e.Entity is BaseEntity be && 
                        be.DeletedAt != null &&
                        processedEntities.Contains(e.Entity))
            .ToList();

        foreach (var entry in entriesToCascade)
        {
            CascadeDelete(context, entry, processedEntities);
        }
    }

    private void CascadeDelete(DbContext context, EntityEntry entry, HashSet<object> processedEntities)
    {
        var navigations = entry.Metadata.GetNavigations()
            .Where(n => !n.IsOnDependent && n.ForeignKey.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var navigation in navigations)
        {
            if (navigation.IsCollection)
            {
                if (!entry.Collection(navigation.Name).IsLoaded)
                {
                    entry.Collection(navigation.Name).Load();
                }
                
                var relatedEntities = entry.Collection(navigation.Name).CurrentValue;
                if (relatedEntities != null)
                {
                    foreach (var relatedEntity in relatedEntities.Cast<object>().ToList())
                    {
                        if (!processedEntities.Contains(relatedEntity))
                        {
                            processedEntities.Add(relatedEntity);
                            DeleteEntity(context, relatedEntity, processedEntities);
                        }
                    }
                }
            }
            else
            {
                if (!entry.Reference(navigation.Name).IsLoaded)
                {
                    entry.Reference(navigation.Name).Load();
                }
                
                var relatedEntity = entry.Reference(navigation.Name).CurrentValue;
                if (relatedEntity != null && !processedEntities.Contains(relatedEntity))
                {
                    processedEntities.Add(relatedEntity);
                    DeleteEntity(context, relatedEntity, processedEntities);
                }
            }
        }
    }
    
    private void DeleteEntity(DbContext context, object entity, HashSet<object> processedEntities)
    {
        var entityEntry = context.Entry(entity);
        
        if (entity is BaseEntity baseEntity)
        {
            // Soft delete
            if (baseEntity.DeletedAt == null)
            {
                baseEntity.MarkAsDeleted();
                if (entityEntry.State == EntityState.Unchanged)
                {
                    entityEntry.State = EntityState.Modified;
                }
                // Continue cascading
                CascadeDelete(context, entityEntry, processedEntities);
            }
        }
        else
        {
            // Hard delete
            entityEntry.State = EntityState.Deleted;
            // EF Core will handle its cascades automatically
        }
    }
}