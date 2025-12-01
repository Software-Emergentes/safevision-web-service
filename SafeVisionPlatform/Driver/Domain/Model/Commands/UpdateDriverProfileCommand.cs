namespace SafeVisionPlatform.Driver.Domain.Model.Commands;

/// <summary>
/// Comando para actualizar el perfil de un conductor.
/// </summary>
public record UpdateDriverProfileCommand(
    int DriverId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string Address,
    int YearsExperience
);

