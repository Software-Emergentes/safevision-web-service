using SafeVisionPlatform.Management.Domain.Model.Entities;
using SafeVisionPlatform.Management.Domain.Services;
using SafeVisionPlatform.Trip.Domain.Repositories;

namespace SafeVisionPlatform.Management.Infrastructure.Services;

/// <summary>
/// Implementación del servicio generador de reportes.
/// </summary>
public class ReportGeneratorServiceImpl : IReportGeneratorService
{
    private readonly ITripRepository _tripRepository;
    private readonly IAlertRepository _alertRepository;
    private readonly ILogger<ReportGeneratorServiceImpl> _logger;

    public ReportGeneratorServiceImpl(
        ITripRepository tripRepository,
        IAlertRepository alertRepository,
        ILogger<ReportGeneratorServiceImpl> logger)
    {
        _tripRepository = tripRepository;
        _alertRepository = alertRepository;
        _logger = logger;
    }

    public async Task<Report> GenerateDriverPerformanceReportAsync(
        int driverId,
        int generatedById,
        DateTime startDate,
        DateTime endDate)
    {
        var report = new Report(
            ReportType.DriverPerformance,
            $"Reporte de Rendimiento - Conductor #{driverId}",
            generatedById,
            startDate,
            endDate,
            driverId);

        var metrics = await CalculateMetricsAsync(startDate, endDate, driverId);
        report.SetMetrics(metrics);

        report.SetDescription(
            $"Análisis de rendimiento del conductor #{driverId} " +
            $"durante el período {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}");

        return report;
    }

    public async Task<Report> GenerateFleetOverviewReportAsync(
        int fleetId,
        int generatedById,
        DateTime startDate,
        DateTime endDate)
    {
        var report = new Report(
            ReportType.FleetOverview,
            $"Resumen de Flota #{fleetId}",
            generatedById,
            startDate,
            endDate,
            fleetId: fleetId);

        var metrics = await CalculateMetricsAsync(startDate, endDate, fleetId: fleetId);
        report.SetMetrics(metrics);

        report.SetDescription(
            $"Resumen general de la flota #{fleetId} " +
            $"durante el período {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}");

        return report;
    }

    public async Task<Report> GenerateSafetyAnalysisReportAsync(
        int generatedById,
        DateTime startDate,
        DateTime endDate,
        int? driverId = null,
        int? fleetId = null)
    {
        var title = driverId.HasValue
            ? $"Análisis de Seguridad - Conductor #{driverId}"
            : fleetId.HasValue
                ? $"Análisis de Seguridad - Flota #{fleetId}"
                : "Análisis de Seguridad General";

        var report = new Report(
            ReportType.SafetyAnalysis,
            title,
            generatedById,
            startDate,
            endDate,
            driverId,
            fleetId);

        var metrics = await CalculateMetricsAsync(startDate, endDate, driverId, fleetId);
        report.SetMetrics(metrics);

        report.SetDescription(
            $"Análisis de seguridad detallado " +
            $"para el período {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}");

        return report;
    }

    public async Task<Report> GenerateAlertsSummaryReportAsync(
        int generatedById,
        DateTime startDate,
        DateTime endDate)
    {
        var report = new Report(
            ReportType.AlertsSummary,
            $"Resumen de Alertas {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}",
            generatedById,
            startDate,
            endDate);

        var metrics = await CalculateMetricsAsync(startDate, endDate);
        report.SetMetrics(metrics);

        report.SetDescription("Resumen consolidado de todas las alertas generadas en el período");

        return report;
    }

    public async Task<string> ExportToPdfAsync(Report report)
    {
        // Simulación de exportación a PDF
        _logger.LogInformation($"Exportando reporte {report.Id} a PDF");

        // En producción, aquí iría la lógica de generación de PDF
        // usando librerías como iTextSharp, PdfSharp, etc.

        await Task.Delay(500); // Simular proceso

        var fileName = $"report_{report.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
        var exportUrl = $"/exports/pdf/{fileName}";

        _logger.LogInformation($"PDF generado: {exportUrl}");

        return exportUrl;
    }

