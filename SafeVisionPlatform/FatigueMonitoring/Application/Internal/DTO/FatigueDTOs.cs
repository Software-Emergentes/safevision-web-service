namespace SafeVisionPlatform.FatigueMonitoring.Application.Internal.DTO;

/// <summary>
/// DTO para el estado actual de fatiga del conductor.
/// </summary>
public class FatigueStatusDTO
{
    public int DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public int? CurrentTripId { get; set; }
    public string CurrentStatus { get; set; } = string.Empty; // Normal, Warning, Critical
    public double CurrentSeverity { get; set; }
    public int RecentEventsCount { get; set; }
    public DateTime? LastEventTime { get; set; }
    public string? LastEventType { get; set; }
    public bool HasActiveAlert { get; set; }
    public MonitoringSessionStatusDTO? CurrentSession { get; set; }
}

/// <summary>
/// DTO para el estado de una sesión de monitoreo.
/// </summary>
public class MonitoringSessionStatusDTO
{
    public int SessionId { get; set; }
    public DateTime StartedAt { get; set; }
    public int DurationMinutes { get; set; }
    public int TotalEventsDetected { get; set; }
    public int CriticalEventsCount { get; set; }
    public double AverageSeverity { get; set; }
}

/// <summary>
/// DTO para reportes de alertas críticas generadas.
/// </summary>
public class CriticalAlertReportDTO
{
    public int AlertId { get; set; }
    public int DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public int TripId { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public string SeverityLevel { get; set; } = string.Empty;
    public double SeverityValue { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int DrowsinessEventsCount { get; set; }
    public DateTime GeneratedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public string? AcknowledgedByName { get; set; }
    public string? ActionTaken { get; set; }
}

/// <summary>
/// DTO para datos de entrada del sensor.
/// </summary>
public class SensorDataInputDTO
{
    public int DriverId { get; set; }
    public int TripId { get; set; }
    public int MonitoringSessionId { get; set; }
    public double BlinkRate { get; set; }
    public double EyeOpenness { get; set; }
    public double MouthOpenness { get; set; }
    public double HeadTilt { get; set; }
    public double EyeClosureDuration { get; set; }
}

/// <summary>
/// DTO para evento de somnolencia detectado.
/// </summary>
public class DrowsinessEventDTO
{
    public int Id { get; set; }
    public int DriverId { get; set; }
    public int TripId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public double SeverityValue { get; set; }
    public string SeverityLevel { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public bool Processed { get; set; }
    public SensorDataDTO? SensorData { get; set; }
}

/// <summary>
/// DTO para datos del sensor.
/// </summary>
public class SensorDataDTO
{
    public double BlinkRate { get; set; }
    public double EyeOpenness { get; set; }
    public double MouthOpenness { get; set; }
    public double HeadTilt { get; set; }
    public double EyeClosureDuration { get; set; }
    public DateTime CapturedAt { get; set; }
}

/// <summary>
/// DTO para métricas agregadas de alertas.
/// </summary>
public class AlertMetricsDTO
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalAlerts { get; set; }
    public int PendingAlerts { get; set; }
    public int AcknowledgedAlerts { get; set; }
    public int ResolvedAlerts { get; set; }
    public Dictionary<string, int> AlertsByType { get; set; } = new();
    public Dictionary<string, int> AlertsBySeverity { get; set; } = new();
    public double AverageResponseTimeMinutes { get; set; }
}
