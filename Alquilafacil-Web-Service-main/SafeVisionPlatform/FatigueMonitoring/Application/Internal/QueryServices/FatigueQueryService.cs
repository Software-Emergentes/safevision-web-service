using SafeVisionPlatform.FatigueMonitoring.Application.Internal.DTO;
using SafeVisionPlatform.FatigueMonitoring.Domain.Model.Entities;
using SafeVisionPlatform.FatigueMonitoring.Domain.Repositories;

namespace SafeVisionPlatform.FatigueMonitoring.Application.Internal.QueryServices;

/// <summary>
/// Servicio de consultas para fatiga y alertas.
/// </summary>
public class FatigueQueryService
{
    private readonly IDrowsinessEventRepository _drowsinessEventRepository;
    private readonly ICriticalAlertRepository _criticalAlertRepository;
    private readonly ILogger<FatigueQueryService> _logger;

    public FatigueQueryService(
        IDrowsinessEventRepository drowsinessEventRepository,
        ICriticalAlertRepository criticalAlertRepository,
        ILogger<FatigueQueryService> logger)
    {
        _drowsinessEventRepository = drowsinessEventRepository;
        _criticalAlertRepository = criticalAlertRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene el estado actual de fatiga de un conductor.
    /// </summary>
    public async Task<FatigueStatusDTO> GetFatigueStatusAsync(int driverId)
    {
        var recentEvents = await _drowsinessEventRepository.GetByDriverIdAsync(driverId);
        var last24HoursEvents = recentEvents
            .Where(e => e.DetectedAt >= DateTime.UtcNow.AddHours(-24))
            .OrderByDescending(e => e.DetectedAt)
            .ToList();

        var pendingAlerts = await _criticalAlertRepository.GetByDriverIdAsync(driverId);
        var hasActiveAlert = pendingAlerts.Any(a =>
            a.Status == CriticalAlertStatus.Pending ||
            a.Status == CriticalAlertStatus.Sent);

        var currentSeverity = last24HoursEvents.Any()
            ? last24HoursEvents.Average(e => e.Severity.Value)
            : 0;

        var status = currentSeverity switch
        {
            >= 0.70 => "Critical",
            >= 0.50 => "Warning",
            _ => "Normal"
        };

        var lastEvent = last24HoursEvents.FirstOrDefault();

        return new FatigueStatusDTO
        {
            DriverId = driverId,
            DriverName = $"Conductor #{driverId}",
            CurrentStatus = status,
            CurrentSeverity = currentSeverity,
            RecentEventsCount = last24HoursEvents.Count,
            LastEventTime = lastEvent?.DetectedAt,
            LastEventType = lastEvent?.EventType.ToString(),
            HasActiveAlert = hasActiveAlert,
            CurrentTripId = lastEvent?.TripId
        };
    }

    /// <summary>
    /// Obtiene reportes de alertas críticas.
    /// </summary>
    public async Task<IEnumerable<CriticalAlertReportDTO>> GetAlertReportsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;

        var alerts = await _criticalAlertRepository.GetByDateRangeAsync(start, end);

        return alerts.Select(MapAlertToDTO);
    }

    /// <summary>
    /// Obtiene reportes de alertas por conductor.
    /// </summary>
    public async Task<IEnumerable<CriticalAlertReportDTO>> GetAlertReportsByDriverAsync(int driverId)
    {
        var alerts = await _criticalAlertRepository.GetByDriverIdAsync(driverId);
        return alerts.Select(MapAlertToDTO);
    }

    /// <summary>
    /// Obtiene alertas pendientes para un gerente.
    /// </summary>
    public async Task<IEnumerable<CriticalAlertReportDTO>> GetPendingAlertsForManagerAsync(int managerId)
    {
        var alerts = await _criticalAlertRepository.GetPendingAlertsByManagerIdAsync(managerId);
        return alerts.Select(MapAlertToDTO);
    }

    /// <summary>
    /// Obtiene métricas agregadas de alertas.
    /// </summary>
    public async Task<AlertMetricsDTO> GetAlertMetricsAsync(DateTime startDate, DateTime endDate)
    {
        var alerts = await _criticalAlertRepository.GetByDateRangeAsync(startDate, endDate);
        var alertsList = alerts.ToList();

        var acknowledgedAlerts = alertsList.Where(a => a.AcknowledgedAt.HasValue).ToList();
        var avgResponseTime = acknowledgedAlerts.Any()
            ? acknowledgedAlerts.Average(a => (a.AcknowledgedAt!.Value - a.GeneratedAt).TotalMinutes)
            : 0;

        return new AlertMetricsDTO
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalAlerts = alertsList.Count,
            PendingAlerts = alertsList.Count(a => a.Status == CriticalAlertStatus.Pending || a.Status == CriticalAlertStatus.Sent),
            AcknowledgedAlerts = alertsList.Count(a => a.Status == CriticalAlertStatus.Acknowledged),
            ResolvedAlerts = alertsList.Count(a => a.Status == CriticalAlertStatus.Resolved),
            AlertsByType = alertsList.GroupBy(a => a.AlertType.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            AlertsBySeverity = alertsList.GroupBy(a => a.Severity.Level)
                .ToDictionary(g => g.Key, g => g.Count()),
            AverageResponseTimeMinutes = avgResponseTime
        };
    }

    /// <summary>
    /// Obtiene eventos de somnolencia por viaje.
    /// </summary>
    public async Task<IEnumerable<DrowsinessEventDTO>> GetDrowsinessEventsByTripAsync(int tripId)
    {
        var events = await _drowsinessEventRepository.GetByTripIdAsync(tripId);
        return events.Select(MapEventToDTO);
    }

    private CriticalAlertReportDTO MapAlertToDTO(CriticalAlert alert)
    {
        return new CriticalAlertReportDTO
        {
            AlertId = alert.Id,
            DriverId = alert.DriverId,
            DriverName = $"Conductor #{alert.DriverId}",
            TripId = alert.TripId,
            AlertType = alert.AlertType.ToString(),
            SeverityLevel = alert.Severity.Level,
            SeverityValue = alert.Severity.Value,
            Message = alert.Message,
            Status = alert.Status.ToString(),
            DrowsinessEventsCount = alert.DrowsinessEventsCount,
            GeneratedAt = alert.GeneratedAt,
            SentAt = alert.SentAt,
            AcknowledgedAt = alert.AcknowledgedAt,
            ActionTaken = alert.ActionTaken
        };
    }

    private DrowsinessEventDTO MapEventToDTO(DrowsinessEvent ev)
    {
        return new DrowsinessEventDTO
        {
            Id = ev.Id,
            DriverId = ev.DriverId,
            TripId = ev.TripId,
            EventType = ev.EventType.ToString(),
            SeverityValue = ev.Severity.Value,
            SeverityLevel = ev.Severity.Level,
            DetectedAt = ev.DetectedAt,
            Processed = ev.Processed,
            SensorData = new SensorDataDTO
            {
                BlinkRate = ev.SensorData.BlinkRate,
                EyeOpenness = ev.SensorData.EyeOpenness,
                MouthOpenness = ev.SensorData.MouthOpenness,
                HeadTilt = ev.SensorData.HeadTilt,
                EyeClosureDuration = ev.SensorData.EyeClosureDuration,
                CapturedAt = ev.SensorData.CapturedAt
            }
        };
    }
}
