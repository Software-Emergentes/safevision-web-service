using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafeVisionPlatform.Trip.Application.Internal.CommandServices;
using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Application.Internal.QueryServices;
using SafeVisionPlatform.Trip.Domain.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeVisionPlatform.Trip.Interfaces.REST.Controllers;

/// <summary>
/// Controlador REST para gestionar reportes de viajes.
/// </summary>
[ApiController]
[Route("api/trips/reports")]
[SwaggerTag("Trip Reports - Reportes de viajes")]
public class TripsReportsController : ControllerBase
{
    private readonly IReportQueryService _reportQueryService;
    private readonly ITripReportService _tripReportService;
    private readonly IReportExportService _reportExportService;
    private readonly ILogger<TripsReportsController> _logger;

    public TripsReportsController(
        IReportQueryService reportQueryService,
        ITripReportService tripReportService,
        IReportExportService reportExportService,
        ILogger<TripsReportsController> logger)
    {
        _reportQueryService = reportQueryService;
        _tripReportService = tripReportService;
        _reportExportService = reportExportService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los reportes generados.
    /// </summary>
    /// <remarks>
    /// Recupera los reportes de viaje generados (enviados a conductor o gerente) 
    /// y su estado actual.
    /// </remarks>
    [HttpGet]
    [SwaggerOperation(Summary = "Obtener todos los reportes")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<TripReportDTO>>> GetAllReports()
    {
        var reports = await _reportQueryService.GetAllReportsAsync();
        return Ok(reports);
    }

    /// <summary>
    /// Obtiene los reportes de un conductor específico.
    /// </summary>
    [HttpGet("driver/{driverId}")]
    [SwaggerOperation(Summary = "Obtener reportes de un conductor")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<TripReportDTO>>> GetReportsByDriver(int driverId)
    {
        var reports = await _reportQueryService.GetReportsByDriverIdAsync(driverId);
        return Ok(reports);
    }

    /// <summary>
    /// Obtiene el reporte asociado a un viaje específico.
    /// </summary>
    [HttpGet("trip/{tripId}")]
    [SwaggerOperation(Summary = "Obtener reporte de un viaje")]
    [Produces("application/json")]
    public async Task<ActionResult<TripReportDTO>> GetReportByTrip(int tripId)
    {
        var report = await _reportQueryService.GetReportByTripIdAsync(tripId);
        if (report == null)
            return NotFound(new { error = "Reporte no encontrado" });

        return Ok(report);
    }

    /// <summary>
    /// Genera un reporte para un viaje completado.
    /// </summary>
    [HttpPost("generate")]
    [SwaggerOperation(Summary = "Generar un reporte de viaje")]
    [Produces("application/json")]
    public async Task<ActionResult<TripReportDTO>> GenerateReport([FromBody] GenerateReportDTO generateReportDTO)
    {
        try
        {
            _logger.LogInformation($"Generando reporte para viaje {generateReportDTO.TripId}");
            var report = await _tripReportService.CreateReportAsync(generateReportDTO);
            return CreatedAtAction(nameof(GetReportByTrip), new { tripId = report.TripId }, report);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al generar reporte: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Envía un reporte al destinatario.
    /// </summary>
    [HttpPost("{reportId}/send")]
    [SwaggerOperation(Summary = "Enviar un reporte")]
    public async Task<IActionResult> SendReport(int reportId)
    {
        try
        {
            _logger.LogInformation($"Enviando reporte {reportId}");
            await _tripReportService.SendReportAsync(reportId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al enviar reporte: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Exporta un reporte individual a formato PDF.
    /// </summary>
    [HttpGet("{reportId}/export/pdf")]
    [SwaggerOperation(Summary = "Exportar reporte a PDF")]
    [Produces("application/pdf")]
    public async Task<IActionResult> ExportReportToPdf(int reportId)
    {
        try
        {
            _logger.LogInformation($"Exportando reporte {reportId} a PDF");
            var pdfBytes = await _reportExportService.ExportReportToPdfAsync(reportId);
            return File(pdfBytes, "application/pdf", $"reporte_{reportId}.pdf");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"Reporte no encontrado: {ex.Message}");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al exportar reporte a PDF: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Exporta un reporte individual a formato Excel.
    /// </summary>
    [HttpGet("{reportId}/export/excel")]
    [SwaggerOperation(Summary = "Exportar reporte a Excel")]
    [Produces("text/csv")]
    public async Task<IActionResult> ExportReportToExcel(int reportId)
    {
        try
        {
            _logger.LogInformation($"Exportando reporte {reportId} a Excel");
            var excelBytes = await _reportExportService.ExportReportToExcelAsync(reportId);
            return File(excelBytes, "text/csv", $"reporte_{reportId}.csv");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"Reporte no encontrado: {ex.Message}");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al exportar reporte a Excel: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Exporta múltiples reportes a formato PDF.
    /// </summary>
    [HttpPost("export/pdf")]
    [SwaggerOperation(Summary = "Exportar múltiples reportes a PDF")]
    [Produces("application/pdf")]
    public async Task<IActionResult> ExportMultipleReportsToPdf([FromBody] IEnumerable<int> reportIds)
    {
        try
        {
            _logger.LogInformation($"Exportando {reportIds.Count()} reportes a PDF");
            var pdfBytes = await _reportExportService.ExportMultipleReportsToPdfAsync(reportIds);
            return File(pdfBytes, "application/pdf", $"reportes_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al exportar reportes a PDF: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Exporta múltiples reportes a formato Excel.
    /// </summary>
    [HttpPost("export/excel")]
    [SwaggerOperation(Summary = "Exportar múltiples reportes a Excel")]
    [Produces("text/csv")]
    public async Task<IActionResult> ExportMultipleReportsToExcel([FromBody] IEnumerable<int> reportIds)
    {
        try
        {
            _logger.LogInformation($"Exportando {reportIds.Count()} reportes a Excel");
            var excelBytes = await _reportExportService.ExportMultipleReportsToExcelAsync(reportIds);
            return File(excelBytes, "text/csv", $"reportes_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al exportar reportes a Excel: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Exporta reportes de un conductor en un rango de fechas a PDF.
    /// </summary>
    [HttpGet("driver/{driverId}/export/pdf")]
    [SwaggerOperation(Summary = "Exportar reportes de conductor a PDF")]
    [Produces("application/pdf")]
    public async Task<IActionResult> ExportDriverReportsToPdf(
        int driverId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            _logger.LogInformation($"Exportando reportes del conductor {driverId} del {startDate:yyyy-MM-dd} al {endDate:yyyy-MM-dd} a PDF");
            var pdfBytes = await _reportExportService.ExportDriverReportsByDateRangeToPdfAsync(driverId, startDate, endDate);
            return File(pdfBytes, "application/pdf", $"reportes_conductor_{driverId}_{DateTime.UtcNow:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al exportar reportes del conductor a PDF: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Exporta reportes de un conductor en un rango de fechas a Excel.
    /// </summary>
    [HttpGet("driver/{driverId}/export/excel")]
    [SwaggerOperation(Summary = "Exportar reportes de conductor a Excel")]
    [Produces("text/csv")]
    public async Task<IActionResult> ExportDriverReportsToExcel(
        int driverId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            _logger.LogInformation($"Exportando reportes del conductor {driverId} del {startDate:yyyy-MM-dd} al {endDate:yyyy-MM-dd} a Excel");
            var excelBytes = await _reportExportService.ExportDriverReportsByDateRangeToExcelAsync(driverId, startDate, endDate);
            return File(excelBytes, "text/csv", $"reportes_conductor_{driverId}_{DateTime.UtcNow:yyyyMMdd}.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al exportar reportes del conductor a Excel: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Exporta estadísticas de la flota a PDF.
    /// </summary>
    [HttpGet("fleet/statistics/export/pdf")]
    [SwaggerOperation(Summary = "Exportar estadísticas de flota a PDF")]
    [Produces("application/pdf")]
    public async Task<IActionResult> ExportFleetStatisticsToPdf(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            _logger.LogInformation($"Exportando estadísticas de flota del {startDate:yyyy-MM-dd} al {endDate:yyyy-MM-dd} a PDF");
            var pdfBytes = await _reportExportService.ExportFleetStatisticsToPdfAsync(startDate, endDate);
            return File(pdfBytes, "application/pdf", $"estadisticas_flota_{DateTime.UtcNow:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al exportar estadísticas de flota a PDF: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Exporta estadísticas de la flota a Excel.
    /// </summary>
    [HttpGet("fleet/statistics/export/excel")]
    [SwaggerOperation(Summary = "Exportar estadísticas de flota a Excel")]
    [Produces("text/csv")]
    public async Task<IActionResult> ExportFleetStatisticsToExcel(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            _logger.LogInformation($"Exportando estadísticas de flota del {startDate:yyyy-MM-dd} al {endDate:yyyy-MM-dd} a Excel");
            var excelBytes = await _reportExportService.ExportFleetStatisticsToExcelAsync(startDate, endDate);
            return File(excelBytes, "text/csv", $"estadisticas_flota_{DateTime.UtcNow:yyyyMMdd}.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al exportar estadísticas de flota a Excel: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
}

/// <summary>
/// Controlador REST para gestionar alertas de viajes.
/// </summary>
[ApiController]
[Route("api/trips/alerts")]
[SwaggerTag("Trip Alerts - Alertas de viajes")]
public class TripsAlertsController : ControllerBase
{
    private readonly IAlertQueryService _alertQueryService;
    private readonly ITripApplicationService _tripApplicationService;
    private readonly ILogger<TripsAlertsController> _logger;

    public TripsAlertsController(
        IAlertQueryService alertQueryService,
        ITripApplicationService tripApplicationService,
        ILogger<TripsAlertsController> logger)
    {
        _alertQueryService = alertQueryService;
        _tripApplicationService = tripApplicationService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene las alertas de un viaje específico.
    /// </summary>
    /// <remarks>
    /// Devuelve todas las alertas (somnolencia, distracción, micro-sueño, etc.)
    /// detectadas durante el viaje.
    /// </remarks>
    [HttpGet("trip/{tripId}")]
    [SwaggerOperation(Summary = "Obtener alertas de un viaje")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<AlertDTO>>> GetAlertsByTrip(int tripId)
    {
        var alerts = await _alertQueryService.GetAlertsByTripIdAsync(tripId);
        return Ok(alerts);
    }

    /// <summary>
    /// Obtiene alertas por tipo en un rango de fechas.
    /// </summary>
    /// <remarks>
    /// Permite filtrar alertas por tipo (somnolencia, distracción, etc.)
    /// en un rango de fechas específico.
    /// </remarks>
    [HttpGet("type/{alertType}")]
    [SwaggerOperation(Summary = "Obtener alertas por tipo y rango de fechas")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<AlertDTO>>> GetAlertsByType(int alertType, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var alerts = await _alertQueryService.GetAlertsByTypeAndDateRangeAsync(alertType, startDate, endDate);
        return Ok(alerts);
    }

    /// <summary>
    /// Agrega una nueva alerta a un viaje.
    /// </summary>
    /// <remarks>
    /// Registra una alerta detectada durante el viaje (ej: somnolencia, distracción).
    /// </remarks>
    [HttpPost]
    [SwaggerOperation(Summary = "Agregar una alerta a un viaje")]
    [Produces("application/json")]
    public async Task<IActionResult> AddAlert([FromBody] CreateAlertDTO createAlertDTO)
    {
        try
        {
            _logger.LogInformation($"Agregando alerta al viaje {createAlertDTO.TripId}");
            await _tripApplicationService.AddAlertAsync(createAlertDTO);
            return StatusCode(201, new { message = "Alerta agregada exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al agregar alerta: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
}
