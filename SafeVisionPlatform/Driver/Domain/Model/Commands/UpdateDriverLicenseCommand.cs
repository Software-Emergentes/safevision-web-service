namespace SafeVisionPlatform.Driver.Domain.Model.Commands;

/// <summary>
/// Comando para actualizar la licencia de conducir de un conductor.
/// </summary>
public record UpdateDriverLicenseCommand(
    int DriverId,
    string LicenseNumber,
    string Category,
    DateTime IssuedDate,
    DateTime ExpirationDate
);

