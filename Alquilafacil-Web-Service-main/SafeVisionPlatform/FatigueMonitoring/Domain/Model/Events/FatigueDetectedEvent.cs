namespace SafeVisionPlatform.FatigueMonitoring.Domain.Model.Events;

/// <summary>
/// Evento de dominio que se dispara cuando se detectan signos de fatiga en el conductor.
/// </summary>
public class FatigueDetectedEvent
{
    public int DriverId { get; }
    public int TripId { get; }
    public int DrowsinessEventId { get; }
    public string EventType { get; }
    public double SeverityValue { get; }
    public DateTime DetectedAt { get; }

    public FatigueDetectedEvent(
        int driverId,
        int tripId,
        int drowsinessEventId,
        string eventType,
        double severityValue)
    {
        DriverId = driverId;
        TripId = tripId;
        DrowsinessEventId = drowsinessEventId;
        EventType = eventType;
        SeverityValue = severityValue;
        DetectedAt = DateTime.UtcNow;
    }
}
