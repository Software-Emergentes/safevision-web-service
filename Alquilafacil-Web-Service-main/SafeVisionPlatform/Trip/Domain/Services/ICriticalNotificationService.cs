using SafeVisionPlatform.Trip.Application.Internal.DTO;

namespace SafeVisionPlatform.Trip.Domain.Services;

/// <summary>
/// Servicio de dominio para gestionar notificaciones críticas a gerentes.
/// </summary>
public interface ICriticalNotificationService
{
    /// <summary>
    /// Crea y envía una notificación crítica al gerente.
    /// </summary>
    Task<NotificationResponseDTO> CreateAndSendCriticalNotificationAsync(CreateCriticalNotificationDTO notificationDto);

    /// <summary>
    /// Obtiene todas las notificaciones de un gerente.
    /// </summary>
    Task<IEnumerable<CriticalNotificationDTO>> GetManagerNotificationsAsync(int managerId);

    /// <summary>
    /// Obtiene las notificaciones pendientes de un gerente.
    /// </summary>
    Task<IEnumerable<CriticalNotificationDTO>> GetPendingManagerNotificationsAsync(int managerId);

    /// <summary>
    /// Marca una notificación como leída.
    /// </summary>
    Task MarkAsReadAsync(int notificationId);

    /// <summary>
    /// Marca una notificación como reconocida.
    /// </summary>
    Task MarkAsAcknowledgedAsync(int notificationId);

    /// <summary>
    /// Evalúa si un conductor está en estado crítico y envía notificación si es necesario.
    /// </summary>
    Task EvaluateAndNotifyIfCriticalAsync(int driverId, int tripId);

    /// <summary>
    /// Envía notificación de viaje completado de manera segura (sin alertas críticas).
    /// </summary>
    Task SendSafeTripCompletedNotificationAsync(int driverId, int tripId, int? managerId = null);
}
