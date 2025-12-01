using SafeVisionPlatform.Trip.Application.Internal.DTO;

namespace SafeVisionPlatform.Trip.Domain.Services;

/// <summary>
/// Servicio de dominio para gestionar feedback de alertas.
/// Permite a los conductores reportar falsas alarmas y enviar comentarios.
/// </summary>
public interface IAlertFeedbackService
{
    /// <summary>
    /// Envía feedback sobre una alerta.
    /// </summary>
    /// <param name="feedbackDto">Datos del feedback</param>
    /// <returns>Respuesta del feedback enviado</returns>
    Task<FeedbackResponseDTO> SubmitFeedbackAsync(AlertFeedbackDTO feedbackDto);

    /// <summary>
    /// Obtiene estadísticas de feedback del sistema.
    /// </summary>
    /// <returns>Estadísticas agregadas de feedback</returns>
    Task<FeedbackStatisticsDTO> GetFeedbackStatisticsAsync();

    /// <summary>
    /// Obtiene estadísticas de feedback para un conductor específico.
    /// </summary>
    /// <param name="driverId">ID del conductor</param>
    /// <returns>Estadísticas de feedback del conductor</returns>
    Task<FeedbackStatisticsDTO> GetDriverFeedbackStatisticsAsync(int driverId);

    /// <summary>
    /// Obtiene alertas con feedback más recientes.
    /// </summary>
    /// <param name="limit">Cantidad de alertas a retornar</param>
    /// <returns>Lista de alertas con feedback</returns>
    Task<IEnumerable<AlertWithFeedbackDTO>> GetRecentFeedbackAsync(int limit = 50);

    /// <summary>
    /// Obtiene alertas marcadas como falsas alarmas.
    /// </summary>
    /// <returns>Lista de falsas alarmas reportadas</returns>
    Task<IEnumerable<AlertWithFeedbackDTO>> GetFalseAlarmsAsync();
}
