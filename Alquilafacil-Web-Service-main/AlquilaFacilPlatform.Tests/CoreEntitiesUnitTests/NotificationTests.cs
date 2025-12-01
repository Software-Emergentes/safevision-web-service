using AlquilaFacilPlatform.Notifications.Domain.Models.Aggregates;
using AlquilaFacilPlatform.Notifications.Domain.Models.Commands;

namespace AlquilaFacilPlatform.Tests.CoreEntitiesUnitTests;

public class NotificationTests
{
    [Fact]
    public void Notification_Constructor_WithCommand_ShouldInitializeProperties()
    {
        // Arrange
        var command = new CreateNotificationCommand("Reserva de espacio", "Un usuario ha reservado tu espacio.", 1);

        // Act
        var notification = new Notification(command);

        // Assert
        Assert.Equal(command.Title, notification.Title);
        Assert.Equal(command.Description, notification.Description);
        Assert.Equal(command.UserId, notification.UserId);
    }
}