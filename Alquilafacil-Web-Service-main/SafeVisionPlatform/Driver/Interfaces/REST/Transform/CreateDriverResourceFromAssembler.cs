using SafeVisionPlatform.Driver.Application.DTOs;
using SafeVisionPlatform.Driver.Interfaces.REST.Resources;

namespace SafeVisionPlatform.Driver.Interfaces.REST.Transform;

/// <summary>
/// Ensamblador para convertir recursos REST a DTOs de aplicación.
/// </summary>
public class CreateDriverResourceFromAssembler
{
    public static DriverRegistrationDTO ToDto(CreateDriverResource resource)
    {
        return new DriverRegistrationDTO
        {
            UserId = resource.UserId,
            Username = resource.Username,
            EncryptedPassword = resource.EncryptedPassword,
            FirstName = resource.FirstName,
            LastName = resource.LastName,
            PhoneNumber = resource.PhoneNumber,
            Email = resource.Email,
            Address = resource.Address,
            YearsExperience = resource.YearsExperience,
            LicenseNumber = resource.LicenseNumber,
            LicenseCategory = resource.LicenseCategory,
            LicenseIssuedDate = resource.LicenseIssuedDate,
            LicenseExpirationDate = resource.LicenseExpirationDate
        };
    }
}

