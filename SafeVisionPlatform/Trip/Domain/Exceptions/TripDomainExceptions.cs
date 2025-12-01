namespace SafeVisionPlatform.Trip.Domain.Exceptions;

/// <summary>
/// Excepciones personalizadas del dominio Trip.
/// </summary>

/// <summary>
/// Excepción lanzada cuando no se puede iniciar un viaje.
/// </summary>
public class TripCannotBeStartedException : Exception
{
    public TripCannotBeStartedException(string message = "No se puede iniciar el viaje en su estado actual.")
        : base(message)
    {
    }
}

/// <summary>
/// Excepción lanzada cuando no se puede finalizar un viaje.
/// </summary>
public class TripCannotBeEndedException : Exception
{
    public TripCannotBeEndedException(string message = "No se puede finalizar el viaje en su estado actual.")
        : base(message)
    {
    }
}

/// <summary>
/// Excepción lanzada cuando no se puede cancelar un viaje.
/// </summary>
public class TripCannotBeCancelledException : Exception
{
    public TripCannotBeCancelledException(string message = "No se puede cancelar el viaje en su estado actual.")
        : base(message)
    {
    }
}

/// <summary>
/// Excepción lanzada cuando el conductor no está disponible.
/// </summary>
public class DriverNotAvailableException : Exception
{
    public int DriverId { get; }

    public DriverNotAvailableException(int driverId, string message = "El conductor no está disponible.")
        : base(message)
    {
        DriverId = driverId;
    }
}

/// <summary>
/// Excepción lanzada cuando el vehículo no está disponible.
/// </summary>
public class VehicleNotAvailableException : Exception
{
    public int VehicleId { get; }

    public VehicleNotAvailableException(int vehicleId, string message = "El vehículo no está disponible.")
        : base(message)
    {
        VehicleId = vehicleId;
    }
}

/// <summary>
/// Excepción lanzada cuando hay un error en la política de datos del viaje.
/// </summary>
public class InvalidTripDataPolicyException : Exception
{
    public InvalidTripDataPolicyException(string message = "La política de datos del viaje es inválida.")
        : base(message)
    {
    }
}

/// <summary>
/// Excepción lanzada cuando no se puede sincronizar los datos del viaje.
/// </summary>
public class TripDataSyncException : Exception
{
    public int TripId { get; }

    public TripDataSyncException(int tripId, string message = "No se pudieron sincronizar los datos del viaje.")
        : base(message)
    {
        TripId = tripId;
    }
}

/// <summary>
/// Excepción lanzada cuando no se puede generar un reporte del viaje.
/// </summary>
public class TripReportGenerationException : Exception
{
    public int TripId { get; }

    public TripReportGenerationException(int tripId, string message = "No se pudo generar el reporte del viaje.")
        : base(message)
    {
        TripId = tripId;
    }
}

