namespace SafeVisionPlatform.Driver.Domain.Model.Queries;

/// <summary>
/// Consulta para obtener los detalles completos de un conductor.
/// </summary>
public record GetDriverByIdQuery(int DriverId);

/// <summary>
/// Consulta para obtener listado de todos los conductores registrados.
/// </summary>
public record GetAllDriversQuery;
