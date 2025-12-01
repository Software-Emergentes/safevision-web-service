using SafeVisionPlatform.Driver.Application.DTOs;
using SafeVisionPlatform.Driver.Domain.Model.Commands;

namespace SafeVisionPlatform.Driver.Application.Assemblers;

/// <summary>
/// Ensamblador para convertir DTOs a Comandos de Registro de Conductor.
/// </summary>
public class RegisterDriverCommandFromResourceAssembler
{
    public static RegisterDriverCommand ToCommandFromResource(DriverRegistrationDTO resource)
    {
        return new RegisterDriverCommand(
            resource.UserId,
            resource.Username,
            resource.EncryptedPassword,
            resource.FirstName,
            resource.LastName,
            resource.PhoneNumber,
            resource.Email,
            resource.Address,
            resource.YearsExperience,
            resource.LicenseNumber,
            resource.LicenseCategory,
            resource.LicenseIssuedDate,
            resource.LicenseExpirationDate
        );
    }
}

