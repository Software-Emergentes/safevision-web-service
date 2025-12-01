namespace SafeVisionPlatform.Trip.Application.Internal.DTO;

/// <summary>
/// DTO para recomendaciones personalizadas después de un viaje.
/// </summary>
public class TripRecommendationsDTO
{
    /// <summary>
    /// ID del viaje.
    /// </summary>
    public int TripId { get; set; }

    /// <summary>
    /// ID del conductor.
    /// </summary>
    public int DriverId { get; set; }

    /// <summary>
    /// Nivel de riesgo del viaje: "Low", "Medium", "High", "Critical".
    /// </summary>
    public string RiskLevel { get; set; } = string.Empty;

    /// <summary>
    /// Puntuación de seguridad del viaje (0-100).
    /// </summary>
    public int SafetyScore { get; set; }

    /// <summary>
    /// Lista de recomendaciones personalizadas.
    /// </summary>
    public List<RecommendationDTO> Recommendations { get; set; } = new List<RecommendationDTO>();

    /// <summary>
    /// Estadísticas del viaje.
    /// </summary>
    public TripStatisticsDTO Statistics { get; set; } = new TripStatisticsDTO();
}

/// <summary>
/// DTO para una recomendación individual.
/// </summary>
public class RecommendationDTO
{
    /// <summary>
    /// Categoría de la recomendación: "Rest", "Hydration", "Exercise", "Schedule", "Health".
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Prioridad: "Low", "Medium", "High".
    /// </summary>
    public string Priority { get; set; } = string.Empty;

    /// <summary>
    /// Título de la recomendación.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Icono o emoji representativo.
    /// </summary>
    public string Icon { get; set; } = string.Empty;
}

/// <summary>
/// DTO para estadísticas de un viaje.
/// </summary>
public class TripStatisticsDTO
{
    public int TotalAlerts { get; set; }
    public int DrowsinessAlerts { get; set; }
    public int DistractionAlerts { get; set; }
    public int MicroSleepAlerts { get; set; }
    public int DurationMinutes { get; set; }
    public double DistanceKm { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
