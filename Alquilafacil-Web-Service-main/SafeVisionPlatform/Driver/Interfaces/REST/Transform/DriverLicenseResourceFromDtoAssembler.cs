using SafeVisionPlatform.Driver.Application.DTOs;
using SafeVisionPlatform.Driver.Interfaces.REST.Resources;

namespace SafeVisionPlatform.Driver.Interfaces.REST.Transform;

/// <summary>
/// Ensamblador para convertir DTOs a recursos REST de licencia de conductor.
/// </summary>
public class DriverLicenseResourceFromDtoAssembler
{
    public static DriverLicenseResource ToResource(DriverLicenseDTO dto)
    {
        return new DriverLicenseResource
        {
            Id = dto.Id,
            LicenseNumber = dto.LicenseNumber,
            Category = dto.Category,
            IssuedDate = dto.IssuedDate,
            ExpirationDate = dto.ExpirationDate,
            IsValidated = dto.IsValidated,
            ValidatedDate = dto.ValidatedDate,
            IsExpired = dto.IsExpired
        };
    }
}

