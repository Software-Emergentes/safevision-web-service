using SafeVisionPlatform.Driver.Application.DTOs;
using SafeVisionPlatform.Driver.Interfaces.REST.Resources;

namespace SafeVisionPlatform.Driver.Interfaces.REST.Transform;

/// <summary>
/// Ensamblador para convertir recursos REST a DTOs de perfil de conductor.
/// </summary>
public class UpdateDriverProfileResourceFromAssembler
{
    public static DriverProfileDTO ToDto(UpdateDriverProfileResource resource)
    {
        return new DriverProfileDTO
        {
            FirstName = resource.FirstName,
            LastName = resource.LastName,
            PhoneNumber = resource.PhoneNumber,
            Email = resource.Email,
            Address = resource.Address,
            YearsExperience = resource.YearsExperience
        };
    }
}
