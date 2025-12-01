using Microsoft.AspNetCore.Mvc;
using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Domain.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeVisionPlatform.Trip.Interfaces.REST.Controllers;

/// <summary>
/// Controlador REST para el dashboard de flota.
/// Proporciona información en tiempo real sobre el estado de la flota.
/// </summary>
[ApiController]
[Route("api/fleet/dashboard")]
[SwaggerTag("Fleet Dashboard - Panel de control de flota")]
public class FleetDashboardController : ControllerBase
{
    private readonly IFleetDashboardService _fleetDashboardService;
    private readonly ILogger<FleetDashboardController> _logger;

    public FleetDashboardController(
        IFleetDashboardService fleetDashboardService,
        ILogger<FleetDashboardController> logger)
    {
        _fleetDashboardService = fleetDashboardService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene el dashboard completo de la flota con información en tiempo real.
    /// </summary>
    /// <remarks>
    /// Devuelve información agregada sobre:
    /// - Conductores y viajes activos
    /// - Alertas del día
    /// - Conductores en riesgo
    /// - Estadísticas de la flota
    /// </remarks>
    [HttpGet]
    [SwaggerOperation(Summary = "Obtener dashboard completo de la flota")]
    [Produces("application/json")]
    public async Task<ActionResult<FleetDashboardDTO>> GetFleetDashboard()
    {
        try
        {
            _logger.LogInformation("Obteniendo dashboard de flota");
            var dashboard = await _fleetDashboardService.GetFleetDashboardAsync();
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener dashboard de flota: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene la lista de conductores actualmente en riesgo.
    /// </summary>
    /// <remarks>
    /// Devuelve conductores con alertas críticas recientes ordenados por nivel de riesgo.
    /// </remarks>
    [HttpGet("drivers-at-risk")]
    [SwaggerOperation(Summary = "Obtener conductores en riesgo")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<DriverAtRiskDTO>>> GetDriversAtRisk()
    {
        try
        {
            _logger.LogInformation("Obteniendo conductores en riesgo");
            var driversAtRisk = await _fleetDashboardService.GetDriversAtRiskAsync();
            return Ok(driversAtRisk);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener conductores en riesgo: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene estadísticas de la flota para un rango de fechas.
    /// </summary>
    /// <remarks>
    /// Devuelve métricas agregadas como distancia total, tiempo de conducción,
    /// promedio de alertas por viaje y porcentaje de viajes seguros.
    /// </remarks>
    [HttpGet("statistics")]
    [SwaggerOperation(Summary = "Obtener estadísticas de la flota")]
    [Produces("application/json")]
    public async Task<ActionResult<FleetStatisticsDTO>> GetFleetStatistics(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            _logger.LogInformation($"Obteniendo estadísticas de flota del {startDate:yyyy-MM-dd} al {endDate:yyyy-MM-dd}");
            var statistics = await _fleetDashboardService.GetFleetStatisticsAsync(startDate, endDate);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener estadísticas de flota: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
}
