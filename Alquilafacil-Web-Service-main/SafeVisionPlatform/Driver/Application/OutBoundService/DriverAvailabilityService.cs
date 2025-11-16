using SafeVisionPlatform.Driver.Domain.Services;

namespace SafeVisionPlatform.Driver.Application.OutBoundService;

/// <summary>
/// Implementación del servicio de dominio para disponibilidad de conductores.
/// </summary>
public class DriverAvailabilityService : IDriverAvailabilityService
{
    public bool IsDriverAvailable(Domain.Model.Aggregates.DriverAggregate driver)
    {
        return driver.IsAvailable();
    }

    public bool CanBeAssignedToTrip(Domain.Model.Aggregates.DriverAggregate driver)
    {
        return driver.CanBeAssignedToTrip();
    }

    public bool IsInTrip(Domain.Model.Aggregates.DriverAggregate driver)
    {
        return driver.Status.IsInTrip();
    }
}
