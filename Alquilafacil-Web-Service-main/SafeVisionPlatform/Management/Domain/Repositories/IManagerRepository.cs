using SafeVisionPlatform.Management.Domain.Model.Entities;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.Management.Domain.Repositories;

/// <summary>
/// Repositorio para gestionar gerentes.
/// </summary>
public interface IManagerRepository : IBaseRepository<Manager>
{
    /// <summary>
    /// Obtiene un gerente por su UserId.
    /// </summary>
    Task<Manager?> GetByUserIdAsync(int userId);

    /// <summary>
    /// Obtiene gerentes por rol.
    /// </summary>
    Task<IEnumerable<Manager>> GetByRoleAsync(ManagerRole role);

    /// <summary>
    /// Obtiene gerentes activos.
    /// </summary>
    Task<IEnumerable<Manager>> GetActiveManagersAsync();

    /// <summary>
    /// Obtiene gerentes que gestionan una flota específica.
    /// </summary>
    Task<IEnumerable<Manager>> GetManagersByFleetIdAsync(int fleetId);

    /// <summary>
    /// Actualiza un gerente.
    /// </summary>
    Task UpdateAsync(Manager manager);
}
