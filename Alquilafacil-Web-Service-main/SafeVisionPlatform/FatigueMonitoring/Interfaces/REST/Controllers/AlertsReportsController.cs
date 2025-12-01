using Microsoft.AspNetCore.Mvc;
using SafeVisionPlatform.FatigueMonitoring.Application.Internal.CommandServices;
using SafeVisionPlatform.FatigueMonitoring.Application.Internal.DTO;
using SafeVisionPlatform.FatigueMonitoring.Application.Internal.QueryServices;

namespace SafeVisionPlatform.FatigueMonitoring.Interfaces.REST.Controllers;

/// <summary>
/// Controlador para reportes y métricas de alertas críticas.
/// </summary>
[ApiController]
[Route("api/alerts")]
[Tags("Fatigue Monitoring - Alerts")]
public class AlertsReportsController : ControllerBase
{
    private readonly FatigueQueryService _fatigueQueryService;
    private readonly FatigueDetectionCommandService _commandService;

    public AlertsReportsController(
        FatigueQueryService fatigueQueryService,
        FatigueDetectionCommandService commandService)
    {
        _fatigueQueryService = fatigueQueryService;
        _commandService = commandService;
    }

    /// <summary>
    /// Obtiene reportes de alertas críticas generadas.
    /// </summary>
    [HttpGet("reports")]
    [ProducesResponseType(typeof(IEnumerable<CriticalAlertReportDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAlertReports(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var reports = await _fatigueQueryService.GetAlertReportsAsync(startDate, endDate);
        return Ok(reports);
    }

    /// <summary>
    /// Obtiene reportes de alertas por conductor.
    /// </summary>
    [HttpGet("reports/driver/{driverId}")]
    [ProducesResponseType(typeof(IEnumerable<CriticalAlertReportDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAlertReportsByDriver(int driverId)
    {
        var reports = await _fatigueQueryService.GetAlertReportsByDriverAsync(driverId);
        return Ok(reports);
    }

    /// <summary>
    /// Obtiene alertas pendientes para un gerente.
    /// </summary>
    [HttpGet("pending/manager/{managerId}")]
    [ProducesResponseType(typeof(IEnumerable<CriticalAlertReportDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingAlertsForManager(int managerId)
    {
        var alerts = await _fatigueQueryService.GetPendingAlertsForManagerAsync(managerId);
        return Ok(alerts);
    }

    /// <summary>
    /// Obtiene métricas agregadas de alertas.
    /// </summary>
    [HttpGet("metrics")]
    [ProducesResponseType(typeof(AlertMetricsDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAlertMetrics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;
        var metrics = await _fatigueQueryService.GetAlertMetricsAsync(start, end);
        return Ok(metrics);
    }

    /// <summary>
    /// Procesa datos del sensor para detección de fatiga.
    /// </summary>
    [HttpPost("sensor-data")]
    [ProducesResponseType(typeof(DrowsinessEventDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ProcessSensorData([FromBody] SensorDataInputDTO input)
    {
        var result = await _commandService.ProcessSensorDataAsync(input);
        if (result == null)
            return NoContent();
        return Ok(result);
    }

    /// <summary>
    /// Reconoce una alerta crítica.
    /// </summary>
    [HttpPost("{alertId}/acknowledge")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcknowledgeAlert(
        int alertId,
        [FromQuery] int userId,
        [FromBody] string? actionTaken = null)
    {
        try
        {
            await _commandService.AcknowledgeAlertAsync(alertId, userId, actionTaken);
            return Ok(new { message = "Alerta reconocida exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Resuelve una alerta crítica.
    /// </summary>
    [HttpPost("{alertId}/resolve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResolveAlert(int alertId, [FromBody] string actionTaken)
    {
        try
        {
            await _commandService.ResolveAlertAsync(alertId, actionTaken);
            return Ok(new { message = "Alerta resuelta exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
