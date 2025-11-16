using SafeVisionPlatform.Driver.Domain.Model.Entities;
using SafeVisionPlatform.Driver.Domain.Services;

namespace SafeVisionPlatform.Driver.Application.OutBoundService;

/// <summary>
/// Implementación del servicio de dominio para validación de licencias de conducir.
/// </summary>
public class DriverLicenseValidationService : IDriverLicenseValidationService
{
    public async Task<bool> ValidateLicenseAsync(DriverLicense license)
    {
        // Aquí se podrían integrar servicios externos para validación de licencias
        // Por ahora, realizamos validaciones básicas
        return await Task.FromResult(ValidateBasicLicense(license));
    }

    public bool IsLicenseExpired(DriverLicense license)
    {
        return license.IsExpired();
    }

    public bool IsLicenseValid(DriverLicense license)
    {
        return license.IsValid();
    }

    private bool ValidateBasicLicense(DriverLicense license)
    {
        // Validación básica: la licencia no debe estar expirada
        if (license.IsExpired())
            return false;

        // Validación: la fecha de emisión debe ser antes que la de expiración
        if (license.IssuedDate >= license.ExpirationDate)
            return false;

        // Validación: la categoría no debe estar vacía
        if (string.IsNullOrWhiteSpace(license.Category))
            return false;

        return true;
    }
}
