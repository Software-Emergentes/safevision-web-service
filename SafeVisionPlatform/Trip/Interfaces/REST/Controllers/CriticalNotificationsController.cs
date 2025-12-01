using Microsoft.AspNetCore.Mvc;
using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Domain.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeVisionPlatform.Trip.Interfaces.REST.Controllers;

/// <summary>
/// Controlador REST para notificaciones críticas a gerentes.
/// </summary>
[ApiController]
[Route("api/notifications/critical")]
[SwaggerTag("Critical Notifications - Notificaciones críticas para gerentes")]
public class CriticalNotificationsController : ControllerBase
{
    private readonly ICriticalNotificationService _notificationService;
    private readonly ILogger<CriticalNotificationsController> _logger;

    public CriticalNotificationsController(
        ICriticalNotificationService notificationService,
        ILogger<CriticalNotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Crea y envía una notificación crítica al gerente.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Crear y enviar notificación crítica")]
    [Produces("application/json")]
    public async Task<ActionResult<NotificationResponseDTO>> CreateCriticalNotification(
        [FromBody] CreateCriticalNotificationDTO notificationDto)
    {
        try
        {
            _logger.LogInformation($"Creando notificación crítica para conductor {notificationDto.DriverId}");
            var response = await _notificationService.CreateAndSendCriticalNotificationAsync(notificationDto);

            if (response.Success)
                return Ok(response);
            else
                return StatusCode(500, response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear notificación crítica: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene todas las notificaciones de un gerente.
    /// </summary>
    [HttpGet("manager/{managerId}")]
    [SwaggerOperation(Summary = "Obtener notificaciones de un gerente")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<CriticalNotificationDTO>>> GetManagerNotifications(int managerId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo notificaciones del gerente {managerId}");
            var notifications = await _notificationService.GetManagerNotificationsAsync(managerId);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener notificaciones del gerente: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene las notificaciones pendientes de un gerente.
    /// </summary>
    [HttpGet("manager/{managerId}/pending")]
    [SwaggerOperation(Summary = "Obtener notificaciones pendientes de un gerente")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<CriticalNotificationDTO>>> GetPendingManagerNotifications(int managerId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo notificaciones pendientes del gerente {managerId}");
            var notifications = await _notificationService.GetPendingManagerNotificationsAsync(managerId);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener notificaciones pendientes: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Marca una notificación como leída.
    /// </summary>
    [HttpPut("{notificationId}/read")]
    [SwaggerOperation(Summary = "Marcar notificación como leída")]
    public async Task<IActionResult> MarkAsRead(int notificationId)
    {
        try
        {
            _logger.LogInformation($"Marcando notificación {notificationId} como leída");
            await _notificationService.MarkAsReadAsync(notificationId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"Notificación no encontrada: {ex.Message}");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al marcar notificación como leída: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Marca una notificación como reconocida.
    /// </summary>
    [HttpPut("{notificationId}/acknowledge")]
    [SwaggerOperation(Summary = "Marcar notificación como reconocida")]
    public async Task<IActionResult> MarkAsAcknowledged(int notificationId)
    {
        try
        {
            _logger.LogInformation($"Marcando notificación {notificationId} como reconocida");
            await _notificationService.MarkAsAcknowledgedAsync(notificationId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"Notificación no encontrada: {ex.Message}");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al marcar notificación como reconocida: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Evalúa el estado de un conductor y envía notificación si está en estado crítico.
    /// </summary>
    [HttpPost("evaluate")]
    [SwaggerOperation(Summary = "Evaluar conductor y notificar si está en riesgo crítico")]
    public async Task<IActionResult> EvaluateAndNotify([FromQuery] int driverId, [FromQuery] int tripId)
    {
        try
        {
            _logger.LogInformation($"Evaluando estado del conductor {driverId} en viaje {tripId}");
            await _notificationService.EvaluateAndNotifyIfCriticalAsync(driverId, tripId);
            return Ok(new { message = "Evaluación completada" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al evaluar conductor: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Envía notificación de viaje completado de manera segura (sin alertas críticas).
    /// </summary>
    /// <remarks>
    /// Este endpoint debería ser llamado cuando un viaje se completa exitosamente.
    /// Solo envía la notificación si el viaje no tuvo alertas críticas.
    /// </remarks>
    [HttpPost("safe-trip-completed")]
    [SwaggerOperation(Summary = "Notificar viaje seguro completado")]
    public async Task<IActionResult> NotifySafeTripCompleted(
        [FromQuery] int driverId,
        [FromQuery] int tripId,
        [FromQuery] int? managerId = null)
    {
        try
        {
            _logger.LogInformation($"Enviando notificación de viaje seguro para viaje {tripId}");
            await _notificationService.SendSafeTripCompletedNotificationAsync(driverId, tripId, managerId);
            return Ok(new { message = "Notificación de viaje seguro enviada" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al enviar notificación de viaje seguro: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
}
