using SafeVisionPlatform.Driver.Domain.Model.Aggregates;

namespace SafeVisionPlatform.Driver.Domain.Services;

/// <summary>
/// Servicio de dominio que determina si un conductor está disponible para ser asignado a un viaje.
/// Evalúa su estado actual y condiciones operativas.
/// </summary>
public interface IDriverAvailabilityService
{
    /// <summary>
    /// Verifica si un conductor está disponible para viajes.
    /// </summary>
    bool IsDriverAvailable(DriverAggregate driver);

    /// <summary>
    /// Verifica si un conductor puede ser asignado a un viaje.
    /// </summary>
    bool CanBeAssignedToTrip(DriverAggregate driver);

    /// <summary>
    /// Verifica si el conductor está actualmente en un viaje.
    /// </summary>
    bool IsInTrip(DriverAggregate driver);
}
