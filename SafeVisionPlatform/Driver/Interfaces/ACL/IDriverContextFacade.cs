using SafeVisionPlatform.Driver.Application.DTOs;

namespace SafeVisionPlatform.Driver.Interfaces.ACL;

/// <summary>
/// Fachada ACL (Anti-Corruption Layer) que expone funcionalidades del contexto Driver 
/// hacia otros bounded contexts de forma controlada.
/// </summary>
public interface IDriverContextFacade
{
    /// <summary>
    /// Verifica si un conductor está disponible para viajes.
    /// </summary>
    Task<bool> IsDriverAvailableAsync(int driverId);

    /// <summary>
    /// Obtiene información básica de un conductor.
    /// </summary>
    Task<DriverDTO?> GetDriverBasicInfoAsync(int driverId);

    /// <summary>
    /// Verifica si la licencia de un conductor es válida.
    /// </summary>
    Task<bool> IsDriverLicenseValidAsync(int driverId);

    /// <summary>
    /// Obtiene el estado actual de un conductor.
    /// </summary>
    Task<string?> GetDriverStatusAsync(int driverId);
}
