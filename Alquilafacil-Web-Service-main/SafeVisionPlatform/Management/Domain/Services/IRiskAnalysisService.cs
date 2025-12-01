using SafeVisionPlatform.Management.Domain.Model.ValueObjects;

namespace SafeVisionPlatform.Management.Domain.Services;

/// <summary>
/// Servicio de dominio que aplica reglas de negocio para identificar patrones de riesgo
/// a partir de los datos de viajes y alertas de conductores.
/// </summary>
public interface IRiskAnalysisService
{
    /// <summary>
    /// Analiza patrones de riesgo para un conductor.
    /// </summary>
    Task<IEnumerable<RiskPattern>> AnalyzeDriverRiskPatternsAsync(
        int driverId,
        DateTime startDate,
        DateTime endDate);

    /// <summary>
    /// Analiza patrones de riesgo para una flota.
    /// </summary>
    Task<IEnumerable<RiskPattern>> AnalyzeFleetRiskPatternsAsync(
        int fleetId,
        DateTime startDate,
        DateTime endDate);

    /// <summary>
    /// Identifica patrones por hora del día.
    /// </summary>
    Task<IEnumerable<RiskPattern>> IdentifyTimeOfDayPatternsAsync(
        int driverId,
        DateTime startDate,
        DateTime endDate);

    /// <summary>
    /// Identifica patrones por día de la semana.
    /// </summary>
    Task<IEnumerable<RiskPattern>> IdentifyDayOfWeekPatternsAsync(
        int driverId,
        DateTime startDate,
        DateTime endDate);

    /// <summary>
    /// Genera recomendaciones basadas en patrones.
    /// </summary>
    IEnumerable<string> GenerateRecommendations(IEnumerable<RiskPattern> patterns);
}
