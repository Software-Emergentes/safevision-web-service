using SafeVisionPlatform.Driver.Application.DTOs;
using SafeVisionPlatform.Driver.Interfaces.REST.Resources;

namespace SafeVisionPlatform.Driver.Interfaces.REST.Transform;

/// <summary>
/// Ensamblador para convertir recursos REST a DTOs de licencia de conductor.
/// </summary>
public class UpdateDriverLicenseResourceFromAssembler
{
    public static DriverLicenseDTO ToDto(UpdateDriverLicenseResource resource)
    {
        return new DriverLicenseDTO
        {
            LicenseNumber = resource.LicenseNumber,
            Category = resource.Category,
            IssuedDate = resource.IssuedDate,
            ExpirationDate = resource.ExpirationDate
        };
    }
}                                                                               
                                                                    