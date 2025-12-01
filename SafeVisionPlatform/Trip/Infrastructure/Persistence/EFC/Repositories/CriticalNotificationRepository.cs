using Microsoft.EntityFrameworkCore;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;
using SafeVisionPlatform.Trip.Domain.Model.Entities;
using SafeVisionPlatform.Trip.Domain.Repositories;

namespace SafeVisionPlatform.Trip.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Implementación del repositorio de notificaciones críticas.
/// </summary>
public class CriticalNotificationRepository : BaseRepository<CriticalNotification>, ICriticalNotificationRepository
{
    public CriticalNotificationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CriticalNotification>> GetNotificationsByManagerIdAsync(int managerId)
    {
        return await Context.Set<CriticalNotification>()
            .Where(n => n.ManagerId == managerId)
            .OrderByDescending(n => n.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<CriticalNotification>> GetPendingNotificationsByManagerIdAsync(int managerId)
    {
        return await Context.Set<CriticalNotification>()
            .Where(n => n.ManagerId == managerId && (n.Status == "Pending" || n.Status == "Sent"))
            .OrderByDescending(n => n.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<CriticalNotification>> GetNotificationsByDriverIdAsync(int driverId)
    {
        return await Context.Set<CriticalNotification>()
            .Where(n => n.DriverId == driverId)
            .OrderByDescending(n => n.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<CriticalNotification>> GetNotificationsByTripIdAsync(int tripId)
    {
        return await Context.Set<CriticalNotification>()
            .Where(n => n.TripId == tripId)
            .OrderByDescending(n => n.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<CriticalNotification>> GetUnacknowledgedNotificationsAsync()
    {
        return await Context.Set<CriticalNotification>()
            .Where(n => n.Status != "Acknowledged")
            .OrderByDescending(n => n.Timestamp)
            .ToListAsync();
    }

    public async Task UpdateAsync(CriticalNotification notification)
    {
        Context.Set<CriticalNotification>().Update(notification);
        await Task.CompletedTask;
    }
}
