using SafeVisionPlatform.Driver.Application.DTOs;

namespace SafeVisionPlatform.Driver.Application.CommandServices;

/// <summary>
/// Interfaz del servicio de aplicación que gestiona comandos de conductor.
/// </summary>
public interface IDriverCommandService
{
    /// <summary>
    /// Registra un nuevo conductor.
    /// </summary>
    Task<DriverDTO> RegisterDriverAsync(DriverRegistrationDTO registrationDto);

    /// <summary>
    /// Actualiza el perfil de un conductor.
    /// </summary>
    Task<DriverDTO> UpdateProfileAsync(int driverId, DriverProfileDTO profileDto);

    /// <summary>
    /// Actualiza la licencia de un conductor.
    /// </summary>
    Task<DriverLicenseDTO> UpdateLicenseAsync(int driverId, DriverLicenseDTO licenseDto);

    /// <summary>
    /// Cambia el estado de un conductor.
    /// </summary>
    Task<DriverDTO> ChangeStatusAsync(int driverId, int statusValue);

    /// <summary>
    /// Valida la licencia de un conductor.
    /// </summary>
    Task<DriverLicenseDTO> ValidateLicenseAsync(int driverId);

    /// <summary>
    /// Activa un conductor.
    /// </summary>
    Task<DriverDTO> ActivateDriverAsync(int driverId);

    /// <summary>
    /// Desactiva un conductor.
    /// </summary>
    Task<DriverDTO> DeactivateDriverAsync(int driverId);

    /// <summary>
    /// Suspende un conductor.
    /// </summary>
    Task<DriverDTO> SuspendDriverAsync(int driverId);
}
