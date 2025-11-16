using SafeVisionPlatform.Driver.Application.DTOs;

namespace SafeVisionPlatform.Driver.Application.QueryServices;

/// <summary>
/// Interfaz del servicio de aplicación que gestiona consultas de conductor.
/// </summary>
public interface IDriverQueryService
{
    /// <summary>
    /// Obtiene un conductor por su ID.
    /// </summary>
    Task<DriverDTO?> GetDriverByIdAsync(int driverId);

    /// <summary>
    /// Obtiene todos los conductores.
    /// </summary>
    Task<IEnumerable<DriverDTO>> GetAllDriversAsync();

    /// <summary>
    /// Obtiene un conductor por su ID de usuario.
    /// </summary>
    Task<DriverDTO?> GetDriverByUserIdAsync(int userId);

    /// <summary>
    /// Obtiene conductores por estado.
    /// </summary>
    Task<IEnumerable<DriverDTO>> GetDriversByStatusAsync(int statusValue);

    /// <summary>
    /// Obtiene la licencia de un conductor.
    /// </summary>
    Task<DriverLicenseDTO?> GetDriverLicenseAsync(int driverId);

    /// <summary>
    /// Obtiene el perfil de un conductor.
    /// </summary>
    Task<DriverProfileDTO?> GetDriverProfileAsync(int driverId);

    /// <summary>
    /// Verifica si un conductor está disponible.
    /// </summary>
    Task<bool> IsDriverAvailableAsync(int driverId);
}
