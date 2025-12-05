using SafeVisionPlatform.Trip.Domain.Model.Aggregates;
using SafeVisionPlatform.Trip.Domain.Model.ValueObjects;
using Xunit;

namespace SafeVisionPlatform.Tests.UnitTests.Trip;

public class TripAggregateTests
{
    [Fact]
    public void Constructor_ShouldCreateTripWithInitiatedStatus()
    {
        // Arrange
        var driverId = 1;
        var vehicleId = 1;
        var dataPolicy = new TripDataPolicy(false, 5);

        // Act
        var trip = new TripAggregate(driverId, vehicleId, dataPolicy);

        // Assert
        Assert.NotNull(trip);
        Assert.Equal(driverId, trip.DriverId);
        Assert.Equal(vehicleId, trip.VehicleId);
        Assert.Equal(TripStatus.Initiated, trip.Status);
    }

    [Fact]
    public void StartTrip_WhenInitiated_ShouldChangeStatusToInProgress()
    {
        // Arrange
        var trip = CreateTestTrip();

        // Act
        trip.StartTrip();

        // Assert
        Assert.Equal(TripStatus.InProgress, trip.Status);
    }

    [Fact]
    public void StartTrip_WhenNotInitiated_ShouldThrowException()
    {
        // Arrange
        var trip = CreateTestTrip();
        trip.StartTrip();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => trip.StartTrip());
    }

    [Fact]
    public void EndTrip_WhenInProgress_ShouldChangeStatusToCompleted()
    {
        // Arrange
        var trip = CreateTestTrip();
        trip.StartTrip();

        // Act
        trip.EndTrip();

        // Assert
        Assert.Equal(TripStatus.Completed, trip.Status);
        Assert.NotNull(trip.Time.EndTime);
    }

    [Fact]
    public void EndTrip_WhenNotInProgress_ShouldThrowException()
    {
        // Arrange
        var trip = CreateTestTrip();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => trip.EndTrip());
    }

    [Fact]
    public void CancelTrip_WhenInitiated_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var trip = CreateTestTrip();

        // Act
        trip.CancelTrip();

        // Assert
        Assert.Equal(TripStatus.Cancelled, trip.Status);
    }

    [Fact]
    public void CancelTrip_WhenCompleted_ShouldThrowException()
    {
        // Arrange
        var trip = CreateTestTrip();
        trip.StartTrip();
        trip.EndTrip();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => trip.CancelTrip());
    }

    private TripAggregate CreateTestTrip()
    {
        var dataPolicy = new TripDataPolicy(false, 5);
        return new TripAggregate(1, 1, dataPolicy);
    }
}

