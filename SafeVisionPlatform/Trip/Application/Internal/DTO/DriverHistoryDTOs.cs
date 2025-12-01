namespace SafeVisionPlatform.Trip.Application.Internal.DTO;

/// <summary>
/// DTO para el historial completo de un conductor.
/// </summary>
public class DriverHistoryDTO
{
    /// <summary>
    /// ID del conductor.
    /// </summary>
    public int DriverId { get; set; }

    /// <summary>
    /// Total de viajes realizados.
    /// </summary>
    public int TotalTrips { get; set; }

    /// <summary>
    /// Total de alertas recibidas.
    /// </summary>
    public int TotalAlerts { get; set; }

    /// <summary>
    /// Promedio de alertas por viaje.
    /// </summary>
    public double AverageAlertsPerTrip { get; set; }

    /// <summary>
    /// Distancia total recorrida (km).
    /// </summary>
    public double TotalDistanceKm { get; set; }

    /// <summary>
    /// Tiempo total de conducción (minutos).
    /// </summary>
    public int TotalDrivingMinutes { get; set; }

    /// <summary>
    /// Lista de viajes del conductor.
    /// </summary>
    public IEnumerable<TripDTO> Trips { get; set; } = new List<TripDTO>();

    /// <summary>
    /// Lista de todas las alertas del conductor.
    /// </summary>
    public IEnumerable<AlertDTO> Alerts { get; set; } = new List<AlertDTO>();

    /// <summary>
    /// Distribución de alertas por tipo.
    /// </summary>
    public Dictionary<int, int> AlertsByType { get; set; } = new Dictionary<int, int>();

    /// <summary>
    /// Análisis de patrones de fatiga.
    /// </summary>
    public FatiguePatternDTO FatiguePattern { get; set; } = new FatiguePatternDTO();
}

/// <summary>
/// DTO para análisis de patrones de fatiga de un conductor.
/// </summary>
public class FatiguePatternDTO
{
    /// <summary>
    /// Hora del día con más alertas (0-23).
    /// </summary>
    public int MostAlertsHour { get; set; }

    /// <summary>
    /// Cantidad de alertas en la hora pico.
    /// </summary>
    public int AlertsInPeakHour { get; set; }

    /// <summary>
    /// Franjas horarias de mayor riesgo.
    /// </summary>
    public List<HourlyAlertDistributionDTO> HourlyDistribution { get; set; } = new List<HourlyAlertDistributionDTO>();

    /// <summary>
    /// Tipo de alerta más frecuente.
    /// </summary>
    public int MostFrequentAlertType { get; set; }

    /// <summary>
    /// Porcentaje de viajes seguros (sin alertas críticas).
    /// </summary>
    public double SafeTripsPercentage { get; set; }
}

/// <summary>
/// DTO para distribución de alertas por hora.
/// </summary>
public class HourlyAlertDistributionDTO
{
    /// <summary>
    /// Hora del día (0-23).
    /// </summary>
    public int Hour { get; set; }

    /// <summary>
    /// Cantidad de alertas en esta hora.
    /// </summary>
    public int AlertCount { get; set; }
}
