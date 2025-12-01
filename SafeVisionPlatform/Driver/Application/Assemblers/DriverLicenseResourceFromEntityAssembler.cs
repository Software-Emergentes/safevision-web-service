using SafeVisionPlatform.Driver.Application.DTOs;
using SafeVisionPlatform.Driver.Domain.Model.Entities;

namespace SafeVisionPlatform.Driver.Application.Assemblers;

/// <summary>
/// Ensamblador para convertir Licencias de Conductor a DTOs.
/// </summary>
public class DriverLicenseResourceFromEntityAssembler
{
    public static DriverLicenseDTO ToResourceFromEntity(DriverLicense entity)
    {
        return new DriverLicenseDTO
        {
            Id = entity.Id,
            LicenseNumber = entity.LicenseNumber.ToString(),
            Category = entity.Category,
            IssuedDate = entity.IssuedDate,
            ExpirationDate = entity.ExpirationDate,
            IsValidated = entity.IsValidated,
            ValidatedDate = entity.ValidatedDate,
            IsExpired = entity.IsExpired()
        };
    }
}

