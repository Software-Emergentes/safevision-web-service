using SafeVisionPlatform.FatigueMonitoring.Domain.Model.Entities;
using SafeVisionPlatform.FatigueMonitoring.Domain.Model.ValueObjects;
using SafeVisionPlatform.FatigueMonitoring.Domain.Repositories;
using SafeVisionPlatform.FatigueMonitoring.Domain.Services;

namespace SafeVisionPlatform.FatigueMonitoring.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de detección de fatiga.
/// </summary>
public class FatigueDetectionServiceImpl : IFatigueDetectionService
{
    private readonly IDrowsinessEventRepository _drowsinessEventRepository;
    private readonly ILogger<FatigueDetectionServiceImpl> _logger;

    public FatigueDetectionServiceImpl(
        IDrowsinessEventRepository drowsinessEventRepository,
        ILogger<FatigueDetectionServiceImpl> logger)
    {
        _drowsinessEventRepository = drowsinessEventRepository;
        _logger = logger;
    }

    public async Task<DrowsinessEvent?> AnalyzeSensorDataAsync(
        int driverId,
        int tripId,
        int monitoringSessionId,
        SensorData sensorData)
    {
        var eventType = DetermineEventType(sensorData);

        if (eventType == null)
        {
            return null; // No se detectó fatiga
        }

        var severity = CalculateSeverity(sensorData, eventType.Value);

        var drowsinessEvent = new DrowsinessEvent(
            driverId,
            tripId,
            monitoringSessionId,
            eventType.Value,
            sensorData,
            severity);

        _logger.LogInformation(
            $"Fatiga detectada: Driver {driverId}, Tipo {eventType}, Severidad {severity.Value:F2}");

        return await Task.FromResult(drowsinessEvent);
    }

    public SeverityScore CalculateSeverity(SensorData sensorData, DrowsinessEventType eventType)
    {
        double baseScore = eventType switch
        {
            DrowsinessEventType.MicroSleep => 0.9,
            DrowsinessEventType.EyeClosure => 0.75,
            DrowsinessEventType.HeadNod => 0.65,
            DrowsinessEventType.Yawn => 0.45,
            DrowsinessEventType.Blink => 0.35,
            _ => 0.3
        };

        // Ajustar según datos del sensor
        var eyeClosurePenalty = sensorData.EyeClosureDuration > 2 ? 0.1 : 0;
        var lowOpennessPenalty = sensorData.EyeOpenness < 30 ? 0.05 : 0;
        var headTiltPenalty = sensorData.HeadTilt > 20 ? 0.05 : 0;

        var finalScore = Math.Min(baseScore + eyeClosurePenalty + lowOpennessPenalty + headTiltPenalty, 1.0);

        return new SeverityScore(finalScore);
    }

    public DrowsinessEventType? DetermineEventType(SensorData sensorData)
    {
        // Priorizar detección por gravedad
        if (sensorData.IndicatesMicroSleep())
            return DrowsinessEventType.MicroSleep;

        if (sensorData.EyeClosureDuration >= 2)
            return DrowsinessEventType.EyeClosure;

        if (sensorData.HeadTilt > 15)
            return DrowsinessEventType.HeadNod;

        if (sensorData.IndicatesYawning())
            return DrowsinessEventType.Yawn;

        if (sensorData.IndicatesDrowsiness())
            return DrowsinessEventType.Blink;

        return null; // Sin fatiga detectada
    }

    public async Task<bool> ShouldGenerateCriticalAlertAsync(int driverId, int tripId)
    {
        var recentEvents = await _drowsinessEventRepository.GetByTripIdAsync(tripId);
        var unprocessedEvents = recentEvents.Where(e => !e.Processed).ToList();

        // Generar alerta si:
        // 1. Hay un evento de micro-sueño
        // 2. Hay 3 o más eventos no procesados
        // 3. La severidad promedio es >= 0.7

        if (unprocessedEvents.Any(e => e.EventType == DrowsinessEventType.MicroSleep))
            return true;

        if (unprocessedEvents.Count >= 3)
            return true;

        if (unprocessedEvents.Any() && unprocessedEvents.Average(e => e.Severity.Value) >= 0.7)
            return true;

        return false;
    }
}
