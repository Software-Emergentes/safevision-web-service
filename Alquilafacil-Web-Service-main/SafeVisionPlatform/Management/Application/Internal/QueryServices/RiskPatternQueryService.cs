using SafeVisionPlatform.Management.Application.Internal.DTO;
using SafeVisionPlatform.Management.Domain.Services;

namespace SafeVisionPlatform.Management.Application.Internal.QueryServices;

/// <summary>
/// Servicio de consultas para patrones de riesgo.
/// </summary>
public class RiskPatternQueryService
{
    private readonly IRiskAnalysisService _riskAnalysisService;
    private readonly ILogger<RiskPatternQueryService> _logger;

    public RiskPatternQueryService(
        IRiskAnalysisService riskAnalysisService,
        ILogger<RiskPatternQueryService> logger)
    {
        _riskAnalysisService = riskAnalysisService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene patrones de riesgo de un conductor.
    /// </summary>
    public async Task<IEnumerable<RiskPatternDTO>> GetDriverRiskPatternsAsync(
        int driverId,
        DateTime startDate,
        DateTime endDate)
    {
        var patterns = await _riskAnalysisService.AnalyzeDriverRiskPatternsAsync(
            driverId,
            startDate,
            endDate);

        return patterns.Select(p => new RiskPatternDTO
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
        });
    }

    /// <summary>
    /// Obtiene patrones de riesgo de una flota.
    /// </summary>
    public async Task<IEnumerable<RiskPatternDTO>> GetFleetRiskPatternsAsync(
        int fleetId,
        DateTime startDate,
        DateTime endDate)
    {
        var patterns = await _riskAnalysisService.AnalyzeFleetRiskPatternsAsync(
            fleetId,
            startDate,
            endDate);

        return patterns.Select(p => new RiskPatternDTO
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
        });
    }

    /// <summary>
    /// Genera recomendaciones basadas en patrones.
    /// </summary>
    public async Task<IEnumerable<string>> GetRecommendationsAsync(
        int driverId,
        DateTime startDate,
        DateTime endDate)
    {
        var patterns = await _riskAnalysisService.AnalyzeDriverRiskPatternsAsync(
            driverId,
            startDate,
            endDate);

        return _riskAnalysisService.GenerateRecommendations(patterns);
    }
}
