using SafeVisionPlatform.FatigueMonitoring.Application.Internal.DTO;
using SafeVisionPlatform.FatigueMonitoring.Domain.Model.Entities;
using SafeVisionPlatform.FatigueMonitoring.Domain.Model.ValueObjects;
using SafeVisionPlatform.FatigueMonitoring.Domain.Repositories;
using SafeVisionPlatform.FatigueMonitoring.Domain.Services;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.FatigueMonitoring.Application.Internal.CommandServices;

/// <summary>
/// Servicio de comandos para la detección de fatiga.
/// Implementa los casos de uso DetectFatigueUseCase y GenerateCriticalAlertUseCase.
/// </summary>
public class FatigueDetectionCommandService
{
    private readonly IFatigueDetectionService _fatigueDetectionService;
    private readonly IAlertGenerationService _alertGenerationService;
    private readonly IDrowsinessEventRepository _drowsinessEventRepository;
    private readonly ICriticalAlertRepository _criticalAlertRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FatigueDetectionCommandService> _logger;

    public FatigueDetectionCommandService(
        IFatigueDetectionService fatigueDetectionService,
        IAlertGenerationService alertGenerationService,
        IDrowsinessEventRepository drowsinessEventRepository,
        ICriticalAlertRepository criticalAlertRepository,
        IUnitOfWork unitOfWork,
        ILogger<FatigueDetectionCommandService> logger)
    {
        _fatigueDetectionService = fatigueDetectionService;
        _alertGenerationService = alertGenerationService;
        _drowsinessEventRepository = drowsinessEventRepository;
        _criticalAlertRepository = criticalAlertRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Procesa datos del sensor y detecta fatiga.
    /// </summary>
    public async Task<DrowsinessEventDTO?> ProcessSensorDataAsync(SensorDataInputDTO input)
    {
        try
        {
            var sensorData = new SensorData(
                input.BlinkRate,
                input.EyeOpenness,
                input.MouthOpenness,
                input.HeadTilt,
                input.EyeClosureDuration);

            var drowsinessEvent = await _fatigueDetectionService.AnalyzeSensorDataAsync(
                input.DriverId,
                input.TripId,
                input.MonitoringSessionId,
                sensorData);

            if (drowsinessEvent != null)
            {
                await _drowsinessEventRepository.AddAsync(drowsinessEvent);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation(
                    $"Evento de somnolencia detectado: Driver {input.DriverId}, Tipo {drowsinessEvent.EventType}");

                // Verificar si se debe generar alerta crítica
                if (await _fatigueDetectionService.ShouldGenerateCriticalAlertAsync(input.DriverId, input.TripId))
                {
                    await GenerateCriticalAlertAsync(input.DriverId, input.TripId, null);
                }

                return MapToDTO(drowsinessEvent);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error procesando datos del sensor: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Genera una alerta crítica manualmente.
    /// </summary>
    public async Task<CriticalAlertReportDTO> GenerateCriticalAlertAsync(
        int driverId,
        int tripId,
        int? managerId)
    {
        try
        {
            var recentEvents = await _drowsinessEventRepository.GetByTripIdAsync(tripId);
            var unprocessedEvents = recentEvents.Where(e => !e.Processed).ToList();

            if (!unprocessedEvents.Any())
            {
                throw new InvalidOperationException("No hay eventos de somnolencia para generar alerta");
            }

            var alert = await _alertGenerationService.GenerateAlertAsync(
                driverId,
                tripId,
                managerId,
                unprocessedEvents);

            await _criticalAlertRepository.AddAsync(alert);

            // Marcar eventos como procesados
            foreach (var ev in unprocessedEvents)
            {
                ev.MarkAsProcessed();
            }

            await _unitOfWork.CompleteAsync();

            // Enviar notificación
            await _alertGenerationService.SendAlertNotificationAsync(alert);
            alert.MarkAsSent();
            await _criticalAlertRepository.UpdateAsync(alert);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Alerta crítica generada: AlertId {alert.Id}, Driver {driverId}");

            return MapAlertToDTO(alert);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generando alerta crítica: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Reconoce una alerta crítica.
    /// </summary>
    public async Task AcknowledgeAlertAsync(int alertId, int userId, string? actionTaken = null)
    {
        var alert = await _criticalAlertRepository.FindByIdAsync(alertId);
        if (alert == null)
            throw new InvalidOperationException($"Alerta con ID {alertId} no encontrada");

        alert.Acknowledge(userId, actionTaken);
        await _criticalAlertRepository.UpdateAsync(alert);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation($"Alerta {alertId} reconocida por usuario {userId}");
    }

    /// <summary>
    /// Resuelve una alerta crítica.
    /// </summary>
    public async Task ResolveAlertAsync(int alertId, string actionTaken)
    {
        var alert = await _criticalAlertRepository.FindByIdAsync(alertId);
        if (alert == null)
            throw new InvalidOperationException($"Alerta con ID {alertId} no encontrada");

        alert.Resolve(actionTaken);
        await _criticalAlertRepository.UpdateAsync(alert);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation($"Alerta {alertId} resuelta");
    }

    private DrowsinessEventDTO MapToDTO(DrowsinessEvent ev)
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
}
