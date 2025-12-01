using SafeVisionPlatform.Trip.Application.Internal.DTO;

namespace SafeVisionPlatform.Trip.Domain.Services;

/// <summary>
/// Servicio de dominio para generar recomendaciones personalizadas de viaje.
/// </summary>
public interface ITripRecommendationService
{
    /// <summary>
    /// Genera recomendaciones personalizadas después de un viaje.
    /// </summary>
    Task<TripRecommendationsDTO> GenerateRecommendationsAsync(int tripId);

    /// <summary>
    /// Calcula la puntuación de seguridad de un viaje.
    /// </summary>
    Task<int> CalculateSafetyScoreAsync(int tripId);
}
