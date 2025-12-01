using Microsoft.AspNetCore.Mvc;
using SafeVisionPlatform.FatigueMonitoring.Application.Internal.DTO;
using SafeVisionPlatform.FatigueMonitoring.Application.Internal.QueryServices;

namespace SafeVisionPlatform.FatigueMonitoring.Interfaces.REST.Controllers;

/// <summary>
/// Controlador para consultar el estado de fatiga de los conductores.
/// </summary>
[ApiController]
[Route("api/fatigue")]
[Tags("Fatigue Monitoring")]
public class FatigueStatusController : ControllerBase
{
    private readonly FatigueQueryService _fatigueQueryService;

    public FatigueStatusController(FatigueQueryService fatigueQueryService)
    {
        _fatigueQueryService = fatigueQueryService;
    }

    /// <summary>
    /// Consulta el estado actual de fatiga de un conductor.
    /// </summary>
    [HttpGet("status/{driverId}")]
    [ProducesResponseType(typeof(FatigueStatusDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFatigueStatus(int driverId)
    {
        try
        {
            var status = await _fatigueQueryService.GetFatigueStatusAsync(driverId);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene eventos de somnolencia de un viaje específico.
    /// </summary>
    [HttpGet("events/trip/{tripId}")]
    [ProducesResponseType(typeof(IEnumerable<DrowsinessEventDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDrowsinessEventsByTrip(int tripId)
    {
        var events = await _fatigueQueryService.GetDrowsinessEventsByTripAsync(tripId);
        return Ok(events);
    }
}
