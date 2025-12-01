using SafeVisionPlatform.FatigueMonitoring.Domain.Model.Entities;
using SafeVisionPlatform.FatigueMonitoring.Domain.Model.ValueObjects;
using SafeVisionPlatform.FatigueMonitoring.Domain.Services;
using SafeVisionPlatform.FatigueMonitoring.Infrastructure.Integration;

namespace SafeVisionPlatform.FatigueMonitoring.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de generación de alertas críticas.
/// </summary>
public class AlertGenerationServiceImpl : IAlertGenerationService
{
    private readonly NotificationAdapter _notificationAdapter;
    private readonly ILogger<AlertGenerationServiceImpl> _logger;

    public AlertGenerationServiceImpl(
        NotificationAdapter notificationAdapter,
        ILogger<AlertGenerationServiceImpl> logger)
    {
        _notificationAdapter = notificationAdapter;
        _logger = logger;
    }

    public async Task<CriticalAlert> GenerateAlertAsync(
        int driverId,
        int tripId,
        int? managerId,
        IEnumerable<DrowsinessEvent> drowsinessEvents)
    {
        var eventsList = drowsinessEvents.ToList();

        if (!eventsList.Any())
            throw new InvalidOperationException("No hay eventos para generar alerta");

        var alertType = DetermineAlertType(eventsList);
        var maxSeverity = eventsList.Max(e => e.Severity.Value);
        var severity = new SeverityScore(maxSeverity);
        var message = GenerateAlertMessage(driverId, alertType, eventsList.Count, maxSeverity);

        var alert = new CriticalAlert(
            driverId,
            tripId,
            managerId,
            alertType,
            severity,
            message,
            eventsList.Count,
            "InApp");

        _logger.LogInformation(
            $"Alerta crítica generada: Driver {driverId}, Tipo {alertType}, Severidad {maxSeverity:F2}");

        return await Task.FromResult(alert);
    }

    public CriticalAlertType DetermineAlertType(IEnumerable<DrowsinessEvent> events)
    {
        var eventsList = events.ToList();

        // Priorizar por tipo de evento más grave
        if (eventsList.Any(e => e.EventType == DrowsinessEventType.MicroSleep))
            return CriticalAlertType.MicroSleepDetected;

        if (eventsList.Any(e => e.EventType == DrowsinessEventType.EyeClosure))
            return CriticalAlertType.ExtendedEyeClosure;

        if (eventsList.Count(e => e.EventType == DrowsinessEventType.Yawn) >= 3)
            return CriticalAlertType.RepeatedYawning;

        if (eventsList.Average(e => e.Severity.Value) >= 0.7)
            return CriticalAlertType.SevereDrowsiness;

        if (eventsList.Count >= 3)
            return CriticalAlertType.MultipleWarnings;

        return CriticalAlertType.SevereDrowsiness;
    }

    public string GenerateAlertMessage(
        int driverId,
        CriticalAlertType alertType,
        int eventsCount,
        double maxSeverity)
    {
        var severityLevel = maxSeverity switch
        {
            >= 0.85 => "CRÍTICA",
            >= 0.70 => "ALTA",
            >= 0.50 => "MEDIA",
            _ => "BAJA"
        };

        var alertDescription = alertType switch
        {
            CriticalAlertType.MicroSleepDetected =>
                "MICRO-SUEÑO DETECTADO. El conductor ha experimentado un episodio de micro-sueño.",
            CriticalAlertType.ExtendedEyeClosure =>
                "CIERRE PROLONGADO DE OJOS. El conductor mantiene los ojos cerrados por períodos extendidos.",
            CriticalAlertType.RepeatedYawning =>
                "BOSTEZOS REPETIDOS. Se han detectado múltiples bostezos indicando fatiga.",
            CriticalAlertType.SevereDrowsiness =>
                "SOMNOLENCIA SEVERA. Los indicadores muestran niveles peligrosos de fatiga.",
            CriticalAlertType.MultipleWarnings =>
                "MÚLTIPLES ADVERTENCIAS. Se han acumulado varios eventos de fatiga.",
            _ => "ALERTA DE FATIGA. Se requiere atención inmediata."
        };

        return $"[{severityLevel}] ALERTA CONDUCTOR #{driverId}: {alertDescription} " +
               $"Se han registrado {eventsCount} evento(s) de fatiga. " +
               $"Severidad máxima: {maxSeverity:P0}. Se recomienda intervención inmediata.";
    }

    public async Task SendAlertNotificationAsync(CriticalAlert alert)
    {
        await _notificationAdapter.SendCriticalAlertNotificationAsync(alert);

        // Notificar al conductor
        await _notificationAdapter.SendPushNotificationToDriverAsync(
            alert.DriverId,
            "¡Alerta de fatiga! Se recomienda tomar un descanso.");

        // Notificar al gerente si está asignado
        if (alert.ManagerId.HasValue)
        {
            await _notificationAdapter.SendNotificationToManagerAsync(
                alert.ManagerId.Value,
                alert.Message,
                alert.Severity.Level);
        }
    }
}
