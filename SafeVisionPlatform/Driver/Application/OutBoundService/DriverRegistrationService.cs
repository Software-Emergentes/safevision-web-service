using SafeVisionPlatform.Driver.Domain.Repositories;
using SafeVisionPlatform.Driver.Domain.Model.Entities;
using SafeVisionPlatform.Driver.Domain.Model.ValueObjects;
using SafeVisionPlatform.Driver.Domain.Services;

namespace SafeVisionPlatform.Driver.Application.OutBoundService;

/// <summary>
/// Implementación del servicio de dominio para registración de conductores.
/// Coordina el proceso de validación e registro.
/// </summary>
public class DriverRegistrationService : IDriverRegistrationService
{
    private readonly IDriverLicenseValidationService _licenseValidationService;

    public DriverRegistrationService(IDriverLicenseValidationService licenseValidationService)
    {
        _licenseValidationService = licenseValidationService;
    }

    public async Task<Domain.Model.Aggregates.DriverAggregate> RegisterDriverAsync(
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
        DateTime licenseExpirationDate)
    {
        // Validar y crear componentes
        var credentials = DriverCredentials.Create(username, encryptedPassword);
        var contactInfo = ContactInformation.Create(phoneNumber, email, address);
        var profile = new DriverProfile(firstName, lastName, phoneNumber, email, address, yearsExperience);
        var license = new DriverLicense(
            LicenseNumber.Create(licenseNumber),
            licenseCategory,
            licenseIssuedDate,
            licenseExpirationDate
        );

        // Validar licencia
        var isLicenseValid = await _licenseValidationService.ValidateLicenseAsync(license);
        if (!isLicenseValid)
            throw new InvalidOperationException("License validation failed. The provided license is not valid.");

        license.ValidateLicense();

        // Crear agregado
        var driver = new Domain.Model.Aggregates.DriverAggregate(
            userId,
            credentials,
            contactInfo,
            profile,
            license
        );

        return driver;
    }
}

