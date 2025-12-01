using Microsoft.EntityFrameworkCore;
using SafeVisionPlatform.FatigueMonitoring.Domain.Model.Entities;
using SafeVisionPlatform.FatigueMonitoring.Domain.Repositories;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeVisionPlatform.FatigueMonitoring.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Implementación del repositorio de alertas críticas de fatiga.
/// </summary>
public class FatigueCriticalAlertRepository : BaseRepository<CriticalAlert>, ICriticalAlertRepository
{
    public FatigueCriticalAlertRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CriticalAlert>> GetByDriverIdAsync(int driverId)
    {
        return await Context.Set<CriticalAlert>()
            .Where(a => a.DriverId == driverId)
            .OrderByDescending(a => a.GeneratedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CriticalAlert>> GetByTripIdAsync(int tripId)
    {
        return await Context.Set<CriticalAlert>()
            .Where(a => a.TripId == tripId)
            .OrderByDescending(a => a.GeneratedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CriticalAlert>> GetByManagerIdAsync(int managerId)
    {
        return await Context.Set<CriticalAlert>()
            .Where(a => a.ManagerId == managerId)
            .OrderByDescending(a => a.GeneratedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CriticalAlert>> GetPendingAlertsAsync()
    {
        return await Context.Set<CriticalAlert>()
            .Where(a => a.Status == CriticalAlertStatus.Pending || a.Status == CriticalAlertStatus.Sent)
            .OrderByDescending(a => a.GeneratedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CriticalAlert>> GetPendingAlertsByManagerIdAsync(int managerId)
    {
        return await Context.Set<CriticalAlert>()
            .Where(a => a.ManagerId == managerId &&
                       (a.Status == CriticalAlertStatus.Pending || a.Status == CriticalAlertStatus.Sent))
            .OrderByDescending(a => a.GeneratedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CriticalAlert>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await Context.Set<CriticalAlert>()
            .Where(a => a.GeneratedAt >= startDate && a.GeneratedAt <= endDate)
            .OrderByDescending(a => a.GeneratedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(CriticalAlert alert)
    {
        Context.Set<CriticalAlert>().Update(alert);
        await Task.CompletedTask;
    }
}
