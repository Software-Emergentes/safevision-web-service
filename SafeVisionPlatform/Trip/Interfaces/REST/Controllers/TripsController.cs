using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafeVisionPlatform.Trip.Application.Internal.CommandServices;
using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Application.Internal.QueryServices;
using SafeVisionPlatform.Trip.Domain.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeVisionPlatform.Trip.Interfaces.REST.Controllers;

/// <summary>
/// Controlador REST para gestionar viajes.
/// Expone endpoints para iniciar, finalizar, cancelar y consultar viajes.
/// </summary>
[ApiController]
[Route("api/trips")]
[SwaggerTag("Trips - Gestión de viajes")]
public class TripsController : ControllerBase
{
    private readonly ITripApplicationService _tripApplicationService;
    private readonly ITripQueryService _tripQueryService;
    private readonly ITripRecommendationService _recommendationService;
    private readonly ILogger<TripsController> _logger;

    public TripsController(
        ITripApplicationService tripApplicationService,
        ITripQueryService tripQueryService,
        ITripRecommendationService recommendationService,
        ILogger<TripsController> logger)
    {
        _tripApplicationService = tripApplicationService;
        _tripQueryService = tripQueryService;
        _recommendationService = recommendationService;
        _logger = logger;
    }

    /// <summary>
    /// Inicia un nuevo viaje.
    /// </summary>
    /// <remarks>
    /// Valida que el conductor y vehículo estén disponibles.
    /// Registra la hora de inicio y el estado "En progreso".
    /// </remarks>
    [HttpPost("start")]
    [SwaggerOperation(Summary = "Iniciar un nuevo viaje")]
    [Produces("application/json")]
    public async Task<ActionResult<TripDTO>> StartTrip([FromBody] CreateTripDTO createTripDTO)
    {
        try
        {
            _logger.LogInformation($"Iniciando viaje para conductor {createTripDTO.DriverId}");
            var trip = await _tripApplicationService.StartTripAsync(createTripDTO);
            return CreatedAtAction(nameof(GetTripById), new { id = trip.Id }, trip);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"Error al iniciar viaje: {ex.Message}");
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error no esperado: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Finaliza un viaje activo.
    /// </summary>
    /// <remarks>
    /// Registra la hora de finalización y dispara la generación del reporte.
    /// </remarks>
    [HttpPut("{id}/end")]
    [SwaggerOperation(Summary = "Finalizar un viaje")]
    [Produces("application/json")]
    public async Task<ActionResult<TripDTO>> EndTrip(int id)
    {
        try
        {
            _logger.LogInformation($"Finalizando viaje {id}");
            var trip = await _tripApplicationService.EndTripAsync(id);
            return Ok(trip);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"Error al finalizar viaje: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error no esperado: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Cancela un viaje antes de finalizarlo.
    /// </summary>
    /// <remarks>
    /// Actualiza el estado y registra el motivo de cancelación.
    /// </remarks>
    [HttpPut("{id}/cancel")]
    [SwaggerOperation(Summary = "Cancelar un viaje")]
    [Produces("application/json")]
    public async Task<ActionResult<TripDTO>> CancelTrip(int id, [FromQuery] string? reason = null)
    {
        try
        {
            _logger.LogInformation($"Cancelando viaje {id}");
            var trip = await _tripApplicationService.CancelTripAsync(id, reason);
            return Ok(trip);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"Error al cancelar viaje: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error no esperado: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene los detalles de un viaje específico.
    /// </summary>
    /// <remarks>
    /// Devuelve información del viaje: conductor, vehículo, alertas y reporte.
    /// </remarks>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obtener detalles de un viaje")]
    [Produces("application/json")]
    public async Task<ActionResult<TripDTO>> GetTripById(int id)
    {
        var trip = await _tripQueryService.GetTripByIdAsync(id);
        if (trip == null)
            return NotFound(new { error = "Viaje no encontrado" });

        return Ok(trip);
    }

    /// <summary>
    /// Obtiene el historial de viajes de un conductor.
    /// </summary>
    /// <remarks>
    /// Soporta filtros por fecha y estado.
    /// </remarks>
    [HttpGet("driver/{driverId}")]
    [SwaggerOperation(Summary = "Obtener historial de viajes de un conductor")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<TripDTO>>> GetTripsByDriver(int driverId)
    {
        var trips = await _tripQueryService.GetTripsByDriverIdAsync(driverId);
        return Ok(trips);
    }

    /// <summary>
    /// Obtiene todos los viajes de un vehículo.
    /// </summary>
    /// <remarks>
    /// Útil para control de mantenimiento o análisis de uso.
    /// </remarks>
    [HttpGet("vehicle/{vehicleId}")]
    [SwaggerOperation(Summary = "Obtener viajes de un vehículo")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<TripDTO>>> GetTripsByVehicle(int vehicleId)
    {
        var trips = await _tripQueryService.GetTripsByVehicleIdAsync(vehicleId);
        return Ok(trips);
    }

    /// <summary>
    /// Sincroniza los datos del viaje con la nube.
    /// </summary>
    /// <remarks>
    /// Sincroniza alertas, duración y métricas según la política TripDataPolicy.
    /// </remarks>
    [HttpPost("{id}/sync")]
    [SwaggerOperation(Summary = "Sincronizar datos del viaje con la nube")]
    public async Task<IActionResult> SyncTripData(int id)
    {
        try
        {
            _logger.LogInformation($"Sincronizando datos del viaje {id}");
            await _tripApplicationService.SyncTripDataAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"Error al sincronizar viaje: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error no esperado: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene recomendaciones personalizadas después de un viaje.
    /// </summary>
    /// <remarks>
    /// Analiza el viaje y genera recomendaciones basadas en:
    /// - Cantidad y tipo de alertas
    /// - Duración del viaje
    /// - Horario de conducción
    /// - Patrones de fatiga
    /// </remarks>
    [HttpGet("{id}/recommendations")]
    [SwaggerOperation(Summary = "Obtener recomendaciones personalizadas del viaje")]
    [Produces("application/json")]
    public async Task<ActionResult<TripRecommendationsDTO>> GetTripRecommendations(int id)
    {
        try
        {
            _logger.LogInformation($"Generando recomendaciones para viaje {id}");
            var recommendations = await _recommendationService.GenerateRecommendationsAsync(id);
            return Ok(recommendations);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"Viaje no encontrado: {ex.Message}");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al generar recomendaciones: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
}
