using SafeVisionPlatform.Driver.Domain.Model.ValueObjects;

namespace SafeVisionPlatform.Driver.Domain.Model.Commands;

/// <summary>
/// Comando para registrar un nuevo conductor en el sistema.
/// </summary>
public record RegisterDriverCommand(
    int UserId,
    string Username,
    string EncryptedPassword,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string Address,
    int YearsExperience,
    string LicenseNumber,
    string LicenseCategory,
    DateTime LicenseIssuedDate,
    DateTime LicenseExpirationDate
);
