namespace SafeVisionPlatform.Trip.Application.Internal.DTO;

/// <summary>
/// DTO para notificación crítica al gerente.
/// </summary>
public class CriticalNotificationDTO
{
    /// <summary>
    /// ID de la notificación.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID del conductor en riesgo.
    /// </summary>
    public int DriverId { get; set; }

    /// <summary>
    /// Nombre del conductor (si está disponible).
    /// </summary>
    public string? DriverName { get; set; }

    /// <summary>
    /// ID del viaje activo.
    /// </summary>
    public int TripId { get; set; }

    /// <summary>
    /// ID del gerente destinatario.
    /// </summary>
    public int? ManagerId { get; set; }

    /// <summary>
    /// Nivel de severidad: "Low", "Medium", "High", "Critical".
    /// </summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de alerta que disparó la notificación.
    /// </summary>
    public int AlertType { get; set; }

    /// <summary>
    /// Cantidad de alertas críticas en el viaje actual.
    /// </summary>
    public int CriticalAlertsCount { get; set; }

    /// <summary>
    /// Mensaje de la notificación.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora de la notificación.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Estado de la notificación: "Pending", "Sent", "Read", "Acknowledged".
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Canal de notificación: "InApp", "Email", "SMS", "Push".
    /// </summary>
    public string Channel { get; set; } = "InApp";
}

/// <summary>
/// DTO para crear una notificación crítica.
/// </summary>
public class CreateCriticalNotificationDTO
{
    public int DriverId { get; set; }
    public int TripId { get; set; }
    public int? ManagerId { get; set; }
    public string Severity { get; set; } = "High";
    public int AlertType { get; set; }
    public int CriticalAlertsCount { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Channel { get; set; } = "InApp";
}

/// <summary>
/// DTO para respuesta de notificación enviada.
/// </summary>
public class NotificationResponseDTO
{
    public int NotificationId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}
