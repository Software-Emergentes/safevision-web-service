using SafeVisionPlatform.FatigueMonitoring.Domain.Model.Entities;
using SafeVisionPlatform.FatigueMonitoring.Domain.Model.ValueObjects;
using Xunit;

namespace SafeVisionPlatform.Tests.UnitTests.FatigueMonitoring;

public class DrowsinessEventTests
{
    [Fact]
    public void Constructor_ShouldCreateDrowsinessEventWithCorrectProperties()
    {
        // Arrange
        var driverId = 1;
        var tripId = 1;
        var sessionId = 1;
        var eventType = DrowsinessEventType.Blink;
        var sensorData = new SensorData(0.5, 0.8, 0.3, 0.0, 0.2);
        var severity = new SeverityScore(0.7);

        // Act
        var drowsinessEvent = new DrowsinessEvent(driverId, tripId, sessionId, eventType, sensorData, severity);

        // Assert
        Assert.NotNull(drowsinessEvent);
        Assert.Equal(driverId, drowsinessEvent.DriverId);
        Assert.Equal(tripId, drowsinessEvent.TripId);
        Assert.Equal(sessionId, drowsinessEvent.MonitoringSessionId);
        Assert.Equal(eventType, drowsinessEvent.EventType);
    }

    [Fact]
    public void Constructor_ShouldSetProcessedToFalse()
    {
        // Arrange
        var sensorData = new SensorData(0.5, 0.8, 0.3, 0.0, 0.2);
        var severity = new SeverityScore(0.7);

        // Act
        var drowsinessEvent = new DrowsinessEvent(1, 1, 1, DrowsinessEventType.Blink, sensorData, severity);

        // Assert
        Assert.False(drowsinessEvent.Processed);
    }
}
