using SafeVisionPlatform.Shared.Domain.Repositories;
using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Domain.Repositories;
using SafeVisionPlatform.Trip.Domain.Services;

namespace SafeVisionPlatform.Trip.Application.Internal.Services;

/// <summary>
/// Implementación del servicio de feedback de alertas.
/// </summary>
public class AlertFeedbackService : IAlertFeedbackService
{
    private readonly IAlertRepository _alertRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AlertFeedbackService> _logger;

    public AlertFeedbackService(
        IAlertRepository alertRepository,
        IUnitOfWork unitOfWork,
        ILogger<AlertFeedbackService> logger)
    {
        _alertRepository = alertRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<FeedbackResponseDTO> SubmitFeedbackAsync(AlertFeedbackDTO feedbackDto)
    {
        try
        {
            var alert = await _alertRepository.FindByIdAsync(feedbackDto.AlertId);
            if (alert == null)
            {
                return new FeedbackResponseDTO
                {
                    AlertId = feedbackDto.AlertId,
                    Success = false,
                    Message = "Alerta no encontrada",
                    SubmittedAt = DateTime.UtcNow
                };
            }

            // Aplicar el feedback según el tipo
            if (feedbackDto.IsFalseAlarm)
            {
                alert.MarkAsFalseAlarm(feedbackDto.DriverId, feedbackDto.Comment);
                _logger.LogInformation($"Alerta {feedbackDto.AlertId} marcada como falsa alarma por conductor {feedbackDto.DriverId}");
            }
            else if (!string.IsNullOrWhiteSpace(feedbackDto.Comment))
            {
                alert.AddFeedback(feedbackDto.DriverId, feedbackDto.Comment);
                _logger.LogInformation($"Feedback agregado a alerta {feedbackDto.AlertId} por conductor {feedbackDto.DriverId}");
            }

            await _alertRepository.UpdateAsync(alert);
            await _unitOfWork.CompleteAsync();

            return new FeedbackResponseDTO
            {
                AlertId = feedbackDto.AlertId,
                Success = true,
                Message = feedbackDto.IsFalseAlarm
                    ? "Alerta marcada como falsa alarma exitosamente"
                    : "Feedback enviado exitosamente",
                SubmittedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al enviar feedback: {ex.Message}");
            return new FeedbackResponseDTO
            {
                AlertId = feedbackDto.AlertId,
                Success = false,
                Message = $"Error al enviar feedback: {ex.Message}",
                SubmittedAt = DateTime.UtcNow
            };
        }
    }

    public async Task<FeedbackStatisticsDTO> GetFeedbackStatisticsAsync()
    {
        var allAlerts = await _alertRepository.ListAsync();
        return CalculateStatistics(allAlerts.ToList());
    }

    public async Task<FeedbackStatisticsDTO> GetDriverFeedbackStatisticsAsync(int driverId)
    {
        var allAlerts = await _alertRepository.ListAsync();
        var driverAlerts = allAlerts.Where(a => a.FeedbackSubmittedBy == driverId).ToList();
        return CalculateStatistics(driverAlerts);
    }

    public async Task<IEnumerable<AlertWithFeedbackDTO>> GetRecentFeedbackAsync(int limit = 50)
    {
        var allAlerts = await _alertRepository.ListAsync();
        var alertsWithFeedback = allAlerts
            .Where(a => a.FeedbackSubmittedAt.HasValue)
            .OrderByDescending(a => a.FeedbackSubmittedAt)
            .Take(limit)
            .ToList();

        return alertsWithFeedback.Select(ToAlertWithFeedbackDTO);
    }

    public async Task<IEnumerable<AlertWithFeedbackDTO>> GetFalseAlarmsAsync()
    {
        var allAlerts = await _alertRepository.ListAsync();
        var falseAlarms = allAlerts
            .Where(a => a.MarkedAsFalseAlarm)
            .OrderByDescending(a => a.FeedbackSubmittedAt)
            .ToList();

        return falseAlarms.Select(ToAlertWithFeedbackDTO);
    }

    private FeedbackStatisticsDTO CalculateStatistics(List<Domain.Model.Entities.Alert> alerts)
    {
        var alertsWithFeedback = alerts.Where(a => a.FeedbackSubmittedAt.HasValue).ToList();
        var falseAlarms = alertsWithFeedback.Where(a => a.MarkedAsFalseAlarm).ToList();

        var totalEvaluated = alertsWithFeedback.Count;
        var totalFalseAlarms = falseAlarms.Count;
        var falseAlarmPercentage = totalEvaluated > 0
            ? (double)totalFalseAlarms / totalEvaluated * 100
            : 0;

        // Distribución por tipo
        var falseAlarmsByType = falseAlarms
            .GroupBy(a => (int)a.AlertType)
            .ToDictionary(g => g.Key, g => g.Count());

        // Feedback recientes
        var recentFeedback = alertsWithFeedback
            .OrderByDescending(a => a.FeedbackSubmittedAt)
            .Take(10)
            .Select(ToAlertWithFeedbackDTO)
            .ToList();

        return new FeedbackStatisticsDTO
        {
            TotalAlertsEvaluated = totalEvaluated,
            TotalFalseAlarms = totalFalseAlarms,
            FalseAlarmPercentage = falseAlarmPercentage,
            FalseAlarmsByType = falseAlarmsByType,
            RecentFeedback = recentFeedback
        };
    }

    private AlertWithFeedbackDTO ToAlertWithFeedbackDTO(Domain.Model.Entities.Alert alert)
    {
        return new AlertWithFeedbackDTO
        {
            AlertId = alert.Id,
            TripId = alert.TripId,
            AlertType = (int)alert.AlertType,
            Description = alert.Description,
            DetectedAt = alert.DetectedAt,
            MarkedAsFalseAlarm = alert.MarkedAsFalseAlarm,
            FeedbackComment = alert.FeedbackComment,
            FeedbackSubmittedAt = alert.FeedbackSubmittedAt,
            FeedbackSubmittedBy = alert.FeedbackSubmittedBy
        };
    }
}
