namespace SafeVisionPlatform.Management.Application.Internal.DTO;

/// <summary>
/// DTO para transferir datos de reportes.
/// </summary>
public class ReportDTO
{
    public int Id { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? DriverId { get; set; }
    public string? DriverName { get; set; }
    public int? FleetId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ExportFormat { get; set; }
    public string? ExportUrl { get; set; }
    public DateTime GeneratedAt { get; set; }
    public DateTime? ExportedAt { get; set; }
    public ReportMetricsDTO Metrics { get; set; } = new();
    public List<RiskPatternDTO> RiskPatterns { get; set; } = new();
}

/// <summary>
/// DTO para métricas de reporte.
/// </summary>
public class ReportMetricsDTO
{
    public int TotalTrips { get; set; }
    public int CompletedTrips { get; set; }
    public int TotalAlerts { get; set; }
    public int CriticalAlerts { get; set; }
    public double TotalDistanceKm { get; set; }
    public int TotalDrivingMinutes { get; set; }
    public double SafeTripsPercentage { get; set; }
    public double AverageAlertsPerTrip { get; set; }
    public double AverageSafetyScore { get; set; }
    public int UniqueDrivers { get; set; }
    public Dictionary<string, int> AlertsByType { get; set; } = new();
    public Dictionary<string, int> AlertsByDay { get; set; } = new();
}

/// <summary>
/// DTO para patrones de riesgo.
/// </summary>
public class RiskPatternDTO
{
    public string PatternType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OccurrenceCount { get; set; }
    public string SeverityLevel { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public int? StartHour { get; set; }
    public int? EndHour { get; set; }
    public List<string> DaysOfWeek { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

/// <summary>
/// DTO para datos del conductor.
/// </summary>
public class DriverDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? CurrentTripId { get; set; }
    public int? AssignedVehicleId { get; set; }
    public double SafetyScore { get; set; }
    public int TotalTrips { get; set; }
    public int TotalAlerts { get; set; }
}

/// <summary>
/// DTO para eventos críticos.
/// </summary>
public class CriticalEventDTO
{
    public int Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public int DriverId { get; set; }
    public string? DriverName { get; set; }
    public int? TripId { get; set; }
    public int? ManagedByManagerId { get; set; }
    public string? ManagerName { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<string> ActionsTaken { get; set; } = new();
    public string? InsuranceReference { get; set; }
    public bool EmergencyResponseDispatched { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

/// <summary>
/// DTO para crear un evento crítico.
/// </summary>
public class CreateCriticalEventDTO
{
    public string EventType { get; set; } = string.Empty;
    public int DriverId { get; set; }
    public int? TripId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string? Location { get; set; }
}

/// <summary>
/// DTO para generar un reporte de gestión.
/// </summary>
public class GenerateManagementReportDTO
{
    public string ReportType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int GeneratedById { get; set; }
    public int? DriverId { get; set; }
    public int? FleetId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

/// <summary>
/// DTO para exportar un reporte.
/// </summary>
public class ExportReportDTO
{
    public int ReportId { get; set; }
    public string Format { get; set; } = "PDF"; // PDF, Excel
}

/// <summary>
/// DTO para asignación de conductor.
/// </summary>
public class DriverAssignmentDTO
{
    public int DriverId { get; set; }
    public int TripId { get; set; }
    public int AssignedByManagerId { get; set; }
    public string? Reason { get; set; }
}

/// <summary>
/// DTO para gerente.
/// </summary>
public class ManagerDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
    public List<int> ManagedFleetIds { get; set; } = new();
    public bool IsActive { get; set; }
}
