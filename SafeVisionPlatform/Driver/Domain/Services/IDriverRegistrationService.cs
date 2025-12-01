using SafeVisionPlatform.Driver.Domain.Model.Aggregates;

namespace SafeVisionPlatform.Driver.Domain.Services;

/// <summary>
/// Servicio de dominio que coordina el proceso de registro de un nuevo conductor.
/// Valida datos personales, credenciales y verifica que la licencia esté vigente.
/// </summary>
public interface IDriverRegistrationService
{
    /// <summary>
    /// Registra un nuevo conductor en el sistema.
    /// </summary>
    Task<DriverAggregate> RegisterDriverAsync(
        int userId,
        string username,
        string encryptedPassword,
        string firstName,
        string lastName,
        string phoneNumber,
        string email,
        string address,
        int yearsExperience,
        string licenseNumber,
        string licenseCategory,
        DateTime licenseIssuedDate,
        DateTime licenseExpirationDate);
}

