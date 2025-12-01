namespace SafeVisionPlatform.Trip.Application.Internal.DTO;

/// <summary>
/// Objeto de transferencia que contiene los datos básicos del viaje.
/// </summary>
public class TripDTO
{
    public int Id { get; set; }
    public int DriverId { get; set; }
    public int VehicleId { get; set; }
    public int Status { get; set; }
    public string StatusString => Status switch
    {
        0 => "Initiated",
        1 => "InProgress",
        2 => "Completed",
        3 => "Cancelled",
        _ => "Unknown"
    };
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public int AlertCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public TripDataPolicyDTO DataPolicy { get; set; } = new TripDataPolicyDTO();
}

/// <summary>
/// DTO para la política de datos del viaje.
/// </summary>
public class TripDataPolicyDTO
{
    public bool SyncToCloud { get; set; }
    public int SyncIntervalMinutes { get; set; }
    public double TotalDistanceKm { get; set; }
    public int TotalDurationMinutes { get; set; }
}

/// <summary>
/// Transporta la información del reporte de viaje, incluyendo métricas,
/// alertas y destino de envío (conductor o gerente).
/// </summary>
public class TripReportDTO
{
    public int Id { get; set; }
    public int? TripId { get; set; }
    public int DriverId { get; set; }
    public int VehicleId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public double DistanceKm { get; set; }
    public int AlertCount { get; set; }
    public string? Notes { get; set; }
    public int Recipient { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
}

/// <summary>
/// DTO para crear un nuevo viaje.
/// </summary>
public class CreateTripDTO
{
    public int DriverId { get; set; }
    public int VehicleId { get; set; }
    public bool SyncToCloud { get; set; } = true;
    public int SyncIntervalMinutes { get; set; } = 5;
}

/// <summary>
/// DTO para las alertas de viaje.
/// </summary>
public class AlertDTO
{
    public int Id { get; set; }
    public int TripId { get; set; }
    public int AlertType { get; set; }
    public string Description { get; set; } = string.Empty;
    public double? Severity { get; set; }
    public DateTime DetectedAt { get; set; }
    public DateTime Timestamp => DetectedAt;
    public bool Acknowledged { get; set; }
}

/// <summary>
/// DTO para crear una alerta.
/// </summary>
public class CreateAlertDTO
{
    public int TripId { get; set; }
    public int AlertType { get; set; }
    public string Description { get; set; } = string.Empty;
    public double? Severity { get; set; }
}

/// <summary>
/// DTO para generar un reporte de viaje.
/// </summary>
public class GenerateReportDTO
{
    public int TripId { get; set; }
    public double DistanceKm { get; set; }
    public int Recipient { get; set; }
    public string? Notes { get; set; }
}
