using Microsoft.AspNetCore.Mvc;
using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Domain.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeVisionPlatform.Trip.Interfaces.REST.Controllers;

/// <summary>
/// Controlador REST para el historial personal de conductores.
/// Permite a los conductores consultar su historial de viajes y alertas.
/// </summary>
[ApiController]
[Route("api/drivers/{driverId}/history")]
[SwaggerTag("Driver History - Historial personal de conductores")]
public class DriverHistoryController : ControllerBase
{
    private readonly IDriverHistoryService _driverHistoryService;
    private readonly ILogger<DriverHistoryController> _logger;

    public DriverHistoryController(
        IDriverHistoryService driverHistoryService,
        ILogger<DriverHistoryController> logger)
    {
        _driverHistoryService = driverHistoryService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene el historial completo de un conductor.
    /// </summary>
    /// <remarks>
    /// Devuelve:
    /// - Todos los viajes del conductor
    /// - Todas las alertas recibidas
    /// - Estadísticas agregadas
    /// - Análisis de patrones de fatiga
    /// </remarks>
    [HttpGet]
    [SwaggerOperation(Summary = "Obtener historial completo de un conductor")]
    [Produces("application/json")]
    public async Task<ActionResult<DriverHistoryDTO>> GetDriverHistory(int driverId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo historial del conductor {driverId}");
            var history = await _driverHistoryService.GetDriverHistoryAsync(driverId);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener historial del conductor: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene el historial de un conductor filtrado por rango de fechas.
    /// </summary>
    /// <remarks>
    /// Permite filtrar el historial por un período específico.
    /// </remarks>
    [HttpGet("date-range")]
    [SwaggerOperation(Summary = "Obtener historial de conductor por rango de fechas")]
    [Produces("application/json")]
    public async Task<ActionResult<DriverHistoryDTO>> GetDriverHistoryByDateRange(
        int driverId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            _logger.LogInformation($"Obteniendo historial del conductor {driverId} del {startDate:yyyy-MM-dd} al {endDate:yyyy-MM-dd}");
            var history = await _driverHistoryService.GetDriverHistoryByDateRangeAsync(driverId, startDate, endDate);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener historial del conductor por rango de fechas: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene el análisis de patrones de fatiga de un conductor.
    /// </summary>
    /// <remarks>
    /// Devuelve:
    /// - Horas del día con más alertas
    /// - Distribución horaria de alertas
    /// - Tipo de alerta más frecuente
    /// - Porcentaje de viajes seguros
    /// </remarks>
    [HttpGet("fatigue-patterns")]
    [SwaggerOperation(Summary = "Obtener análisis de patrones de fatiga")]
    [Produces("application/json")]
    public async Task<ActionResult<FatiguePatternDTO>> GetFatiguePatterns(int driverId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo patrones de fatiga del conductor {driverId}");
            var patterns = await _driverHistoryService.GetDriverFatiguePatternsAsync(driverId);
            return Ok(patterns);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener patrones de fatiga: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
}
