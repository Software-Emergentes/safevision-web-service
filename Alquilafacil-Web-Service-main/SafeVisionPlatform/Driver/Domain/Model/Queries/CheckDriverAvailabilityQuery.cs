namespace SafeVisionPlatform.Driver.Domain.Model.Queries;

/// <summary>
/// Consulta para verificar la disponibilidad de un conductor para asignación de viajes.
/// </summary>
public record CheckDriverAvailabilityQuery(int DriverId);
