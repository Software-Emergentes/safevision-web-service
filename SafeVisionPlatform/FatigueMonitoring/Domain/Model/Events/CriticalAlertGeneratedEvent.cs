namespace SafeVisionPlatform.FatigueMonitoring.Domain.Model.Events;

/// <summary>
/// Evento de dominio que se emite al crear una alerta crítica para notificar a otros sistemas.
/// </summary>
public class CriticalAlertGeneratedEvent
{
    public int AlertId { get; }
    public int DriverId { get; }
    public int TripId { get; }
    public int? ManagerId { get; }
    public string AlertType { get; }
    public string SeverityLevel { get; }
    public string Message { get; }
    public DateTime GeneratedAt { get; }

    public CriticalAlertGeneratedEvent(
        int alertId,
        int driverId,
        int tripId,
        int? managerId,
        string alertType,
        string severityLevel,
        string message)
    {
        AlertId = alertId;
        DriverId = driverId;
        TripId = tripId;
        ManagerId = managerId;
        AlertType = alertType;
        SeverityLevel = severityLevel;
        Message = message;
        GeneratedAt = DateTime.UtcNow;
    }
}
