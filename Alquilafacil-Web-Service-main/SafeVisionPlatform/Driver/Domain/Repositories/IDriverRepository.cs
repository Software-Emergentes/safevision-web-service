using SafeVisionPlatform.Driver.Domain.Model.Aggregates;
using SafeVisionPlatform.Shared.Domain.Repositories;

namespace SafeVisionPlatform.Driver.Domain.Repositories;

/// <summary>
/// Interfaz de repositorio que define las operaciones de persistencia del agregado Driver.
/// </summary>
public interface IDriverRepository : IBaseRepository<DriverAggregate>
{
    /// <summary>
    /// Obtiene un conductor por su ID.
    /// </summary>
    Task<DriverAggregate?> GetByIdAsync(int driverId);

    /// <summary>
    /// Obtiene todos los conductores registrados.
    /// </summary>
    Task<IEnumerable<DriverAggregate>> GetAllAsync();

    /// <summary>
    /// Obtiene un conductor por su ID de usuario.
    /// </summary>
    Task<DriverAggregate?> GetByUserIdAsync(int userId);

    /// <summary>
    /// Obtiene conductores por estado.
    /// </summary>
    Task<IEnumerable<DriverAggregate>> GetByStatusAsync(int statusValue);

    /// <summary>
    /// Verifica si existe un conductor con un usuario específico.
    /// </summary>
    Task<bool> ExistsByUserIdAsync(int userId);

    /// <summary>
    /// Añade un nuevo conductor.
    /// </summary>
    new Task AddAsync(DriverAggregate driver);

    /// <summary>
    /// Actualiza un conductor existente.
    /// </summary>
    Task UpdateAsync(DriverAggregate driver);

    /// <summary>
    /// Elimina un conductor.
    /// </summary>
    Task DeleteAsync(int driverId);
}

