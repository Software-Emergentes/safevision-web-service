using Microsoft.AspNetCore.Mvc;
using SafeVisionPlatform.Management.Application.Internal.DTO;
using SafeVisionPlatform.Management.Application.Internal.QueryServices;

namespace SafeVisionPlatform.Management.Interfaces.REST.Controllers;

/// <summary>
/// Controlador para consulta de patrones de riesgo.
/// </summary>
[ApiController]
[Route("api/risk-patterns")]
[Tags("Management - Risk Patterns")]
public class RiskPatternsController : ControllerBase
{
    private readonly RiskPatternQueryService _queryService;

    public RiskPatternsController(RiskPatternQueryService queryService)
    {
        _queryService = queryService;
    }

    /// <summary>
    /// Obtiene patrones de riesgo de un conductor.
    /// </summary>
    [HttpGet("driver/{driverId}")]
    [ProducesResponseType(typeof(IEnumerable<RiskPatternDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDriverRiskPatterns(
        int driverId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;

        var patterns = await _queryService.GetDriverRiskPatternsAsync(driverId, start, end);
        return Ok(patterns);
    }

    /// <summary>
    /// Obtiene patrones de riesgo de una flota.
    /// </summary>
    [HttpGet("fleet/{fleetId}")]
    [ProducesResponseType(typeof(IEnumerable<RiskPatternDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFleetRiskPatterns(
        int fleetId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;

        var patterns = await _queryService.GetFleetRiskPatternsAsync(fleetId, start, end);
        return Ok(patterns);
    }

    /// <summary>
    /// Obtiene recomendaciones basadas en patrones de riesgo.
    /// </summary>
    [HttpGet("driver/{driverId}/recommendations")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecommendations(
        int driverId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;

        var recommendations = await _queryService.GetRecommendationsAsync(driverId, start, end);
        return Ok(recommendations);
    }
}
