using SafeVisionPlatform.FatigueMonitoring.Domain.Model.Entities;

namespace SafeVisionPlatform.FatigueMonitoring.Domain.Services;

/// <summary>
/// Servicio de dominio encargado de generar alertas críticas
/// a partir de los eventos de somnolencia acumulados.
/// </summary>
public interface IAlertGenerationService
{
    /// <summary>
    /// Genera una alerta crítica basada en los eventos de somnolencia.
    /// </summary>
    Task<CriticalAlert> GenerateAlertAsync(
        int driverId,
        int tripId,
        int? managerId,
        IEnumerable<DrowsinessEvent> drowsinessEvents);

    /// <summary>
    /// Determina el tipo de alerta basado en los eventos.
    /// </summary>
    CriticalAlertType DetermineAlertType(IEnumerable<DrowsinessEvent> events);

    /// <summary>
    /// Genera el mensaje de la alerta.
    /// </summary>
    string GenerateAlertMessage(
        int driverId,
        CriticalAlertType alertType,
        int eventsCount,
        double maxSeverity);

    /// <summary>
    /// Envía la notificación de la alerta.
    /// </summary>
    Task SendAlertNotificationAsync(CriticalAlert alert);
}
