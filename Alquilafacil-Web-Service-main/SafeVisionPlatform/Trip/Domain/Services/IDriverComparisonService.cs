using SafeVisionPlatform.Trip.Application.Internal.DTO;

namespace SafeVisionPlatform.Trip.Domain.Services;

/// <summary>
/// Servicio de dominio para comparación entre conductores.
/// Permite a los gerentes comparar el desempeño de diferentes conductores.
/// </summary>
public interface IDriverComparisonService
{
    /// <summary>
    /// Compara múltiples conductores en un rango de fechas.
    /// </summary>
    /// <param name="driverIds">Lista de IDs de conductores a comparar</param>
    /// <param name="startDate">Fecha de inicio</param>
    /// <param name="endDate">Fecha de fin</param>
    /// <returns>Comparación detallada de los conductores</returns>
    Task<DriverComparisonDTO> CompareDriversAsync(IEnumerable<int> driverIds, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Compara todos los conductores de la flota.
    /// </summary>
    /// <param name="startDate">Fecha de inicio</param>
    /// <param name="endDate">Fecha de fin</param>
    /// <returns>Comparación de todos los conductores</returns>
    Task<DriverComparisonDTO> CompareAllDriversAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Obtiene el ranking de conductores por puntuación de seguridad.
    /// </summary>
    /// <param name="startDate">Fecha de inicio</param>
    /// <param name="endDate">Fecha de fin</param>
    /// <param name="limit">Cantidad de conductores en el ranking</param>
    /// <returns>Lista de conductores rankeados</returns>
    Task<IEnumerable<DriverRankingDTO>> GetDriverRankingsAsync(DateTime startDate, DateTime endDate, int limit = 10);
}
