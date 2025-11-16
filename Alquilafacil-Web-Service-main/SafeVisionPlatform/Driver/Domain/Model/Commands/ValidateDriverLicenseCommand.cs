namespace SafeVisionPlatform.Driver.Domain.Model.Commands;

/// <summary>
/// Comando para validar la licencia de un conductor.
/// </summary>
public record ValidateDriverLicenseCommand(
    int DriverId
);
