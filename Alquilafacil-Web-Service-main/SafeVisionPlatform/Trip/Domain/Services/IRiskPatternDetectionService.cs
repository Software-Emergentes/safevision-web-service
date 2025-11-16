using SafeVisionPlatform.Trip.Application.Internal.DTO;

namespace SafeVisionPlatform.Trip.Domain.Services;

/// <summary>
/// Servicio de dominio para detección de patrones de riesgo en conductores.
/// Analiza datos históricos para identificar patrones recurrentes de somnolencia.
/// </summary>
public interface IRiskPatternDetectionService
{
    /// <summary>
    /// Analiza los patrones de riesgo de un conductor específico.
    /// </summary>
    Task<DriverRiskAnalysisDTO> AnalyzeDriverRiskPatternsAsync(RiskPatternAnalysisRequestDTO request);

    /// <summary>
    /// Analiza los patrones de riesgo de toda la flota.
    /// </summary>
    Task<FleetRiskPatternsDTO> AnalyzeFleetRiskPatternsAsync(RiskPatternAnalysisRequestDTO request);

    /// <summary>
    /// Obtiene conductores con patrones de alto riesgo.
    /// </summary>
    Task<IEnumerable<DriverRiskAnalysisDTO>> GetHighRiskDriversAsync(DateTime startDate, DateTime endDate, double minRiskScore = 70.0);

    /// <summary>
    /// Obtiene patrones de riesgo por horario para un conductor.
    /// </summary>
    Task<IEnumerable<TimeOfDayRiskPatternDTO>> GetTimeOfDayPatternsAsync(int driverId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Obtiene patrones de riesgo por día de la semana para un conductor.
    /// </summary>
    Task<IEnumerable<DayOfWeekRiskPatternDTO>> GetDayOfWeekPatternsAsync(int driverId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Obtiene patrones de riesgo por duración de viaje para un conductor.
    /// </summary>
    Task<IEnumerable<TripDurationRiskPatternDTO>> GetTripDurationPatternsAsync(int driverId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Genera notificaciones automáticas para gerentes sobre patrones de riesgo detectados.
    /// </summary>
    Task NotifyManagersAboutRiskPatternsAsync(int driverId, DateTime startDate, DateTime endDate);
}
