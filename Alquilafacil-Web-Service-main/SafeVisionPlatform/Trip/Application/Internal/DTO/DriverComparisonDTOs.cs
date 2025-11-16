namespace SafeVisionPlatform.Trip.Application.Internal.DTO;

/// <summary>
/// DTO para comparación de múltiples conductores.
/// </summary>
public class DriverComparisonDTO
{
    /// <summary>
    /// Rango de fechas para la comparación.
    /// </summary>
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Lista de métricas de conductores.
    /// </summary>
    public List<DriverMetricsDTO> DriverMetrics { get; set; } = new List<DriverMetricsDTO>();

    /// <summary>
    /// Estadísticas agregadas de todos los conductores.
    /// </summary>
    public AggregateMetricsDTO AggregateMetrics { get; set; } = new AggregateMetricsDTO();

    /// <summary>
    /// Ranking de conductores ordenados por puntuación de seguridad.
    /// </summary>
    public List<DriverRankingDTO> DriverRankings { get; set; } = new List<DriverRankingDTO>();
}

/// <summary>
/// DTO para métricas individuales de un conductor.
/// </summary>
public class DriverMetricsDTO
{
    public int DriverId { get; set; }
    public string? DriverName { get; set; }

    // Métricas de viajes
    public int TotalTrips { get; set; }
    public int CompletedTrips { get; set; }
    public double TotalDistanceKm { get; set; }
    public int TotalDrivingMinutes { get; set; }

    // Métricas de alertas
    public int TotalAlerts { get; set; }
    public int DrowsinessAlerts { get; set; }
    public int DistractionAlerts { get; set; }
    public int MicroSleepAlerts { get; set; }

    // Métricas calculadas
    public double AverageAlertsPerTrip { get; set; }
    public double AlertsPerHour { get; set; }
    public double AlertsPerKm { get; set; }
    public int SafetyScore { get; set; } // 0-100

    // Porcentajes
    public double SafeTripsPercentage { get; set; }
    public double CriticalAlertsPercentage { get; set; }
}

/// <summary>
/// DTO para ranking de conductores.
/// </summary>
public class DriverRankingDTO
{
    public int Rank { get; set; }
    public int DriverId { get; set; }
    public string? DriverName { get; set; }
    public int SafetyScore { get; set; }
    public int TotalTrips { get; set; }
    public int TotalAlerts { get; set; }
    public double SafeTripsPercentage { get; set; }
}

/// <summary>
/// DTO para métricas agregadas del grupo de conductores.
/// </summary>
public class AggregateMetricsDTO
{
    public int TotalDrivers { get; set; }
    public int TotalTrips { get; set; }
    public int TotalAlerts { get; set; }
    public double AverageSafetyScore { get; set; }
    public double AverageAlertsPerTrip { get; set; }
    public double BestSafetyScore { get; set; }
    public double WorstSafetyScore { get; set; }
    public int BestDriverId { get; set; }
    public int DriversMostImprovement { get; set; }
}
