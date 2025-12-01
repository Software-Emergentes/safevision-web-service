using SafeVisionPlatform.Trip.Application.Internal.DTO;
using SafeVisionPlatform.Trip.Domain.Model.Aggregates;
using SafeVisionPlatform.Trip.Domain.Model.Entities;

namespace SafeVisionPlatform.Trip.Interfaces.REST.Transform;

/// <summary>
/// Ensamblador para transformar TripAggregate a TripDTO.
/// </summary>
public class TripAssembler
{
    public static TripDTO ToDTO(TripAggregate trip)
    {
        return new TripDTO
        {
            Id = trip.Id,
            DriverId = trip.DriverId,
            VehicleId = trip.VehicleId,
            Status = (int)trip.Status,
            StartTime = trip.Time.StartTime,
            EndTime = trip.Time.EndTime,
            DurationMinutes = trip.GetDurationInMinutes(),
            AlertCount = trip.Alerts.Count,
            CreatedAt = trip.CreatedAt,
            UpdatedAt = trip.UpdatedAt
        };
    }

    public static IEnumerable<TripDTO> ToDTOList(IEnumerable<TripAggregate> trips)
    {
        return trips.Select(ToDTO);
    }
}

/// <summary>
/// Ensamblador para transformar Report a TripReportDTO.
/// </summary>
public class ReportAssembler
{
    public static TripReportDTO ToDTO(Report report)
    {
        return new TripReportDTO
        {
            Id = report.Id,
            TripId = report.TripId,
            DriverId = report.DriverId,
            VehicleId = report.VehicleId,
            StartTime = report.StartTime,
            EndTime = report.EndTime,
            DurationMinutes = report.DurationMinutes,
            DistanceKm = report.DistanceKm,
            AlertCount = report.AlertCount,
            Notes = report.Notes,
            Recipient = (int)report.Recipient,
            Status = (int)report.Status,
            CreatedAt = report.CreatedAt,
            SentAt = report.SentAt
        };
    }

    public static IEnumerable<TripReportDTO> ToDTOList(IEnumerable<Report> reports)
    {
        return reports.Select(ToDTO);
    }
}

/// <summary>
/// Ensamblador para transformar Alert a AlertDTO.
/// </summary>
public class AlertAssembler
{
    public static AlertDTO ToDTO(Alert alert)
    {
        return new AlertDTO
        {
            Id = alert.Id,
            TripId = alert.TripId,
            AlertType = (int)alert.AlertType,
            Description = alert.Description,
            Severity = alert.Severity,
            DetectedAt = alert.DetectedAt,
            Acknowledged = alert.Acknowledged
        };
    }

    public static IEnumerable<AlertDTO> ToDTOList(IEnumerable<Alert> alerts)
    {
        return alerts.Select(ToDTO);
    }
}

