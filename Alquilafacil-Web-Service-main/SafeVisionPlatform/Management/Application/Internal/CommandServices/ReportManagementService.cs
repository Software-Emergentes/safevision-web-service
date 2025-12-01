using SafeVisionPlatform.Management.Application.Internal.DTO;
using SafeVisionPlatform.Management.Domain.Model.Entities;
using SafeVisionPlatform.Management.Domain.Repositories;
using SafeVisionPlatform.Management.Domain.Services;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.Management.Application.Internal.CommandServices;

/// <summary>
/// Fachada que centraliza la generación, consulta y exportación de reportes.
/// </summary>
public class ReportManagementService
{
    private readonly IReportRepository _reportRepository;
    private readonly IReportGeneratorService _reportGeneratorService;
    private readonly IRiskAnalysisService _riskAnalysisService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReportManagementService> _logger;

    public ReportManagementService(
        IReportRepository reportRepository,
        IReportGeneratorService reportGeneratorService,
        IRiskAnalysisService riskAnalysisService,
        IUnitOfWork unitOfWork,
        ILogger<ReportManagementService> logger)
    {
        _reportRepository = reportRepository;
        _reportGeneratorService = reportGeneratorService;
        _riskAnalysisService = riskAnalysisService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Genera un nuevo reporte.
    /// </summary>
    public async Task<ReportDTO> GenerateReportAsync(GenerateManagementReportDTO request)
    {
        try
        {
            Report report;

            var reportType = Enum.Parse<ReportType>(request.ReportType);

            report = reportType switch
            {
                ReportType.DriverPerformance when request.DriverId.HasValue =>
                    await _reportGeneratorService.GenerateDriverPerformanceReportAsync(
                        request.DriverId.Value,
                        request.GeneratedById,
                        request.StartDate,
                        request.EndDate),

                ReportType.FleetOverview when request.FleetId.HasValue =>
                    await _reportGeneratorService.GenerateFleetOverviewReportAsync(
                        request.FleetId.Value,
                        request.GeneratedById,
                        request.StartDate,
                        request.EndDate),

                ReportType.SafetyAnalysis =>
                    await _reportGeneratorService.GenerateSafetyAnalysisReportAsync(
                        request.GeneratedById,
                        request.StartDate,
                        request.EndDate,
                        request.DriverId,
                        request.FleetId),

                ReportType.AlertsSummary =>
                    await _reportGeneratorService.GenerateAlertsSummaryReportAsync(
                        request.GeneratedById,
                        request.StartDate,
                        request.EndDate),

                _ => throw new ArgumentException($"Tipo de reporte no soportado: {request.ReportType}")
            };

            // Agregar patrones de riesgo si es relevante
            if (request.DriverId.HasValue)
            {
                var patterns = await _riskAnalysisService.AnalyzeDriverRiskPatternsAsync(
                    request.DriverId.Value,
                    request.StartDate,
                    request.EndDate);

                foreach (var pattern in patterns)
                {
                    report.AddRiskPattern(pattern);
                }
            }

            report.MarkAsCompleted();
            await _reportRepository.AddAsync(report);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Reporte generado: {report.Id} - {report.Title}");

            return MapToDTO(report);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generando reporte: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Exporta un reporte a un formato específico.
    /// </summary>
    public async Task<ReportDTO> ExportReportAsync(ExportReportDTO request)
    {
        var report = await _reportRepository.FindByIdAsync(request.ReportId);
        if (report == null)
            throw new InvalidOperationException($"Reporte con ID {request.ReportId} no encontrado");

        string exportUrl;
        var format = Enum.Parse<ExportFormat>(request.Format);

        exportUrl = format switch
        {
            ExportFormat.PDF => await _reportGeneratorService.ExportToPdfAsync(report),
            ExportFormat.Excel => await _reportGeneratorService.ExportToExcelAsync(report),
            _ => throw new ArgumentException($"Formato de exportación no soportado: {request.Format}")
        };

        report.MarkAsExported(format, exportUrl);
        await _reportRepository.UpdateAsync(report);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation($"Reporte {report.Id} exportado a {format}");

        return MapToDTO(report);
    }

    /// <summary>
    /// Obtiene un reporte por ID.
    /// </summary>
    public async Task<ReportDTO?> GetReportByIdAsync(int reportId)
    {
        var report = await _reportRepository.FindByIdAsync(reportId);
        return report != null ? MapToDTO(report) : null;
    }

    /// <summary>
    /// Obtiene reportes por gerente.
    /// </summary>
    public async Task<IEnumerable<ReportDTO>> GetReportsByManagerAsync(int managerId)
    {
        var reports = await _reportRepository.GetByGeneratedByIdAsync(managerId);
        return reports.Select(MapToDTO);
    }

    /// <summary>
    /// Obtiene reportes por conductor.
    /// </summary>
    public async Task<IEnumerable<ReportDTO>> GetReportsByDriverAsync(int driverId)
    {
        var reports = await _reportRepository.GetByDriverIdAsync(driverId);
        return reports.Select(MapToDTO);
    }

    private ReportDTO MapToDTO(Report report)
    {
        return new ReportDTO
        {
            Id = report.Id,
            ReportType = report.ReportType.ToString(),
            Title = report.Title,
            Description = report.Description,
            DriverId = report.DriverId,
            FleetId = report.FleetId,
            StartDate = report.StartDate,
            EndDate = report.EndDate,
            Status = report.Status.ToString(),
            ExportFormat = report.ExportFormat?.ToString(),
            ExportUrl = report.ExportUrl,
            GeneratedAt = report.GeneratedAt,
            ExportedAt = report.ExportedAt,
            Metrics = new ReportMetricsDTO
            {
                TotalTrips = report.Metrics.TotalTrips,
                CompletedTrips = report.Metrics.CompletedTrips,
                TotalAlerts = report.Metrics.TotalAlerts,
                CriticalAlerts = report.Metrics.CriticalAlerts,
                TotalDistanceKm = report.Metrics.TotalDistanceKm,
                TotalDrivingMinutes = report.Metrics.TotalDrivingMinutes,
                SafeTripsPercentage = report.Metrics.SafeTripsPercentage,
                AverageAlertsPerTrip = report.Metrics.AverageAlertsPerTrip,
                AverageSafetyScore = report.Metrics.AverageSafetyScore,
                UniqueDrivers = report.Metrics.UniqueDrivers,
                AlertsByType = report.Metrics.AlertsByType,
                AlertsByDay = report.Metrics.AlertsByDay
            },
            RiskPatterns = report.RiskPatterns.Select(p => new RiskPatternDTO
            {
                PatternType = p.PatternType.ToString(),
                Description = p.Description,
                OccurrenceCount = p.OccurrenceCount,
                SeverityLevel = p.SeverityLevel,
                ConfidenceScore = p.ConfidenceScore,
                StartHour = p.StartHour,
                EndHour = p.EndHour,
                DaysOfWeek = p.DaysOfWeek.Select(d => d.ToString()).ToList(),
                Recommendations = p.Recommendations
            }).ToList()
        };
    }
}
