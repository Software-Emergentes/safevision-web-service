using Microsoft.AspNetCore.Mvc;
using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Domain.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeVisionPlatform.Trip.Interfaces.REST.Controllers;

/// <summary>
/// Controlador REST para gestión de parámetros de seguridad.
/// Permite a los gerentes ajustar umbrales de detección de fatiga según políticas internas.
/// </summary>
[ApiController]
[Route("api/security/configurations")]
[SwaggerTag("Security Configuration - Configuración de parámetros de seguridad")]
public class SecurityConfigurationController : ControllerBase
{
    private readonly ISecurityConfigurationService _configService;
    private readonly ILogger<SecurityConfigurationController> _logger;

    public SecurityConfigurationController(
        ISecurityConfigurationService configService,
        ILogger<SecurityConfigurationController> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    /// <summary>
    /// Crea una nueva configuración de seguridad personalizada.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Crear nueva configuración de seguridad")]
    [Produces("application/json")]
    public async Task<ActionResult<ConfigurationOperationResponseDTO>> CreateConfiguration(
        [FromBody] CreateSecurityConfigurationDTO configDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(configDto.Name))
            {
                return BadRequest(new { error = "El nombre de la configuración es requerido" });
            }

            _logger.LogInformation($"Creando nueva configuración de seguridad: {configDto.Name}");

            var response = await _configService.CreateConfigurationAsync(configDto);

            if (response.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear configuración: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene una configuración específica por su ID.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obtener configuración por ID")]
    [Produces("application/json")]
    public async Task<ActionResult<SecurityConfigurationDTO>> GetConfigurationById(int id)
    {
        try
        {
            _logger.LogInformation($"Obteniendo configuración {id}");

            var config = await _configService.GetConfigurationByIdAsync(id);

            if (config == null)
                return NotFound(new { error = "Configuración no encontrada" });

            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener configuración: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene la configuración activa para un gerente específico.
    /// </summary>
    [HttpGet("manager/{managerId}/active")]
    [SwaggerOperation(Summary = "Obtener configuración activa de un gerente")]
    [Produces("application/json")]
    public async Task<ActionResult<SecurityConfigurationDTO>> GetActiveConfigurationForManager(int managerId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo configuración activa del gerente {managerId}");

            var config = await _configService.GetActiveConfigurationForManagerAsync(managerId);

            if (config == null)
                return NotFound(new { error = "No se encontró configuración activa" });

            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener configuración activa: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene la configuración activa para una flota específica.
    /// </summary>
    [HttpGet("fleet/{fleetId}/active")]
    [SwaggerOperation(Summary = "Obtener configuración activa de una flota")]
    [Produces("application/json")]
    public async Task<ActionResult<SecurityConfigurationDTO>> GetActiveConfigurationForFleet(
        int fleetId,
        [FromQuery] int? managerId = null)
    {
        try
        {
            _logger.LogInformation($"Obteniendo configuración activa de la flota {fleetId}");

            var config = await _configService.GetActiveConfigurationForFleetAsync(fleetId, managerId);

            if (config == null)
                return NotFound(new { error = "No se encontró configuración activa" });

            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener configuración activa de flota: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene todas las configuraciones de un gerente.
    /// </summary>
    [HttpGet("manager/{managerId}")]
    [SwaggerOperation(Summary = "Obtener todas las configuraciones de un gerente")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<SecurityConfigurationSummaryDTO>>> GetAllConfigurationsByManager(int managerId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo configuraciones del gerente {managerId}");

            var configs = await _configService.GetAllConfigurationsByManagerAsync(managerId);

            return Ok(configs);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener configuraciones del gerente: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene todas las configuraciones activas del sistema.
    /// </summary>
    [HttpGet("active")]
    [SwaggerOperation(Summary = "Obtener todas las configuraciones activas")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<SecurityConfigurationSummaryDTO>>> GetAllActiveConfigurations()
    {
        try
        {
            _logger.LogInformation("Obteniendo todas las configuraciones activas");

            var configs = await _configService.GetAllActiveConfigurationsAsync();

            return Ok(configs);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener configuraciones activas: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene la configuración predeterminada del sistema.
    /// </summary>
    [HttpGet("default")]
    [SwaggerOperation(Summary = "Obtener configuración predeterminada del sistema")]
    [Produces("application/json")]
    public async Task<ActionResult<SecurityConfigurationDTO>> GetDefaultConfiguration()
    {
        try
        {
            _logger.LogInformation("Obteniendo configuración predeterminada");

            var config = await _configService.GetDefaultConfigurationAsync();

            if (config == null)
                return NotFound(new { error = "No se encontró configuración predeterminada" });

            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener configuración predeterminada: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza los umbrales de detección de somnolencia.
    /// </summary>
    [HttpPut("drowsiness-thresholds")]
    [SwaggerOperation(Summary = "Actualizar umbrales de detección de somnolencia")]
    [Produces("application/json")]
    public async Task<ActionResult<ConfigurationOperationResponseDTO>> UpdateDrowsinessThresholds(
        [FromBody] UpdateDrowsinessThresholdsDTO dto)
    {
        try
        {
            _logger.LogInformation($"Actualizando umbrales de somnolencia en configuración {dto.ConfigurationId}");

            var response = await _configService.UpdateDrowsinessThresholdsAsync(dto);

            if (response.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar umbrales de somnolencia: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza los umbrales de detección de microsueño.
    /// </summary>
    [HttpPut("microsleep-thresholds")]
    [SwaggerOperation(Summary = "Actualizar umbrales de detección de microsueño")]
    [Produces("application/json")]
    public async Task<ActionResult<ConfigurationOperationResponseDTO>> UpdateMicroSleepThresholds(
        [FromBody] UpdateMicroSleepThresholdsDTO dto)
    {
        try
        {
            _logger.LogInformation($"Actualizando umbrales de microsueño en configuración {dto.ConfigurationId}");

            var response = await _configService.UpdateMicroSleepThresholdsAsync(dto);

            if (response.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar umbrales de microsueño: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza los umbrales de detección de distracción.
    /// </summary>
    [HttpPut("distraction-thresholds")]
    [SwaggerOperation(Summary = "Actualizar umbrales de detección de distracción")]
    [Produces("application/json")]
    public async Task<ActionResult<ConfigurationOperationResponseDTO>> UpdateDistractionThresholds(
        [FromBody] UpdateDistractionThresholdsDTO dto)
    {
        try
        {
            _logger.LogInformation($"Actualizando umbrales de distracción en configuración {dto.ConfigurationId}");

            var response = await _configService.UpdateDistractionThresholdsAsync(dto);

            if (response.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar umbrales de distracción: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza los umbrales de alertas críticas.
    /// </summary>
    [HttpPut("critical-alerts-thresholds")]
    [SwaggerOperation(Summary = "Actualizar umbrales de alertas críticas")]
    [Produces("application/json")]
    public async Task<ActionResult<ConfigurationOperationResponseDTO>> UpdateCriticalAlertsThresholds(
        [FromBody] UpdateCriticalAlertsThresholdsDTO dto)
    {
        try
        {
            _logger.LogInformation($"Actualizando umbrales de alertas críticas en configuración {dto.ConfigurationId}");

            var response = await _configService.UpdateCriticalAlertsThresholdsAsync(dto);

            if (response.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar umbrales de alertas críticas: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza la configuración de puntuación de seguridad.
    /// </summary>
    [HttpPut("safety-score-configuration")]
    [SwaggerOperation(Summary = "Actualizar configuración de puntuación de seguridad")]
    [Produces("application/json")]
    public async Task<ActionResult<ConfigurationOperationResponseDTO>> UpdateSafetyScoreConfiguration(
        [FromBody] UpdateSafetyScoreConfigurationDTO dto)
    {
        try
        {
            _logger.LogInformation($"Actualizando configuración de puntuación en configuración {dto.ConfigurationId}");

            var response = await _configService.UpdateSafetyScoreConfigurationAsync(dto);

            if (response.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar configuración de puntuación: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Activa una configuración de seguridad.
    /// </summary>
    [HttpPut("{id}/activate")]
    [SwaggerOperation(Summary = "Activar configuración")]
    public async Task<ActionResult<ConfigurationOperationResponseDTO>> ActivateConfiguration(
        int id,
        [FromQuery] int userId)
    {
        try
        {
            _logger.LogInformation($"Activando configuración {id}");

            var response = await _configService.ActivateConfigurationAsync(id, userId);

            if (response.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al activar configuración: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Desactiva una configuración de seguridad.
    /// </summary>
    [HttpPut("{id}/deactivate")]
    [SwaggerOperation(Summary = "Desactivar configuración")]
    public async Task<ActionResult<ConfigurationOperationResponseDTO>> DeactivateConfiguration(
        int id,
        [FromQuery] int userId)
    {
        try
        {
            _logger.LogInformation($"Desactivando configuración {id}");

            var response = await _configService.DeactivateConfigurationAsync(id, userId);

            if (response.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al desactivar configuración: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Elimina una configuración de seguridad (no se puede eliminar la predeterminada).
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Eliminar configuración")]
    public async Task<ActionResult<ConfigurationOperationResponseDTO>> DeleteConfiguration(int id)
    {
        try
        {
            _logger.LogInformation($"Eliminando configuración {id}");

            var response = await _configService.DeleteConfigurationAsync(id);

            if (response.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al eliminar configuración: {ex.Message}");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
}
