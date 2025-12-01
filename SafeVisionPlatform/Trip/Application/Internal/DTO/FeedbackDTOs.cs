namespace SafeVisionPlatform.Trip.Application.Internal.DTO;

/// <summary>
/// DTO para enviar feedback sobre una alerta.
/// </summary>
public class AlertFeedbackDTO
{
    /// <summary>
    /// ID de la alerta sobre la que se envía feedback.
    /// </summary>
    public int AlertId { get; set; }

    /// <summary>
    /// ID del conductor que envía el feedback.
    /// </summary>
    public int DriverId { get; set; }

    /// <summary>
    /// Indica si la alerta es una falsa alarma.
    /// </summary>
    public bool IsFalseAlarm { get; set; }

    /// <summary>
    /// Comentario opcional del conductor.
    /// </summary>
    public string? Comment { get; set; }
}

/// <summary>
/// DTO para la respuesta de feedback enviado.
/// </summary>
public class FeedbackResponseDTO
{
    public int AlertId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
}

/// <summary>
/// DTO para estadísticas de feedback del sistema.
/// </summary>
public class FeedbackStatisticsDTO
{
    /// <summary>
    /// Total de alertas evaluadas.
    /// </summary>
    public int TotalAlertsEvaluated { get; set; }

    /// <summary>
    /// Total de alertas marcadas como falsas alarmas.
    /// </summary>
    public int TotalFalseAlarms { get; set; }

    /// <summary>
    /// Porcentaje de falsas alarmas.
    /// </summary>
    public double FalseAlarmPercentage { get; set; }

    /// <summary>
    /// Distribución de falsas alarmas por tipo de alerta.
    /// </summary>
    public Dictionary<int, int> FalseAlarmsByType { get; set; } = new Dictionary<int, int>();

    /// <summary>
    /// Alertas con feedback más recientes.
    /// </summary>
    public List<AlertWithFeedbackDTO> RecentFeedback { get; set; } = new List<AlertWithFeedbackDTO>();
}

/// <summary>
/// DTO para alerta con información de feedback.
/// </summary>
public class AlertWithFeedbackDTO
{
    public int AlertId { get; set; }
    public int TripId { get; set; }
    public int AlertType { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public bool MarkedAsFalseAlarm { get; set; }
    public string? FeedbackComment { get; set; }
    public DateTime? FeedbackSubmittedAt { get; set; }
    public int? FeedbackSubmittedBy { get; set; }
}
