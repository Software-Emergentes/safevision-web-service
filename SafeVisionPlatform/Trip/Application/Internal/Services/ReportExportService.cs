using System.Text;
using SafeVisionPlatform.Trip.Application.Internal.QueryServices;
using SafeVisionPlatform.Trip.Domain.Services;

namespace SafeVisionPlatform.Trip.Application.Internal.Services;

/// <summary>
/// Implementación del servicio de exportación de reportes.
/// Genera archivos PDF y Excel con información de viajes y alertas.
/// </summary>
public class ReportExportService : IReportExportService
{
    private readonly IReportQueryService _reportQueryService;
    private readonly IAlertQueryService _alertQueryService;

    public ReportExportService(
        IReportQueryService reportQueryService,
        IAlertQueryService alertQueryService)
    {
        _reportQueryService = reportQueryService;
        _alertQueryService = alertQueryService;
    }

    public async Task<byte[]> ExportReportToPdfAsync(int reportId)
    {
        var report = await _reportQueryService.GetReportByIdAsync(reportId);
        if (report == null)
            throw new InvalidOperationException($"Report with ID {reportId} not found");

        // Por ahora, retornamos un PDF simple en texto
        // En producción, se usaría una librería como iTextSharp o QuestPDF
        var pdfContent = GeneratePdfContent(new[] { report });
        return Encoding.UTF8.GetBytes(pdfContent);
    }

    public async Task<byte[]> ExportMultipleReportsToPdfAsync(IEnumerable<int> reportIds)
    {
        var reports = new List<SafeVisionPlatform.Trip.Application.Internal.DTO.TripReportDTO>();

        foreach (var reportId in reportIds)
        {
            var report = await _reportQueryService.GetReportByIdAsync(reportId);
            if (report != null)
                reports.Add(report);
        }

        var pdfContent = GeneratePdfContent(reports);
        return Encoding.UTF8.GetBytes(pdfContent);
    }

    public async Task<byte[]> ExportReportToExcelAsync(int reportId)
    {
        var report = await _reportQueryService.GetReportByIdAsync(reportId);
        if (report == null)
            throw new InvalidOperationException($"Report with ID {reportId} not found");

        // Por ahora, retornamos un CSV que Excel puede abrir
        // En producción, se usaría una librería como EPPlus o ClosedXML
        var csvContent = GenerateCsvContent(new[] { report });
        return Encoding.UTF8.GetBytes(csvContent);
    }

    public async Task<byte[]> ExportMultipleReportsToExcelAsync(IEnumerable<int> reportIds)
    {
        var reports = new List<SafeVisionPlatform.Trip.Application.Internal.DTO.TripReportDTO>();

        foreach (var reportId in reportIds)
        {
            var report = await _reportQueryService.GetReportByIdAsync(reportId);
            if (report != null)
                reports.Add(report);
        }

        var csvContent = GenerateCsvContent(reports);
        return Encoding.UTF8.GetBytes(csvContent);
    }

    public async Task<byte[]> ExportDriverReportsByDateRangeToPdfAsync(int driverId, DateTime startDate, DateTime endDate)
    {
        var reports = await _reportQueryService.GetReportsByDriverIdAsync(driverId);
        var filteredReports = reports.Where(r =>
            r.StartTime >= startDate && r.EndTime <= endDate).ToList();

        var pdfContent = GeneratePdfContent(filteredReports);
        return Encoding.UTF8.GetBytes(pdfContent);
    }

    public async Task<byte[]> ExportDriverReportsByDateRangeToExcelAsync(int driverId, DateTime startDate, DateTime endDate)
    {
        var reports = await _reportQueryService.GetReportsByDriverIdAsync(driverId);
        var filteredReports = reports.Where(r =>
            r.StartTime >= startDate && r.EndTime <= endDate).ToList();

        var csvContent = GenerateCsvContent(filteredReports);
        return Encoding.UTF8.GetBytes(csvContent);
    }

    public async Task<byte[]> ExportFleetStatisticsToPdfAsync(DateTime startDate, DateTime endDate)
    {
        var allReports = await _reportQueryService.GetAllReportsAsync();
        var filteredReports = allReports.Where(r =>
            r.StartTime >= startDate && r.EndTime <= endDate).ToList();

        var statistics = GenerateFleetStatistics(filteredReports);
        return Encoding.UTF8.GetBytes(statistics);
    }

    public async Task<byte[]> ExportFleetStatisticsToExcelAsync(DateTime startDate, DateTime endDate)
    {
        var allReports = await _reportQueryService.GetAllReportsAsync();
        var filteredReports = allReports.Where(r =>
            r.StartTime >= startDate && r.EndTime <= endDate).ToList();

        var statistics = GenerateFleetStatisticsCSV(filteredReports);
        return Encoding.UTF8.GetBytes(statistics);
    }

