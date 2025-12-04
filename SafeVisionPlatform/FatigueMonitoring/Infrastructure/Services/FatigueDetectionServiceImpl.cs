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
        // Valores ajustados para generar los 4 niveles
        double baseScore = eventType switch
        {
            DrowsinessEventType.MicroSleep => 0.92,      // Critical: 0.92-1.0
            DrowsinessEventType.EyeClosure => 0.72,      // High: 0.72-0.82
            DrowsinessEventType.HeadNod => 0.58,         // Medium: 0.58-0.68
            DrowsinessEventType.Yawn => 0.52,            // Medium: 0.52-0.62
            DrowsinessEventType.Blink => 0.35,           // Low: 0.35-0.45
            _ => 0.3
        };
        
        var eyeClosurePenalty = sensorData.EyeClosureDuration > 2.5 ? 0.05 : 
            sensorData.EyeClosureDuration > 2 ? 0.03 : 0;
        var lowOpennessPenalty = sensorData.EyeOpenness < 0.25 ? 0.03 : 0;
        var headTiltPenalty = sensorData.HeadTilt > 20 ? 0.02 : 0;

        var finalScore = Math.Min(baseScore + eyeClosurePenalty + lowOpennessPenalty + headTiltPenalty, 1.0);
    
        return new SeverityScore(finalScore);
    }

    public DrowsinessEventType? DetermineEventType(SensorData sensorData)
    {
        // Priorizar detección por gravedad con condiciones más específicas
    
        // MicroSleep solo si eyeClosureDuration >= 3 segundos
        if (sensorData.EyeClosureDuration >= 3 || 
            (sensorData.EyeOpenness < 15 && sensorData.HeadTilt > 20))
            return DrowsinessEventType.MicroSleep;
    
        // EyeClosure para 2-2.9 segundos (HIGH severity)
        if (sensorData.EyeClosureDuration >= 2)
            return DrowsinessEventType.EyeClosure;
    
        // HeadNod para inclinación alta sin bostezo
        if (sensorData.HeadTilt > 15 && sensorData.MouthOpenness < 60)
            return DrowsinessEventType.HeadNod;
    
        // Yawn tiene MÁXIMA prioridad si mouthOpenness > 60 (MEDIUM severity)
        if (sensorData.MouthOpenness > 60)
            return DrowsinessEventType.Yawn;
    
        // Blink para parpadeo excesivo (LOW severity)
        if (sensorData.EyeOpenness < 50 || sensorData.BlinkRate > 25 || sensorData.EyeClosureDuration > 1)
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

        if (unprocessedEvents.Any() && unprocessedEvents
                .Where(e => e.Severity != null)
                .Select(e => e.Severity.Value)
                .DefaultIfEmpty(0)
                .Average() >= 0.7)
            return true;

        return false;
    }
}
