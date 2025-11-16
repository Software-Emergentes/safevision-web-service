using Microsoft.AspNetCore.Mvc;
using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Domain.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeVisionPlatform.Trip.Interfaces.REST.Controllers;

/// <summary>
/// Controlador REST para comparación de conductores.
/// Permite a los gerentes comparar el desempeño de diferentes conductores.
/// </summary>
[ApiController]
[Route("api/drivers/comparison")]
[SwaggerTag("Driver Comparison - Comparación de desempeño entre conductores")]
public class DriverComparisonController : ControllerBase
{
    private readonly IDriverComparisonService _comparisonService;
    private readonly ILogger<DriverComparisonController> _logger;

    public DriverComparisonController(
        IDriverComparisonService comparisonService,
        ILogger<DriverComparisonController> logger)
    {
        _comparisonService = comparisonService;
        _logger = logger;
    }

    /// <summary>
    /// Compara múltiples conductores específicos en un rango de fechas.
    /// </summary>
    /// <param name="driverIds">Lista de IDs de conductores a comparar</param>
    /// <param name="startDate">Fecha de inicio del período de análisis</param>
    /// <param name="endDate">Fecha de fin del período de análisis</param>
    /// <returns>Comparación detallada de los conductores seleccionados</returns>
    [HttpPost("compare")]
    [SwaggerOperation(Summary = "Comparar conductores específicos")]
    [Produces("application/json")]
    public async Task<ActionResult<DriverComparisonDTO>> CompareDrivers(
        [FromBody] List<int> driverIds,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (driverIds == null || !driverIds.Any())
            {
                return BadRequest(new { error = "Debe proporcionar al menos un ID de conductor" });
            }

            if (driverIds.Count > 20)
            {
                return BadRequest(new { error = "No se pueden comparar más de 20 conductores a la vez" });
            }

            if (startDate >= endDate)
            {
                return BadRequest(new { error = "La fecha de inicio debe ser anterior a la fecha de fin" });
            }

            if ((endDate - startDate).TotalDays > 365)
            {
                return BadRequest(new { error = "El rango de fechas no puede exceder 365 días" });
            }

            _logger.LogInformation($"Comparando {driverIds.Count} conductores desde {startDate:yyyy-MM-dd} hasta {endDate:yyyy-MM-dd}");

            var comparison = await _comparisonService.CompareDriversAsync(driverIds, startDate, endDate);

            return Ok(comparison);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al comparar conductores: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor al comparar conductores" });
        }
    }

    /// <summary>
    /// Compara todos los conductores de la flota en un rango de fechas.
    /// </summary>
    /// <param name="startDate">Fecha de inicio del período de análisis</param>
    /// <param name="endDate">Fecha de fin del período de análisis</param>
    /// <returns>Comparación de todos los conductores de la flota</returns>
    [HttpGet("compare/all")]
    [SwaggerOperation(Summary = "Comparar todos los conductores de la flota")]
    [Produces("application/json")]
    public async Task<ActionResult<DriverComparisonDTO>> CompareAllDrivers(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate >= endDate)
            {
                return BadRequest(new { error = "La fecha de inicio debe ser anterior a la fecha de fin" });
            }

            if ((endDate - startDate).TotalDays > 365)
            {
                return BadRequest(new { error = "El rango de fechas no puede exceder 365 días" });
            }

            _logger.LogInformation($"Comparando todos los conductores desde {startDate:yyyy-MM-dd} hasta {endDate:yyyy-MM-dd}");

            var comparison = await _comparisonService.CompareAllDriversAsync(startDate, endDate);

            return Ok(comparison);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al comparar todos los conductores: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor al comparar conductores" });
        }
    }

    /// <summary>
    /// Obtiene el ranking de conductores ordenados por puntuación de seguridad.
    /// </summary>
    /// <param name="startDate">Fecha de inicio del período de análisis</param>
    /// <param name="endDate">Fecha de fin del período de análisis</param>
    /// <param name="limit">Número máximo de conductores en el ranking (predeterminado: 10, máximo: 50)</param>
    /// <returns>Lista de conductores rankeados por desempeño</returns>
    [HttpGet("rankings")]
    [SwaggerOperation(Summary = "Obtener ranking de conductores por seguridad")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<DriverRankingDTO>>> GetDriverRankings(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int limit = 10)
    {
        try
        {
            if (startDate >= endDate)
            {
                return BadRequest(new { error = "La fecha de inicio debe ser anterior a la fecha de fin" });
            }

            if ((endDate - startDate).TotalDays > 365)
            {
                return BadRequest(new { error = "El rango de fechas no puede exceder 365 días" });
            }

            if (limit < 1 || limit > 50)
            {
                return BadRequest(new { error = "El límite debe estar entre 1 y 50" });
            }

            _logger.LogInformation($"Obteniendo ranking de top {limit} conductores desde {startDate:yyyy-MM-dd} hasta {endDate:yyyy-MM-dd}");

            var rankings = await _comparisonService.GetDriverRankingsAsync(startDate, endDate, limit);

            return Ok(rankings);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener ranking de conductores: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor al obtener ranking" });
        }
    }

    /// <summary>
    /// Obtiene el ranking de conductores para el mes actual.
    /// </summary>
    /// <param name="limit">Número máximo de conductores en el ranking (predeterminado: 10)</param>
    /// <returns>Ranking de conductores del mes actual</returns>
    [HttpGet("rankings/current-month")]
    [SwaggerOperation(Summary = "Obtener ranking del mes actual")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<DriverRankingDTO>>> GetCurrentMonthRankings(
        [FromQuery] int limit = 10)
    {
        try
        {
            var now = DateTime.UtcNow;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            if (limit < 1 || limit > 50)
            {
                return BadRequest(new { error = "El límite debe estar entre 1 y 50" });
            }

            _logger.LogInformation($"Obteniendo ranking del mes actual (top {limit})");

            var rankings = await _comparisonService.GetDriverRankingsAsync(startDate, endDate, limit);

            return Ok(rankings);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener ranking del mes actual: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor al obtener ranking" });
        }
    }

    /// <summary>
    /// Obtiene el ranking de conductores para la semana actual.
    /// </summary>
    /// <param name="limit">Número máximo de conductores en el ranking (predeterminado: 10)</param>
    /// <returns>Ranking de conductores de la semana actual</returns>
    [HttpGet("rankings/current-week")]
    [SwaggerOperation(Summary = "Obtener ranking de la semana actual")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<DriverRankingDTO>>> GetCurrentWeekRankings(
        [FromQuery] int limit = 10)
    {
        try
        {
            var now = DateTime.UtcNow;
            var startDate = now.AddDays(-(int)now.DayOfWeek);
            var endDate = startDate.AddDays(7);

            if (limit < 1 || limit > 50)
            {
                return BadRequest(new { error = "El límite debe estar entre 1 y 50" });
            }

            _logger.LogInformation($"Obteniendo ranking de la semana actual (top {limit})");

            var rankings = await _comparisonService.GetDriverRankingsAsync(startDate, endDate, limit);

            return Ok(rankings);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener ranking de la semana actual: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor al obtener ranking" });
        }
    }
}
