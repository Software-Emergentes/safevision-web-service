using SafeVisionPlatform.FatigueMonitoring.Domain.Model.Entities;
using SafeVisionPlatform.FatigueMonitoring.Domain.Model.ValueObjects;

namespace SafeVisionPlatform.FatigueMonitoring.Domain.Services;

/// <summary>
/// Servicio de dominio que detecta la fatiga analizando datos de sensores
/// y calcula la severidad de los eventos.
/// </summary>
public interface IFatigueDetectionService
{
    /// <summary>
    /// Analiza los datos del sensor y determina si hay fatiga.
    /// </summary>
    Task<DrowsinessEvent?> AnalyzeSensorDataAsync(
        int driverId,
        int tripId,
        int monitoringSessionId,
        SensorData sensorData);

    /// <summary>
    /// Calcula la severidad de un evento de fatiga basado en los datos del sensor.
    /// </summary>
    SeverityScore CalculateSeverity(SensorData sensorData, DrowsinessEventType eventType);

    /// <summary>
    /// Determina el tipo de evento de somnolencia basado en los datos del sensor.
    /// </summary>
    DrowsinessEventType? DetermineEventType(SensorData sensorData);

    /// <summary>
    /// Verifica si se debe generar una alerta crítica basada en eventos recientes.
    /// </summary>
    Task<bool> ShouldGenerateCriticalAlertAsync(int driverId, int tripId);
}
