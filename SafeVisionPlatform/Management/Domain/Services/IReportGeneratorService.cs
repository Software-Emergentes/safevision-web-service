using SafeVisionPlatform.Management.Domain.Model.Entities;

namespace SafeVisionPlatform.Management.Domain.Services;

/// <summary>
/// Servicio de dominio encargado de crear reportes, consolidar métricas,
/// aplicar formato de exportación (PDF, Excel) y garantizar consistencia de la información.
/// </summary>
public interface IReportGeneratorService
{
    /// <summary>
    /// Genera un reporte de rendimiento de conductor.
    /// </summary>
    Task<Report> GenerateDriverPerformanceReportAsync(
        int driverId,
        int generatedById,
        DateTime startDate,
        DateTime endDate);

    /// <summary>
    /// Genera un reporte de resumen de flota.
    /// </summary>
    Task<Report> GenerateFleetOverviewReportAsync(
        int fleetId,
        int generatedById,
        DateTime startDate,
        DateTime endDate);

    /// <summary>
    /// Genera un reporte de análisis de seguridad.
    /// </summary>
    Task<Report> GenerateSafetyAnalysisReportAsync(
        int generatedById,
        DateTime startDate,
        DateTime endDate,
        int? driverId = null,
        int? fleetId = null);

    /// <summary>
    /// Genera un reporte de resumen de alertas.
    /// </summary>
    Task<Report> GenerateAlertsSummaryReportAsync(
        int generatedById,
        DateTime startDate,
        DateTime endDate);

    /// <summary>
    /// Exporta un reporte a PDF.
    /// </summary>
    Task<string> ExportToPdfAsync(Report report);

    /// <summary>
    /// Exporta un reporte a Excel.
    /// </summary>
    Task<string> ExportToExcelAsync(Report report);

    /// <summary>
    /// Calcula métricas para el reporte.
    /// </summary>
    Task<ReportMetrics> CalculateMetricsAsync(
        DateTime startDate,
        DateTime endDate,
        int? driverId = null,
        int? fleetId = null);
}
