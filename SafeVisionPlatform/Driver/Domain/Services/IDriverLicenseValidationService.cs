using SafeVisionPlatform.Driver.Domain.Model.Entities;

namespace SafeVisionPlatform.Driver.Domain.Services;

/// <summary>
/// Servicio de dominio que valida la autenticidad y vigencia de la licencia de conducir.
/// Aplica reglas de negocio específicas según la categoría y jurisdicción.
/// </summary>
public interface IDriverLicenseValidationService
{
    /// <summary>
    /// Valida una licencia de conducir.
    /// </summary>
    Task<bool> ValidateLicenseAsync(DriverLicense license);

    /// <summary>
    /// Verifica si una licencia está expirada.
    /// </summary>
    bool IsLicenseExpired(DriverLicense license);

    /// <summary>
    /// Verifica si una licencia es válida.
    /// </summary>
    bool IsLicenseValid(DriverLicense license);
}
