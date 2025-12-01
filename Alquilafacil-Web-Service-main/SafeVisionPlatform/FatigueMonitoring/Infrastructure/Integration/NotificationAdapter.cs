using SafeVisionPlatform.FatigueMonitoring.Domain.Model.Entities;

namespace SafeVisionPlatform.FatigueMonitoring.Infrastructure.Integration;

/// <summary>
/// Adaptador para enviar alertas a sistemas externos de notificación.
/// </summary>
public class NotificationAdapter
{
    private readonly ILogger<NotificationAdapter> _logger;

    public NotificationAdapter(ILogger<NotificationAdapter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Envía una notificación de alerta crítica.
    /// </summary>
    public async Task SendCriticalAlertNotificationAsync(CriticalAlert alert)
    {
        // Simular envío de notificación
        _logger.LogInformation(
            $"[NotificationAdapter] Enviando notificación de alerta crítica: " +
            $"AlertId={alert.Id}, DriverId={alert.DriverId}, Channel={alert.NotificationChannel}");

        // En producción, aquí iría la integración con servicios externos
        // como Firebase Cloud Messaging, AWS SNS, SendGrid, etc.

        await Task.Delay(100); // Simular latencia de red

        _logger.LogInformation($"[NotificationAdapter] Notificación enviada exitosamente para alerta {alert.Id}");
    }

    /// <summary>
    /// Envía una notificación push al dispositivo del conductor.
    /// </summary>
    public async Task SendPushNotificationToDriverAsync(int driverId, string message)
    {
        _logger.LogInformation($"[NotificationAdapter] Push notification para conductor {driverId}: {message}");
        await Task.Delay(50);
    }

    /// <summary>
    /// Envía una notificación al gerente.
    /// </summary>
    public async Task SendNotificationToManagerAsync(int managerId, string message, string severity)
    {
        _logger.LogInformation(
            $"[NotificationAdapter] Notificación para gerente {managerId} [{severity}]: {message}");
        await Task.Delay(50);
    }
}
