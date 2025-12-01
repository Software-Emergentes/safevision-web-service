using SafeVisionPlatform.FatigueMonitoring.Domain.Model.ValueObjects;

namespace SafeVisionPlatform.FatigueMonitoring.Domain.Model.Entities;

/// <summary>
/// Representa un evento de somnolencia detectado (como parpadeos, bostezos o micro sueños)
/// asociado a una sesión de monitoreo.
/// </summary>
public class DrowsinessEvent
{
    public int Id { get; private set; }

    /// <summary>
    /// ID del conductor asociado al evento.
    /// </summary>
    public int DriverId { get; private set; }

    /// <summary>
    /// ID del viaje durante el cual se detectó el evento.
    /// </summary>
    public int TripId { get; private set; }

    /// <summary>
    /// ID de la sesión de monitoreo.
    /// </summary>
    public int MonitoringSessionId { get; private set; }

    /// <summary>
    /// Tipo de evento de somnolencia detectado.
    /// </summary>
    public DrowsinessEventType EventType { get; private set; }

    /// <summary>
    /// Datos del sensor al momento de la detección.
    /// </summary>
    public SensorData SensorData { get; private set; } = null!;

    /// <summary>
    /// Puntuación de severidad del evento.
    /// </summary>
    public SeverityScore Severity { get; private set; } = null!;

    /// <summary>
    /// Valor de severidad para persistencia.
    /// </summary>
    public double SeverityValue { get; private set; }

    /// <summary>
    /// Nivel de severidad para persistencia.
    /// </summary>
    public string SeverityLevelStr { get; private set; } = string.Empty;

    // Propiedades para SensorData en persistencia
    public double SensorBlinkRate { get; private set; }
    public double SensorEyeOpenness { get; private set; }
    public double SensorMouthOpenness { get; private set; }
    public double SensorHeadTilt { get; private set; }
    public double SensorEyeClosureDuration { get; private set; }
    public DateTime SensorCapturedAt { get; private set; }

    /// <summary>
    /// Timestamp de detección del evento.
    /// </summary>
    public DateTime DetectedAt { get; private set; }

    /// <summary>
    /// Indica si el evento fue procesado para generar alertas.
    /// </summary>
    public bool Processed { get; private set; }

    /// <summary>
    /// Notas adicionales sobre el evento.
    /// </summary>
    public string? Notes { get; private set; }

    private DrowsinessEvent() { }

    public DrowsinessEvent(
        int driverId,
        int tripId,
        int monitoringSessionId,
        DrowsinessEventType eventType,
        SensorData sensorData,
        SeverityScore severity)
    {
        DriverId = driverId;
        TripId = tripId;
        MonitoringSessionId = monitoringSessionId;
        EventType = eventType;
        SensorData = sensorData;
        Severity = severity;
        // Propiedades para persistencia
        SeverityValue = severity.Value;
        SeverityLevelStr = severity.Level;
        SensorBlinkRate = sensorData.BlinkRate;
        SensorEyeOpenness = sensorData.EyeOpenness;
        SensorMouthOpenness = sensorData.MouthOpenness;
        SensorHeadTilt = sensorData.HeadTilt;
        SensorEyeClosureDuration = sensorData.EyeClosureDuration;
        SensorCapturedAt = sensorData.CapturedAt;
        DetectedAt = DateTime.UtcNow;
        Processed = false;
    }

    /// <summary>
    /// Marca el evento como procesado.
    /// </summary>
    public void MarkAsProcessed()
    {
        Processed = true;
    }

    /// <summary>
    /// Agrega notas al evento.
    /// </summary>
    public void AddNotes(string notes)
    {
        Notes = notes;
    }
}

/// <summary>
/// Tipos de eventos de somnolencia que pueden ser detectados.
/// </summary>
public enum DrowsinessEventType
{
    Blink = 1,          // Parpadeo excesivo
    Yawn = 2,           // Bostezo
    MicroSleep = 3,     // Micro-sueño
    HeadNod = 4,        // Cabeceo
    EyeClosure = 5      // Cierre prolongado de ojos
}
