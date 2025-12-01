using SafeVisionPlatform.Driver.Domain.Model.Entities;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.Driver.Domain.Repositories;

/// <summary>
/// Interfaz de repositorio para gestionar las licencias de conducir.
/// </summary>
public interface IDriverLicenseRepository : IBaseRepository<DriverLicense>
{
    /// <summary>
    /// Obtiene una licencia por su ID.
    /// </summary>
    Task<DriverLicense?> GetByIdAsync(int licenseId);

    /// <summary>
    /// Obtiene la licencia de un conductor específico.
    /// </summary>
    Task<DriverLicense?> GetByDriverIdAsync(int driverId);

    /// <summary>
    /// Añade una nueva licencia.
    /// </summary>
    new Task AddAsync(DriverLicense license);

    /// <summary>
    /// Actualiza una licencia existente.
    /// </summary>
    Task UpdateAsync(DriverLicense license);

    /// <summary>
    /// Elimina una licencia.
    /// </summary>
    Task DeleteAsync(int licenseId);
}

