using SafeVisionPlatform.Trip.Domain.Model.Entities;

namespace SafeVisionPlatform.Trip.Domain.Repositories;

/// <summary>
/// Repositorio para la gestión de configuraciones de seguridad.
/// </summary>
public interface ISecurityConfigurationRepository
{
    /// <summary>
    /// Obtiene una configuración por su ID.
    /// </summary>
    Task<SecurityConfiguration?> FindByIdAsync(int id);

    /// <summary>
    /// Obtiene la configuración predeterminada del sistema.
    /// </summary>
    Task<SecurityConfiguration?> GetDefaultConfigurationAsync();

    /// <summary>
    /// Obtiene la configuración activa para un gerente específico.
    /// </summary>
    Task<SecurityConfiguration?> GetActiveConfigurationByManagerIdAsync(int managerId);

    /// <summary>
    /// Obtiene la configuración activa para una flota específica.
    /// </summary>
    Task<SecurityConfiguration?> GetActiveConfigurationByFleetIdAsync(int fleetId);

    /// <summary>
    /// Obtiene todas las configuraciones de un gerente.
    /// </summary>
    Task<IEnumerable<SecurityConfiguration>> GetAllByManagerIdAsync(int managerId);

    /// <summary>
    /// Obtiene todas las configuraciones activas.
    /// </summary>
    Task<IEnumerable<SecurityConfiguration>> GetAllActiveConfigurationsAsync();

    /// <summary>
    /// Obtiene todas las configuraciones del sistema.
    /// </summary>
    Task<IEnumerable<SecurityConfiguration>> GetAllConfigurationsAsync();

    /// <summary>
    /// Agrega una nueva configuración.
    /// </summary>
    Task AddAsync(SecurityConfiguration configuration);

    /// <summary>
    /// Actualiza una configuración existente.
    /// </summary>
    Task UpdateAsync(SecurityConfiguration configuration);

    /// <summary>
    /// Elimina una configuración.
    /// </summary>
    Task DeleteAsync(SecurityConfiguration configuration);
}
