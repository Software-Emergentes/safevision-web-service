using Microsoft.EntityFrameworkCore;
using SafeVisionPlatform.Management.Domain.Model.Entities;
using SafeVisionPlatform.Management.Domain.Repositories;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeVisionPlatform.Management.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Implementación del repositorio de eventos críticos.
/// </summary>
public class CriticalEventRepositoryImpl : BaseRepository<CriticalEvent>, ICriticalEventRepository
{
    public CriticalEventRepositoryImpl(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CriticalEvent>> GetByDriverIdAsync(int driverId)
    {
        return await Context.Set<CriticalEvent>()
            .Where(e => e.DriverId == driverId)
            .OrderByDescending(e => e.OccurredAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CriticalEvent>> GetByManagerIdAsync(int managerId)
    {
        return await Context.Set<CriticalEvent>()
            .Where(e => e.ManagedByManagerId == managerId)
            .OrderByDescending(e => e.OccurredAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CriticalEvent>> GetByStatusAsync(CriticalEventStatus status)
    {
        return await Context.Set<CriticalEvent>()
            .Where(e => e.Status == status)
            .OrderByDescending(e => e.OccurredAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CriticalEvent>> GetPendingEventsAsync()
    {
        return await Context.Set<CriticalEvent>()
            .Where(e => e.Status == CriticalEventStatus.Reported ||
                       e.Status == CriticalEventStatus.InProgress)
            .OrderByDescending(e => e.OccurredAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CriticalEvent>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await Context.Set<CriticalEvent>()
            .Where(e => e.OccurredAt >= startDate && e.OccurredAt <= endDate)
            .OrderByDescending(e => e.OccurredAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(CriticalEvent criticalEvent)
    {
        Context.Set<CriticalEvent>().Update(criticalEvent);
        await Task.CompletedTask;
    }
}
