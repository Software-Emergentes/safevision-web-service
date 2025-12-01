using Microsoft.EntityFrameworkCore;
using SafeVisionPlatform.FatigueMonitoring.Domain.Model.Entities;
using SafeVisionPlatform.FatigueMonitoring.Domain.Repositories;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeVisionPlatform.FatigueMonitoring.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Implementación del repositorio de eventos de somnolencia.
/// </summary>
public class DrowsinessEventRepository : BaseRepository<DrowsinessEvent>, IDrowsinessEventRepository
{
    public DrowsinessEventRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<DrowsinessEvent>> GetByDriverIdAsync(int driverId)
    {
        return await Context.Set<DrowsinessEvent>()
            .Where(e => e.DriverId == driverId)
            .OrderByDescending(e => e.DetectedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<DrowsinessEvent>> GetByTripIdAsync(int tripId)
    {
        return await Context.Set<DrowsinessEvent>()
            .Where(e => e.TripId == tripId)
            .OrderByDescending(e => e.DetectedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<DrowsinessEvent>> GetByMonitoringSessionIdAsync(int sessionId)
    {
        return await Context.Set<DrowsinessEvent>()
            .Where(e => e.MonitoringSessionId == sessionId)
            .OrderByDescending(e => e.DetectedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<DrowsinessEvent>> GetUnprocessedEventsAsync()
    {
        return await Context.Set<DrowsinessEvent>()
            .Where(e => !e.Processed)
            .OrderBy(e => e.DetectedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<DrowsinessEvent>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await Context.Set<DrowsinessEvent>()
            .Where(e => e.DetectedAt >= startDate && e.DetectedAt <= endDate)
            .OrderByDescending(e => e.DetectedAt)
            .ToListAsync();
    }

    public async Task<Dictionary<DrowsinessEventType, int>> GetEventCountsByTypeForDriverAsync(
        int driverId,
        DateTime startDate,
        DateTime endDate)
    {
        var events = await Context.Set<DrowsinessEvent>()
            .Where(e => e.DriverId == driverId && e.DetectedAt >= startDate && e.DetectedAt <= endDate)
            .GroupBy(e => e.EventType)
            .Select(g => new { EventType = g.Key, Count = g.Count() })
            .ToListAsync();

        return events.ToDictionary(e => e.EventType, e => e.Count);
    }
}
