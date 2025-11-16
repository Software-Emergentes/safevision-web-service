using Microsoft.AspNetCore.Mvc;
using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Domain.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeVisionPlatform.Trip.Interfaces.REST.Controllers;

/// <summary>
/// Controlador REST para feedback de alertas.
/// Permite a los conductores reportar falsas alarmas y enviar comentarios.
/// </summary>
[ApiController]
[Route("api/alerts/feedback")]
[SwaggerTag("Alert Feedback - Feedback y falsas alarmas")]
public class AlertFeedbackController : ControllerBase
{
    private readonly IAlertFeedbackService _feedbackService;
    private readonly ILogger<AlertFeedbackController> _logger;

    public AlertFeedbackController(
        IAlertFeedbackService feedbackService,
        ILogger<AlertFeedbackController> logger)
    {
        _feedbackService = feedbackService;
        _logger = logger;
    }

    /// <summary>
    /// Envía feedback sobre una alerta.
    /// </summary>
    /// <remarks>
    /// Permite al conductor:
    /// - Marcar una alerta como falsa alarma
    /// - Agregar comentarios sobre la alerta
    /// Esto ayuda a mejorar la precisión del sistema.
    /// </remarks>
    [HttpPost]
    [SwaggerOperation(Summary = "Enviar feedback sobre una alerta")]
    [Produces("application/json")]
    public async Task<ActionResult<FeedbackResponseDTO>> SubmitFeedback([FromBody] AlertFeedbackDTO feedbackDto)
    {
        try
        {
            _logger.LogInformation($"Recibiendo feedback para alerta {feedbackDto.AlertId} de conductor {feedbackDto.DriverId}");
            var response = await _feedbackService.SubmitFeedbackAsync(feedbackDto);

            if (response.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al procesar feedback: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene estadísticas de feedback del sistema.
    /// </summary>
    /// <remarks>
    /// Retorna métricas agregadas sobre:
    /// - Total de alertas evaluadas
    /// - Porcentaje de falsas alarmas
    /// - Distribución por tipo de alerta
    /// </remarks>
    [HttpGet("statistics")]
    [SwaggerOperation(Summary = "Obtener estadísticas de feedback del sistema")]
    [Produces("application/json")]
    public async Task<ActionResult<FeedbackStatisticsDTO>> GetFeedbackStatistics()
    {
        try
        {
            _logger.LogInformation("Obteniendo estadísticas de feedback del sistema");
            var statistics = await _feedbackService.GetFeedbackStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener estadísticas de feedback: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene estadísticas de feedback de un conductor específico.
    /// </summary>
    [HttpGet("statistics/driver/{driverId}")]
    [SwaggerOperation(Summary = "Obtener estadísticas de feedback de un conductor")]
    [Produces("application/json")]
    public async Task<ActionResult<FeedbackStatisticsDTO>> GetDriverFeedbackStatistics(int driverId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo estadísticas de feedback del conductor {driverId}");
            var statistics = await _feedbackService.GetDriverFeedbackStatisticsAsync(driverId);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener estadísticas de feedback del conductor: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene feedback reciente del sistema.
    /// </summary>
    /// <param name="limit">Cantidad de registros a retornar (default: 50)</param>
    [HttpGet("recent")]
    [SwaggerOperation(Summary = "Obtener feedback reciente")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<AlertWithFeedbackDTO>>> GetRecentFeedback([FromQuery] int limit = 50)
    {
        try
        {
            _logger.LogInformation($"Obteniendo {limit} feedback recientes");
            var feedback = await _feedbackService.GetRecentFeedbackAsync(limit);
            return Ok(feedback);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener feedback reciente: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene todas las alertas marcadas como falsas alarmas.
    /// </summary>
    /// <remarks>
    /// Útil para análisis de precisión del sistema y mejoras futuras.
    /// </remarks>
    [HttpGet("false-alarms")]
    [SwaggerOperation(Summary = "Obtener alertas marcadas como falsas alarmas")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<AlertWithFeedbackDTO>>> GetFalseAlarms()
    {
        try
        {
            _logger.LogInformation("Obteniendo falsas alarmas reportadas");
            var falseAlarms = await _feedbackService.GetFalseAlarmsAsync();
            return Ok(falseAlarms);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener falsas alarmas: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
}
