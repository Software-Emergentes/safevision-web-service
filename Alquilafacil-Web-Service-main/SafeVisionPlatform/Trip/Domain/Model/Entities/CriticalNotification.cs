namespace SafeVisionPlatform.Trip.Domain.Model.Entities;

/// <summary>
/// Entidad que representa una notificación crítica enviada a un gerente.
/// </summary>
public class CriticalNotification
{
    public int Id { get; set; }
    public int DriverId { get; set; }
    public int TripId { get; set; }
    public int? ManagerId { get; set; }
    public string Severity { get; set; } = string.Empty;
    public int AlertType { get; set; }
    public int CriticalAlertsCount { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Status { get; set; } = "Pending";
    public string Channel { get; set; } = "InApp";
    public DateTime? SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? AcknowledgedAt { get; set; }

    public CriticalNotification()
    {
        Timestamp = DateTime.UtcNow;
    }

    public CriticalNotification(
        int driverId,
        int tripId,
        int? managerId,
        string severity,
        int alertType,
        int criticalAlertsCount,
        string message,
        string channel = "InApp")
    {
        DriverId = driverId;
        TripId = tripId;
        ManagerId = managerId;
        Severity = severity;
        AlertType = alertType;
        CriticalAlertsCount = criticalAlertsCount;
        Message = message;
        Channel = channel;
        Timestamp = DateTime.UtcNow;
        Status = "Pending";
    }

    public void MarkAsSent()
    {
        Status = "Sent";
        SentAt = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        Status = "Read";
        ReadAt = DateTime.UtcNow;
    }

    public void MarkAsAcknowledged()
    {
        Status = "Acknowledged";
        AcknowledgedAt = DateTime.UtcNow;
    }
}
