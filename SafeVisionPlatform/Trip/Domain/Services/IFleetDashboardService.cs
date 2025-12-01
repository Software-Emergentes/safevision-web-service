using SafeVisionPlatform.Trip.Application.Internal.DTO;

namespace SafeVisionPlatform.Trip.Domain.Services;

/// <summary>
/// Servicio de dominio para generar datos del dashboard de flota.
/// Proporciona información en tiempo real sobre el estado de la flota.
/// </summary>
public interface IFleetDashboardService
{
    /// <summary>
    /// Obtiene el dashboard completo de la flota con información en tiempo real.
    /// </summary>
    /// <returns>Datos agregados del dashboard de flota</returns>
    Task<FleetDashboardDTO> GetFleetDashboardAsync();

    /// <summary>
    /// Obtiene lista de conductores actualmente en riesgo.
    /// </summary>
    /// <returns>Lista de conductores en riesgo</returns>
    Task<IEnumerable<DriverAtRiskDTO>> GetDriversAtRiskAsync();

    /// <summary>
    /// Obtiene estadísticas de la flota para un rango de fechas.
    /// </summary>
    /// <param name="startDate">Fecha de inicio</param>
    /// <param name="endDate">Fecha de fin</param>
    /// <returns>Estadísticas de la flota</returns>
    Task<FleetStatisticsDTO> GetFleetStatisticsAsync(DateTime startDate, DateTime endDate);
}
