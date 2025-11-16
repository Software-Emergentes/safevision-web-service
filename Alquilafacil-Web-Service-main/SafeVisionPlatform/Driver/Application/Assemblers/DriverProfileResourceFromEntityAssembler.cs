using SafeVisionPlatform.Driver.Application.DTOs;
using SafeVisionPlatform.Driver.Domain.Model.Entities;

namespace SafeVisionPlatform.Driver.Application.Assemblers;

/// <summary>
/// Ensamblador para convertir Perfiles de Conductor a DTOs.
/// </summary>
public class DriverProfileResourceFromEntityAssembler
{
    public static DriverProfileDTO ToResourceFromEntity(DriverProfile entity)
    {
        return new DriverProfileDTO
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            PhotoUrl = entity.PhotoUrl,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            Address = entity.Address,
            YearsExperience = entity.YearsExperience,
            AcceptsPromotions = entity.AcceptsPromotions,
            ReceiveNotifications = entity.ReceiveNotifications,
            CreatedDate = entity.CreatedDate,
            UpdatedDate = entity.UpdatedDate
        };
    }
}

