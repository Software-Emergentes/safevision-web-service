using SafeVisionPlatform.Driver.Application.DTOs;
using SafeVisionPlatform.Driver.Domain.Model.Aggregates;

namespace SafeVisionPlatform.Driver.Application.Assemblers;

/// <summary>
/// Ensamblador para convertir Agregados de Conductor a DTOs.
/// </summary>
public class DriverResourceFromEntityAssembler
{
    public static DriverDTO ToResourceFromEntity(DriverAggregate entity)
    {
        return new DriverDTO
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Status = entity.Status.ToString(),
            FirstName = entity.Profile.FirstName,
            LastName = entity.Profile.LastName,
            Email = entity.Profile.Email,
            PhoneNumber = entity.Profile.PhoneNumber,
            YearsExperience = entity.Profile.YearsExperience,
            RegisteredDate = entity.RegisteredDate,
            UpdatedDate = entity.UpdatedDate
        };
    }

    public static IEnumerable<DriverDTO> ToResourceFromEntities(IEnumerable<DriverAggregate> entities)
    {
        return entities.Select(ToResourceFromEntity);
    }
}