    private string GeneratePdfContent(IEnumerable<SafeVisionPlatform.Trip.Application.Internal.DTO.TripReportDTO> reports)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== SAFE VISION - REPORTE DE VIAJES ===");
        sb.AppendLine($"Generado: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine();

        foreach (var report in reports)
        {
            sb.AppendLine($"--- Reporte ID: {report.Id} ---");
            sb.AppendLine($"Conductor ID: {report.DriverId}");
            sb.AppendLine($"Vehículo ID: {report.VehicleId}");
            sb.AppendLine($"Inicio: {report.StartTime:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Fin: {report.EndTime:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Duración: {report.DurationMinutes} minutos");
            sb.AppendLine($"Distancia: {report.DistanceKm:F2} km");
            sb.AppendLine($"Alertas: {report.AlertCount}");
            sb.AppendLine($"Notas: {report.Notes ?? "N/A"}");
            sb.AppendLine($"Estado: {report.Status}");
            sb.AppendLine($"Enviado a: {report.Recipient}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private string GenerateCsvContent(IEnumerable<SafeVisionPlatform.Trip.Application.Internal.DTO.TripReportDTO> reports)
    {
        var sb = new StringBuilder();
        sb.AppendLine("ID,DriverID,VehicleID,StartTime,EndTime,DurationMinutes,DistanceKm,AlertCount,Notes,Status,Recipient");

        foreach (var report in reports)
        {
            sb.AppendLine($"{report.Id}," +
                         $"{report.DriverId}," +
                         $"{report.VehicleId}," +
                         $"{report.StartTime:yyyy-MM-dd HH:mm:ss}," +
                         $"{report.EndTime:yyyy-MM-dd HH:mm:ss}," +
                         $"{report.DurationMinutes}," +
                         $"{report.DistanceKm:F2}," +
                         $"{report.AlertCount}," +
                         $"\"{report.Notes?.Replace("\"", "\"\"")}\"," +
                         $"{report.Status}," +
                         $"{report.Recipient}");
        }

        return sb.ToString();
    }

    private string GenerateFleetStatistics(IEnumerable<SafeVisionPlatform.Trip.Application.Internal.DTO.TripReportDTO> reports)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== SAFE VISION - ESTADÍSTICAS DE FLOTA ===");
        sb.AppendLine($"Generado: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine();

        var totalReports = reports.Count();
        var totalAlerts = reports.Sum(r => r.AlertCount);
        var totalDistance = reports.Sum(r => r.DistanceKm);
        var totalDuration = reports.Sum(r => r.DurationMinutes);
        var avgAlertsPerTrip = totalReports > 0 ? (double)totalAlerts / totalReports : 0;

        sb.AppendLine($"Total de viajes: {totalReports}");
        sb.AppendLine($"Total de alertas: {totalAlerts}");
        sb.AppendLine($"Promedio de alertas por viaje: {avgAlertsPerTrip:F2}");
        sb.AppendLine($"Distancia total recorrida: {totalDistance:F2} km");
        sb.AppendLine($"Tiempo total de conducción: {totalDuration} minutos ({totalDuration / 60.0:F2} horas)");
        sb.AppendLine();

        // Estadísticas por conductor
        var byDriver = reports.GroupBy(r => r.DriverId);
        sb.AppendLine("--- Estadísticas por Conductor ---");
        foreach (var group in byDriver)
        {
            sb.AppendLine($"Conductor {group.Key}:");
            sb.AppendLine($"  Viajes: {group.Count()}");
            sb.AppendLine($"  Alertas totales: {group.Sum(r => r.AlertCount)}");
            sb.AppendLine($"  Distancia total: {group.Sum(r => r.DistanceKm):F2} km");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private string GenerateFleetStatisticsCSV(IEnumerable<SafeVisionPlatform.Trip.Application.Internal.DTO.TripReportDTO> reports)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Metric,Value");

        var totalReports = reports.Count();
        var totalAlerts = reports.Sum(r => r.AlertCount);
        var totalDistance = reports.Sum(r => r.DistanceKm);
        var totalDuration = reports.Sum(r => r.DurationMinutes);
        var avgAlertsPerTrip = totalReports > 0 ? (double)totalAlerts / totalReports : 0;

        sb.AppendLine($"Total Trips,{totalReports}");
        sb.AppendLine($"Total Alerts,{totalAlerts}");
        sb.AppendLine($"Average Alerts Per Trip,{avgAlertsPerTrip:F2}");
        sb.AppendLine($"Total Distance (km),{totalDistance:F2}");
        sb.AppendLine($"Total Duration (minutes),{totalDuration}");
        sb.AppendLine($"Total Duration (hours),{totalDuration / 60.0:F2}");

        return sb.ToString();
    }
}
