using SafeVisionPlatform.Driver.Application.DTOs;
using SafeVisionPlatform.Driver.Interfaces.REST.Resources;

namespace SafeVisionPlatform.Driver.Interfaces.REST.Transform;

/// <summary>
/// Ensamblador para convertir DTOs a recursos REST de conductor.
/// </summary>
public class DriverResourceFromDtoAssembler
{
    public static DriverResource ToResource(DriverDTO dto)
    {
        return new DriverResource
        {
            Id = dto.Id,
            UserId = dto.UserId,
            Status = dto.Status,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            YearsExperience = dto.YearsExperience,
            RegisteredDate = dto.RegisteredDate,
            UpdatedDate = dto.UpdatedDate
        };
    }

    public static IEnumerable<DriverResource> ToResources(IEnumerable<DriverDTO> dtos)
    {
        return dtos.Select(ToResource);
    }
}

