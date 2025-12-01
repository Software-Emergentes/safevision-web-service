namespace SafeVisionPlatform.Trip.Domain.Model.ValueObjects;

/// <summary>
/// Objeto de valor que define los estados válidos del viaje.
/// Asegura la consistencia en las transiciones de estado.
/// </summary>
public enum TripStatus
{
    Initiated = 1,    // Viaje iniciado pero no comenzado
    InProgress = 2,   // Viaje en curso
    Completed = 3,    // Viaje finalizado
    Cancelled = 4     // Viaje cancelado
}

