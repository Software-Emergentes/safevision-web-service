using SafeVisionPlatform.Shared.Domain.Repositories;
using SafeVisionPlatform.Trip.Domain.Model.Entities;

namespace SafeVisionPlatform.Trip.Domain.Repositories;

/// <summary>
/// Repositorio para gestionar notificaciones críticas.
/// </summary>
public interface ICriticalNotificationRepository : IBaseRepository<CriticalNotification>
{
    /// <summary>
    /// Obtiene las notificaciones de un gerente específico.
    /// </summary>
    Task<IEnumerable<CriticalNotification>> GetNotificationsByManagerIdAsync(int managerId);

    /// <summary>
    /// Obtiene las notificaciones pendientes de un gerente.
    /// </summary>
    Task<IEnumerable<CriticalNotification>> GetPendingNotificationsByManagerIdAsync(int managerId);

    /// <summary>
    /// Obtiene las notificaciones de un conductor.
    /// </summary>
    Task<IEnumerable<CriticalNotification>> GetNotificationsByDriverIdAsync(int driverId);

    /// <summary>
    /// Obtiene las notificaciones de un viaje específico.
    /// </summary>
    Task<IEnumerable<CriticalNotification>> GetNotificationsByTripIdAsync(int tripId);

    /// <summary>
    /// Obtiene las notificaciones críticas (no leídas o no reconocidas).
    /// </summary>
    Task<IEnumerable<CriticalNotification>> GetUnacknowledgedNotificationsAsync();

    /// <summary>
    /// Actualiza una notificación crítica.
    /// </summary>
    Task UpdateAsync(CriticalNotification notification);
}
