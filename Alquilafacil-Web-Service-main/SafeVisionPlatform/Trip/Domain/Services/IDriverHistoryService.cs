using SafeVisionPlatform.Trip.Application.Internal.DTO;

namespace SafeVisionPlatform.Trip.Domain.Services;

/// <summary>
/// Servicio de dominio para gestionar el historial del conductor.
/// Proporciona análisis y estadísticas personalizadas de viajes y alertas.
/// </summary>
public interface IDriverHistoryService
{
    /// <summary>
    /// Obtiene el historial completo de un conductor con análisis de patrones.
    /// </summary>
    /// <param name="driverId">ID del conductor</param>
    /// <returns>Historial completo con estadísticas y análisis</returns>
    Task<DriverHistoryDTO> GetDriverHistoryAsync(int driverId);

    /// <summary>
    /// Obtiene el historial de un conductor filtrado por rango de fechas.
    /// </summary>
    /// <param name="driverId">ID del conductor</param>
    /// <param name="startDate">Fecha de inicio</param>
    /// <param name="endDate">Fecha de fin</param>
    /// <returns>Historial filtrado por fechas</returns>
    Task<DriverHistoryDTO> GetDriverHistoryByDateRangeAsync(int driverId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Obtiene el análisis de patrones de fatiga de un conductor.
    /// </summary>
    /// <param name="driverId">ID del conductor</param>
    /// <returns>Análisis de patrones de fatiga</returns>
    Task<FatiguePatternDTO> GetDriverFatiguePatternsAsync(int driverId);
}
