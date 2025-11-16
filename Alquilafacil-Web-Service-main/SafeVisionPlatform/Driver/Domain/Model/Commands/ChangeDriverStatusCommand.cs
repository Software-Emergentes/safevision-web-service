using SafeVisionPlatform.Driver.Domain.Model.ValueObjects;

namespace SafeVisionPlatform.Driver.Domain.Model.Commands;

/// <summary>
/// Comando para cambiar el estado de un conductor.
/// </summary>
public record ChangeDriverStatusCommand(
    int DriverId,
    DriverStatus.Status Status
);