    public async Task<string> ExportToExcelAsync(Report report)
    {
        // Simulación de exportación a Excel
        _logger.LogInformation($"Exportando reporte {report.Id} a Excel");

        // En producción, aquí iría la lógica de generación de Excel
        // usando librerías como EPPlus, ClosedXML, etc.

        await Task.Delay(500); // Simular proceso

        var fileName = $"report_{report.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
        var exportUrl = $"/exports/excel/{fileName}";

        _logger.LogInformation($"Excel generado: {exportUrl}");

        return exportUrl;
    }

    public async Task<ReportMetrics> CalculateMetricsAsync(
        DateTime startDate,
        DateTime endDate,
        int? driverId = null,
        int? fleetId = null)
    {
        var trips = await _tripRepository.GetTripsByDateRangeAsync(startDate, endDate);

        if (driverId.HasValue)
        {
            trips = trips.Where(t => t.DriverId == driverId.Value);
        }

        var tripsList = trips.ToList();
        var completedTrips = tripsList.Where(t => t.Status == Trip.Domain.Model.ValueObjects.TripStatus.Completed).ToList();

        var totalAlerts = 0;
        var criticalAlerts = 0;
        var alertsByType = new Dictionary<string, int>();
        var alertsByDay = new Dictionary<string, int>();

        foreach (var trip in tripsList)
        {
            var alerts = await _alertRepository.GetAlertsByTripIdAsync(trip.Id);
            var alertsList = alerts.ToList();

            totalAlerts += alertsList.Count;
            criticalAlerts += alertsList.Count(a =>
                a.AlertType == Trip.Domain.Model.Entities.AlertType.Drowsiness ||
                a.AlertType == Trip.Domain.Model.Entities.AlertType.MicroSleep);

            foreach (var alert in alertsList)
            {
                var typeName = alert.AlertType.ToString();
                if (!alertsByType.ContainsKey(typeName))
                    alertsByType[typeName] = 0;
                alertsByType[typeName]++;

                var dayName = alert.DetectedAt.DayOfWeek.ToString();
                if (!alertsByDay.ContainsKey(dayName))
                    alertsByDay[dayName] = 0;
                alertsByDay[dayName]++;
            }
        }

        var totalDistance = completedTrips.Sum(t => t.DataPolicy.TotalDistanceKm);
        var totalDuration = completedTrips.Sum(t => t.DataPolicy.TotalDurationMinutes);

        var safeTrips = 0;
        foreach (var trip in completedTrips)
        {
            var alerts = await _alertRepository.GetAlertsByTripIdAsync(trip.Id);
            var hasCritical = alerts.Any(a =>
                a.AlertType == Trip.Domain.Model.Entities.AlertType.Drowsiness ||
                a.AlertType == Trip.Domain.Model.Entities.AlertType.MicroSleep);

            if (!hasCritical) safeTrips++;
        }

        var safeTripsPercentage = completedTrips.Any()
            ? (double)safeTrips / completedTrips.Count * 100
            : 100;

        var avgAlertsPerTrip = tripsList.Any()
            ? (double)totalAlerts / tripsList.Count
            : 0;

        return new ReportMetrics
        {
            TotalTrips = tripsList.Count,
            CompletedTrips = completedTrips.Count,
            TotalAlerts = totalAlerts,
            CriticalAlerts = criticalAlerts,
            TotalDistanceKm = totalDistance,
            TotalDrivingMinutes = totalDuration,
            SafeTripsPercentage = safeTripsPercentage,
            AverageAlertsPerTrip = avgAlertsPerTrip,
            AverageSafetyScore = 100 - (avgAlertsPerTrip * 10), // Cálculo simplificado
            UniqueDrivers = tripsList.Select(t => t.DriverId).Distinct().Count(),
            AlertsByType = alertsByType,
            AlertsByDay = alertsByDay
        };
    }
}
