using Microsoft.AspNetCore.Mvc;
using SafeVisionPlatform.Management.Application.Internal.CommandServices;
using SafeVisionPlatform.Management.Application.Internal.DTO;

namespace SafeVisionPlatform.Management.Interfaces.REST.Controllers;

/// <summary>
/// Controlador para gestión de eventos críticos.
/// </summary>
[ApiController]
[Route("api/critical-events")]
[Tags("Management - Critical Events")]
public class CriticalEventsController : ControllerBase
{
    private readonly CriticalEventService _eventService;

    public CriticalEventsController(CriticalEventService eventService)
    {
        _eventService = eventService;
    }

    /// <summary>
    /// Obtiene eventos críticos pendientes.
    /// </summary>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(IEnumerable<CriticalEventDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingEvents()
    {
        var events = await _eventService.GetPendingEventsAsync();
        return Ok(events);
    }

    /// <summary>
    /// Obtiene eventos asignados a un gerente.
    /// </summary>
    [HttpGet("manager/{managerId}")]
    [ProducesResponseType(typeof(IEnumerable<CriticalEventDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEventsByManager(int managerId)
    {
        var events = await _eventService.GetEventsByManagerAsync(managerId);
        return Ok(events);
    }

    /// <summary>
    /// Obtiene un evento específico por ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CriticalEventDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventById(int id)
    {
        var ev = await _eventService.GetEventByIdAsync(id);
        if (ev == null)
            return NotFound(new { message = $"Evento con ID {id} no encontrado" });

        return Ok(ev);
    }

    /// <summary>
    /// Registra y gestiona un evento crítico.
    /// </summary>
    [HttpPost("handle")]
    [ProducesResponseType(typeof(CriticalEventDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleCriticalEvent([FromBody] CreateCriticalEventDTO request)
    {
        try
        {
            var ev = await _eventService.HandleCriticalEventAsync(request);
            return CreatedAtAction(nameof(GetEventById), new { id = ev.Id }, ev);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Asigna un gerente para gestionar el evento.
    /// </summary>
    [HttpPost("{id}/assign-manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignManager(int id, [FromQuery] int managerId)
    {
        try
        {
            await _eventService.AssignManagerAsync(id, managerId);
            return Ok(new { message = "Gerente asignado exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Agrega una acción tomada al evento.
    /// </summary>
    [HttpPost("{id}/add-action")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddAction(int id, [FromBody] string action)
    {
        try
        {
            await _eventService.AddActionAsync(id, action);
            return Ok(new { message = "Acción registrada exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Despacha respuesta de emergencia.
    /// </summary>
    [HttpPost("{id}/dispatch-emergency")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DispatchEmergency(int id)
    {
        try
        {
            await _eventService.DispatchEmergencyResponseAsync(id);
            return Ok(new { message = "Respuesta de emergencia despachada" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Notifica a la aseguradora.
    /// </summary>
    [HttpPost("{id}/notify-insurance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> NotifyInsurance(int id, [FromBody] string insuranceReference)
    {
        try
        {
            await _eventService.NotifyInsuranceAsync(id, insuranceReference);
            return Ok(new { message = "Aseguradora notificada" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Resuelve el evento.
    /// </summary>
    [HttpPost("{id}/resolve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResolveEvent(int id, [FromBody] string notes)
    {
        try
        {
            await _eventService.ResolveEventAsync(id, notes);
            return Ok(new { message = "Evento resuelto exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
