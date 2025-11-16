namespace SafeVisionPlatform.Trip.Application.Internal.DTO;

/// <summary>
/// DTO para el dashboard general de la flota.
/// </summary>
public class FleetDashboardDTO
{
    /// <summary>
    /// Total de conductores activos en este momento.
    /// </summary>
    public int ActiveDriversCount { get; set; }

    /// <summary>
    /// Total de viajes activos en curso.
    /// </summary>
    public int ActiveTripsCount { get; set; }

    /// <summary>
    /// Total de alertas emitidas hoy.
    /// </summary>
    public int TodayAlertsCount { get; set; }

    /// <summary>
    /// Total de viajes completados hoy.
    /// </summary>
    public int TodayCompletedTripsCount { get; set; }

    /// <summary>
    /// Lista de viajes activos con información detallada.
    /// </summary>
    public IEnumerable<ActiveTripSummaryDTO> ActiveTrips { get; set; } = new List<ActiveTripSummaryDTO>();

    /// <summary>
    /// Lista de conductores en riesgo (con alertas críticas recientes).
    /// </summary>
    public IEnumerable<DriverAtRiskDTO> DriversAtRisk { get; set; } = new List<DriverAtRiskDTO>();

    /// <summary>
    /// Estadísticas agregadas de la flota.
    /// </summary>
    public FleetStatisticsDTO Statistics { get; set; } = new FleetStatisticsDTO();
}

/// <summary>
/// DTO para información resumida de un viaje activo.
/// </summary>
public class ActiveTripSummaryDTO
{
    public int TripId { get; set; }
    public int DriverId { get; set; }
    public string? DriverName { get; set; }
    public int VehicleId { get; set; }
    public DateTime StartTime { get; set; }
    public int DurationMinutes { get; set; }
    public int AlertCount { get; set; }
    public int CriticalAlertsCount { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// DTO para conductores en situación de riesgo.
/// </summary>
public class DriverAtRiskDTO
{
    public int DriverId { get; set; }
    public string? DriverName { get; set; }
    public int TripId { get; set; }
    public int CriticalAlertsCount { get; set; }
    public DateTime LastAlertTime { get; set; }
    public string RiskLevel { get; set; } = string.Empty; // "Low", "Medium", "High", "Critical"
}

/// <summary>
/// DTO para estadísticas agregadas de la flota.
/// </summary>
public class FleetStatisticsDTO
{
    /// <summary>
    /// Total de kilómetros recorridos hoy.
    /// </summary>
    public double TotalDistanceToday { get; set; }

    /// <summary>
    /// Total de minutos de conducción hoy.
    /// </summary>
    public int TotalDrivingMinutesToday { get; set; }

    /// <summary>
    /// Promedio de alertas por viaje hoy.
    /// </summary>
    public double AverageAlertsPerTripToday { get; set; }

    /// <summary>
    /// Porcentaje de viajes completados sin alertas críticas.
    /// </summary>
    public double SafeTripsPercentage { get; set; }

    /// <summary>
    /// Total de conductores únicos activos hoy.
    /// </summary>
    public int UniqueDriversToday { get; set; }
}
