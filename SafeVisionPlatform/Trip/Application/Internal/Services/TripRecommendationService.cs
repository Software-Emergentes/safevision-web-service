using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Domain.Model.Entities;
using SafeVisionPlatform.Trip.Domain.Repositories;
using SafeVisionPlatform.Trip.Domain.Services;

namespace SafeVisionPlatform.Trip.Application.Internal.Services;

/// <summary>
/// Implementación del servicio de recomendaciones de viaje.
/// Genera recomendaciones personalizadas basadas en el análisis del viaje.
/// </summary>
public class TripRecommendationService : ITripRecommendationService
{
    private readonly ITripRepository _tripRepository;
    private readonly IAlertRepository _alertRepository;

    public TripRecommendationService(
        ITripRepository tripRepository,
        IAlertRepository alertRepository)
    {
        _tripRepository = tripRepository;
        _alertRepository = alertRepository;
    }

    public async Task<TripRecommendationsDTO> GenerateRecommendationsAsync(int tripId)
    {
        var trip = await _tripRepository.FindByIdAsync(tripId);
        if (trip == null)
            throw new InvalidOperationException($"Trip with ID {tripId} not found");

        var alerts = await _alertRepository.GetAlertsByTripIdAsync(tripId);
        var alertsList = alerts.ToList();

        // Calcular estadísticas
        var statistics = new TripStatisticsDTO
        {
            TotalAlerts = alertsList.Count,
            DrowsinessAlerts = alertsList.Count(a => a.AlertType == AlertType.Drowsiness),
            DistractionAlerts = alertsList.Count(a => a.AlertType == AlertType.Distraction),
            MicroSleepAlerts = alertsList.Count(a => a.AlertType == AlertType.MicroSleep),
            DurationMinutes = trip.Time.EndTime.HasValue
                ? (int)(trip.Time.EndTime.Value - trip.Time.StartTime).TotalMinutes
                : 0,
            DistanceKm = 0,
            StartTime = trip.Time.StartTime,
            EndTime = trip.Time.EndTime
        };

        // Calcular puntuación de seguridad
        var safetyScore = await CalculateSafetyScoreAsync(tripId);

        // Determinar nivel de riesgo
        var riskLevel = DetermineRiskLevel(statistics);

        // Generar recomendaciones
        var recommendations = GeneratePersonalizedRecommendations(statistics, trip.Time.StartTime.Hour);

        return new TripRecommendationsDTO
        {
            TripId = tripId,
            DriverId = trip.DriverId,
            RiskLevel = riskLevel,
            SafetyScore = safetyScore,
            Recommendations = recommendations,
            Statistics = statistics
        };
    }

    public async Task<int> CalculateSafetyScoreAsync(int tripId)
    {
        var trip = await _tripRepository.FindByIdAsync(tripId);
        if (trip == null)
            return 0;

        var alerts = await _alertRepository.GetAlertsByTripIdAsync(tripId);
        var alertsList = alerts.ToList();

        // Puntuación base
        int score = 100;

        // Penalizaciones por tipo de alerta
        var drowsinessAlerts = alertsList.Count(a => a.AlertType == AlertType.Drowsiness);
        var distractionAlerts = alertsList.Count(a => a.AlertType == AlertType.Distraction);
        var microSleepAlerts = alertsList.Count(a => a.AlertType == AlertType.MicroSleep);

        // Penalizar alertas críticas más severamente
        score -= drowsinessAlerts * 10;
        score -= microSleepAlerts * 15;
        score -= distractionAlerts * 5;

        // Bonificaciones por buenas prácticas
        var duration = trip.Time.EndTime.HasValue
            ? (trip.Time.EndTime.Value - trip.Time.StartTime).TotalHours
            : 0;

        // Bonificación si el viaje fue corto y sin alertas
        if (duration < 2 && alertsList.Count == 0)
            score += 10;

        // Limitar entre 0 y 100
        return Math.Max(0, Math.Min(100, score));
    }

