using SafeVisionPlatform.Trip.Application.Internal.DTO;

namespace SafeVisionPlatform.Trip.Domain.Services;

/// <summary>
/// Servicio de dominio para gestión de configuraciones de parámetros de seguridad.
/// Permite a los gerentes ajustar umbrales de detección según políticas internas.
/// </summary>
public interface ISecurityConfigurationService
{
    /// <summary>
    /// Crea una nueva configuración de seguridad.
    /// </summary>
    Task<ConfigurationOperationResponseDTO> CreateConfigurationAsync(CreateSecurityConfigurationDTO configDto);

    /// <summary>
    /// Obtiene una configuración por su ID.
    /// </summary>
    Task<SecurityConfigurationDTO?> GetConfigurationByIdAsync(int id);

    /// <summary>
    /// Obtiene la configuración activa para un gerente.
    /// Si no existe, retorna la configuración predeterminada del sistema.
    /// </summary>
    Task<SecurityConfigurationDTO?> GetActiveConfigurationForManagerAsync(int managerId);

    /// <summary>
    /// Obtiene la configuración activa para una flota.
    /// Si no existe, retorna la configuración del gerente o la predeterminada.
    /// </summary>
    Task<SecurityConfigurationDTO?> GetActiveConfigurationForFleetAsync(int fleetId, int? managerId = null);

    /// <summary>
    /// Obtiene todas las configuraciones de un gerente.
    /// </summary>
    Task<IEnumerable<SecurityConfigurationSummaryDTO>> GetAllConfigurationsByManagerAsync(int managerId);

    /// <summary>
    /// Obtiene todas las configuraciones activas del sistema.
    /// </summary>
    Task<IEnumerable<SecurityConfigurationSummaryDTO>> GetAllActiveConfigurationsAsync();

    /// <summary>
    /// Obtiene la configuración predeterminada del sistema.
    /// </summary>
    Task<SecurityConfigurationDTO?> GetDefaultConfigurationAsync();

    /// <summary>
    /// Actualiza los umbrales de detección de somnolencia.
    /// </summary>
    Task<ConfigurationOperationResponseDTO> UpdateDrowsinessThresholdsAsync(UpdateDrowsinessThresholdsDTO dto);

    /// <summary>
    /// Actualiza los umbrales de detección de microsueño.
    /// </summary>
    Task<ConfigurationOperationResponseDTO> UpdateMicroSleepThresholdsAsync(UpdateMicroSleepThresholdsDTO dto);

    /// <summary>
    /// Actualiza los umbrales de detección de distracción.
    /// </summary>
    Task<ConfigurationOperationResponseDTO> UpdateDistractionThresholdsAsync(UpdateDistractionThresholdsDTO dto);

    /// <summary>
    /// Actualiza los umbrales de alertas críticas.
    /// </summary>
    Task<ConfigurationOperationResponseDTO> UpdateCriticalAlertsThresholdsAsync(UpdateCriticalAlertsThresholdsDTO dto);

    /// <summary>
    /// Actualiza la configuración de puntuación de seguridad.
    /// </summary>
    Task<ConfigurationOperationResponseDTO> UpdateSafetyScoreConfigurationAsync(UpdateSafetyScoreConfigurationDTO dto);

    /// <summary>
    /// Activa una configuración.
    /// </summary>
    Task<ConfigurationOperationResponseDTO> ActivateConfigurationAsync(int configurationId, int userId);

    /// <summary>
    /// Desactiva una configuración.
    /// </summary>
    Task<ConfigurationOperationResponseDTO> DeactivateConfigurationAsync(int configurationId, int userId);

    /// <summary>
    /// Elimina una configuración (solo si no es la predeterminada).
    /// </summary>
    Task<ConfigurationOperationResponseDTO> DeleteConfigurationAsync(int configurationId);
}
