using SafeVisionPlatform.Trip.Domain.Model.ValueObjects;

namespace SafeVisionPlatform.Trip.Domain.Model.Commands;

public class StartTripCommand
{
    public int DriverId { get; set; }
    public int VehicleId { get; set; }
    public TripDataPolicy DataPolicy { get; set; }

    public StartTripCommand(int driverId, int vehicleId, TripDataPolicy? dataPolicy = null)
    {
        DriverId = driverId;
        VehicleId = vehicleId;
        DataPolicy = dataPolicy ?? new TripDataPolicy();
    }
}

public class EndTripCommand
{
    public int TripId { get; set; }

    public EndTripCommand(int tripId)
    {
        TripId = tripId;
    }
}

public class CancelTripCommand
{
    public int TripId { get; set; }
    public string? Reason { get; set; }

    public CancelTripCommand(int tripId, string? reason = null)
    {
        TripId = tripId;
        Reason = reason;
    }
}

public class SyncTripDataCommand
{
    public int TripId { get; set; }

    public SyncTripDataCommand(int tripId)
    {
        TripId = tripId;
    }
}

public class AddAlertCommand
{
    public int TripId { get; set; }
    public int AlertType { get; set; }
    public string Description { get; set; }
    public double? Severity { get; set; }

    public AddAlertCommand(int tripId, int alertType, string description, double? severity = null)
    {
        TripId = tripId;
        AlertType = alertType;
        Description = description;
        Severity = severity;
    }
}

public class GenerateTripReportCommand
{
    public int TripId { get; set; }
    public int DriverId { get; set; }
    public int VehicleId { get; set; }
    public double DistanceKm { get; set; }
    public int AlertCount { get; set; }
    public string? Notes { get; set; }

    public GenerateTripReportCommand(int tripId, int driverId, int vehicleId, double distanceKm, int alertCount, string? notes = null)
    {
        TripId = tripId;
        DriverId = driverId;
        VehicleId = vehicleId;
        DistanceKm = distanceKm;
        AlertCount = alertCount;
        Notes = notes;
    }
}