    private string DetermineRiskLevel(TripStatisticsDTO statistics)
    {
        var criticalAlerts = statistics.DrowsinessAlerts + statistics.MicroSleepAlerts;

        if (criticalAlerts >= 5)
            return "Critical";
        if (criticalAlerts >= 3)
            return "High";
        if (statistics.TotalAlerts >= 5)
            return "Medium";
        return "Low";
    }

    private List<RecommendationDTO> GeneratePersonalizedRecommendations(
        TripStatisticsDTO statistics,
        int startHour)
    {
        var recommendations = new List<RecommendationDTO>();

        // Recomendación de descanso basada en alertas
        if (statistics.DrowsinessAlerts >= 3)
        {
            recommendations.Add(new RecommendationDTO
            {
                Category = "Rest",
                Priority = "High",
                Title = "Descanso obligatorio recomendado",
                Description = $"Has presentado {statistics.DrowsinessAlerts} alertas de somnolencia. " +
                             "Te recomendamos tomar un descanso de al menos 20-30 minutos antes de continuar conduciendo.",
                Icon = "😴"
            });
        }

        // Recomendación de micro-sueños
        if (statistics.MicroSleepAlerts > 0)
        {
            recommendations.Add(new RecommendationDTO
            {
                Category = "Health",
                Priority = "High",
                Title = "Atención: Episodios de micro-sueño detectados",
                Description = $"Se detectaron {statistics.MicroSleepAlerts} episodios de micro-sueño. " +
                             "Esto es muy peligroso. Considera consultar con un médico si esto ocurre frecuentemente.",
                Icon = "⚠️"
            });
        }

        // Recomendación de hidratación
        if (statistics.DurationMinutes > 120)
        {
            recommendations.Add(new RecommendationDTO
            {
                Category = "Hydration",
                Priority = "Medium",
                Title = "Mantente hidratado",
                Description = "En viajes largos, es importante mantenerse hidratado. " +
                             "Bebe agua regularmente y evita el exceso de cafeína.",
                Icon = "💧"
            });
        }

        // Recomendación de horario
        if (startHour >= 0 && startHour < 6)
        {
            recommendations.Add(new RecommendationDTO
            {
                Category = "Schedule",
                Priority = "High",
                Title = "Viaje en horario de alto riesgo",
                Description = "Conducir en la madrugada (12 AM - 6 AM) incrementa significativamente el riesgo de fatiga. " +
                             "Si es posible, planifica tus viajes en horarios diurnos.",
                Icon = "🌙"
            });
        }
        else if (startHour >= 14 && startHour < 16)
        {
            recommendations.Add(new RecommendationDTO
            {
                Category = "Schedule",
                Priority = "Medium",
                Title = "Horario post-almuerzo",
                Description = "Las horas después del almuerzo (2 PM - 4 PM) son propensas a somnolencia. " +
                             "Mantente alerta y considera una breve pausa si sientes fatiga.",
                Icon = "☀️"
            });
        }

        // Recomendación de ejercicio si hay distracciones
        if (statistics.DistractionAlerts >= 5)
        {
            recommendations.Add(new RecommendationDTO
            {
                Category = "Exercise",
                Priority = "Medium",
                Title = "Mejora tu concentración",
                Description = "Las distracciones frecuentes pueden indicar fatiga mental. " +
                             "Ejercicios de respiración y estiramientos pueden ayudar a mantener el enfoque.",
                Icon = "🧘"
            });
        }

        // Felicitación si el viaje fue seguro
        if (statistics.TotalAlerts == 0)
        {
            recommendations.Add(new RecommendationDTO
            {
                Category = "Rest",
                Priority = "Low",
                Title = "¡Excelente trabajo!",
                Description = "Completaste este viaje sin alertas de fatiga. Mantén estos buenos hábitos de conducción segura.",
                Icon = "✅"
            });
        }

        // Recomendación general si hay pocas alertas
        if (statistics.TotalAlerts > 0 && statistics.TotalAlerts < 3)
        {
            recommendations.Add(new RecommendationDTO
            {
                Category = "Rest",
                Priority = "Low",
                Title = "Mantén tu rutina de descanso",
                Description = "Has tenido algunas alertas menores. Asegúrate de dormir 7-8 horas antes de tus viajes largos.",
                Icon = "😊"
            });
        }

        return recommendations;
    }
}
