using Microsoft.AspNetCore.Mvc;
using SafeVisionPlatform.Management.Application.Internal.CommandServices;
using SafeVisionPlatform.Management.Application.Internal.DTO;

namespace SafeVisionPlatform.Management.Interfaces.REST.Controllers;

/// <summary>
/// Controlador para gestión de reportes.
/// </summary>
[ApiController]
[Route("api/reports")]
[Tags("Management - Reports")]
public class ReportsController : ControllerBase
{
    private readonly ReportManagementService _reportService;

    public ReportsController(ReportManagementService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Obtiene todos los reportes generados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReportDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReports([FromQuery] int? managerId = null)
    {
        if (managerId.HasValue)
        {
            var reports = await _reportService.GetReportsByManagerAsync(managerId.Value);
            return Ok(reports);
        }

        return Ok(new List<ReportDTO>());
    }

    /// <summary>
    /// Obtiene un reporte específico por ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReportDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReportById(int id)
    {
        var report = await _reportService.GetReportByIdAsync(id);
        if (report == null)
            return NotFound(new { message = $"Reporte con ID {id} no encontrado" });

        return Ok(report);
    }

    /// <summary>
    /// Obtiene reportes de un conductor específico.
    /// </summary>
    [HttpGet("driver/{driverId}")]
    [ProducesResponseType(typeof(IEnumerable<ReportDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReportsByDriver(int driverId)
    {
        var reports = await _reportService.GetReportsByDriverAsync(driverId);
        return Ok(reports);
    }

    /// <summary>
    /// Genera un nuevo reporte.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ReportDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateReport([FromBody] GenerateManagementReportDTO request)
    {
        try
        {
            var report = await _reportService.GenerateReportAsync(request);
            return CreatedAtAction(nameof(GetReportById), new { id = report.Id }, report);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Exporta un reporte a PDF o Excel.
    /// </summary>
    [HttpPost("export")]
    [ProducesResponseType(typeof(ReportDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportReport([FromBody] ExportReportDTO request)
    {
        try
        {
            var report = await _reportService.ExportReportAsync(request);
            return Ok(report);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
