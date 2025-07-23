using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Versus.Api.Entities;

namespace Versus.Api.Data.Interceptors;

public class AuditInterceptor(ICurrentSessionProvider currentSessionProvider) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = new())
    {
        if (eventData.Context is null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        Guid? userId = currentSessionProvider.UserId;

        var entries = eventData.Context.ChangeTracker.Entries()
            .Where(x => x is { Entity: IAuditableEntity, State: EntityState.Added or EntityState.Modified })
            .ToArray();
        foreach (var entry in entries)
        {
            var auditableEntity = (IAuditableEntity)entry.Entity;
            switch (entry.State)
            {
                case EntityState.Added:
                    auditableEntity.CreatedBy = userId;
                    auditableEntity.CreatedDate = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    auditableEntity.UpdatedBy = userId;
                    auditableEntity.UpdatedDate = DateTime.UtcNow;
                    break;
            }
        }

        var auditEntries = eventData.Context.ChangeTracker.Entries()
            .Where(x => x.Entity is not AuditEntry
                        && x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(x => CreateAuditEntry(x, userId))
            .ToArray();
        if (auditEntries.Length == 0)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        await eventData.Context.Set<AuditEntry>().AddRangeAsync(auditEntries, cancellationToken);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static AuditEntry CreateAuditEntry(EntityEntry entry, Guid? userId)
    {
        var entityState = entry.State switch
        {
            EntityState.Deleted => AuditLogChangeType.Delete,
            EntityState.Modified => AuditLogChangeType.Update,
            EntityState.Added => AuditLogChangeType.Insert,
            _ => throw new InvalidOperationException()
        };

        return new AuditEntry
        {
            EntityName = entry.Entity.GetType().Name,
            EntityId = entry.Properties.First(x => x.Metadata.IsPrimaryKey()).CurrentValue?.ToString()
                       ?? string.Empty,
            ChangeType = entityState,
            ChangedValues = entry.State == EntityState.Deleted
                ? JsonSerializer.Serialize(
                    entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p.Name]))
                : JsonSerializer.Serialize(
                    entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p.Name])),
            ChangedBy = userId,
            ChangeDate = DateTime.UtcNow
        };
    }
}