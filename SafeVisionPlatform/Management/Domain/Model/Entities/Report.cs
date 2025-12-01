using SafeVisionPlatform.Management.Domain.Model.ValueObjects;

namespace SafeVisionPlatform.Management.Domain.Model.Entities;

/// <summary>
/// Representa un reporte consolidado de alertas, viajes y métricas
/// asociadas a un conductor o a toda la flota.
/// </summary>
public class Report
{
    public int Id { get; private set; }

    /// <summary>
    /// Tipo de reporte.
    /// </summary>
    public ReportType ReportType { get; private set; }

    /// <summary>
    /// Título del reporte.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Descripción del reporte.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// ID del gerente que generó el reporte.
    /// </summary>
    public int GeneratedById { get; private set; }

    /// <summary>
    /// ID del conductor (si es reporte individual).
    /// </summary>
    public int? DriverId { get; private set; }

    /// <summary>
    /// ID de la flota (si es reporte de flota).
    /// </summary>
    public int? FleetId { get; private set; }

    /// <summary>
    /// Fecha de inicio del período del reporte.
    /// </summary>
    public DateTime StartDate { get; private set; }

    /// <summary>
    /// Fecha de fin del período del reporte.
    /// </summary>
    public DateTime EndDate { get; private set; }

    /// <summary>
    /// Métricas del reporte.
    /// </summary>
    public ReportMetrics Metrics { get; private set; } = new();

    /// <summary>
    /// Patrones de riesgo identificados.
    /// </summary>
    public List<RiskPattern> RiskPatterns { get; private set; } = new();

    /// <summary>
    /// Estado del reporte.
    /// </summary>
    public ReportStatus Status { get; private set; }

    /// <summary>
    /// Formato de exportación.
    /// </summary>
    public ExportFormat? ExportFormat { get; private set; }

    /// <summary>
    /// URL del archivo exportado.
    /// </summary>
    public string? ExportUrl { get; private set; }

    /// <summary>
    /// Fecha de generación.
    /// </summary>
    public DateTime GeneratedAt { get; private set; }

    /// <summary>
    /// Fecha de exportación.
    /// </summary>
    public DateTime? ExportedAt { get; private set; }

    private Report() { }

    public Report(
        ReportType reportType,
        string title,
        int generatedById,
        DateTime startDate,
        DateTime endDate,
        int? driverId = null,
        int? fleetId = null)
    {
        ReportType = reportType;
        Title = title;
        GeneratedById = generatedById;
        StartDate = startDate;
        EndDate = endDate;
        DriverId = driverId;
        FleetId = fleetId;
        Status = ReportStatus.Generating;
        GeneratedAt = DateTime.UtcNow;
    }

    public void SetDescription(string description)
    {
        Description = description;
    }

    public void SetMetrics(ReportMetrics metrics)
    {
        Metrics = metrics;
    }

    public void AddRiskPattern(RiskPattern pattern)
    {
        RiskPatterns.Add(pattern);
    }

    public void MarkAsCompleted()
    {
        Status = ReportStatus.Completed;
    }

    public void MarkAsExported(ExportFormat format, string exportUrl)
    {
        Status = ReportStatus.Exported;
        ExportFormat = format;
        ExportUrl = exportUrl;
        ExportedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string reason)
    {
        Status = ReportStatus.Failed;
        Description = reason;
    }
}

/// <summary>
/// Tipos de reportes.
/// </summary>
public enum ReportType
{
    DriverPerformance = 1,      // Rendimiento de conductor
    FleetOverview = 2,          // Resumen de flota
    SafetyAnalysis = 3,         // Análisis de seguridad
    AlertsSummary = 4,          // Resumen de alertas
    RiskPatternAnalysis = 5,    // Análisis de patrones de riesgo
    ComplianceReport = 6        // Reporte de cumplimiento
}

/// <summary>
/// Estados del reporte.
/// </summary>
public enum ReportStatus
{
    Generating = 1,     // Generando
    Completed = 2,      // Completado
    Exported = 3,       // Exportado
    Failed = 4          // Fallido
}

/// <summary>
/// Formatos de exportación.
/// </summary>
public enum ExportFormat
{
    PDF = 1,
    Excel = 2,
    CSV = 3
}

/// <summary>
/// Métricas incluidas en el reporte.
/// </summary>
public class ReportMetrics
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
