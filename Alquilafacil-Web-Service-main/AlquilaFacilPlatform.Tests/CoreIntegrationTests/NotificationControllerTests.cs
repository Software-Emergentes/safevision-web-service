
using AlquilaFacilPlatform.Notifications.Domain.Models.Aggregates;
using AlquilaFacilPlatform.Notifications.Domain.Models.Commands;
using AlquilaFacilPlatform.Notifications.Domain.Models.Queries;
using AlquilaFacilPlatform.Notifications.Domain.Services;
using AlquilaFacilPlatform.Notifications.Interfaces.REST;
using AlquilaFacilPlatform.Notifications.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AlquilaFacilPlatform.Tests.CoreIntegrationTests;

public class NotificationControllerTests
{
    [Fact]
    public async Task GetNotificationsByUserId_ReturnsOnlyNotificationsForGivenUser()
    {
        // Arrange
        var mockCommandService = new Mock<INotificationCommandService>();
        var mockQueryService = new Mock<INotificationQueryService>();
        
        var createCommand1 = new CreateNotificationCommand("Titulo 1", "Descripcion 1", 1);
        var createCommand2 = new CreateNotificationCommand("Titulo 2", "Descripcion 2", 2);
        var createCommand3 = new CreateNotificationCommand("Titulo 3", "Descripcion 3", 2);

        var allNotifications = new List<Notification>
        {
               new Notification(createCommand1),
               new Notification(createCommand2),
               new Notification(createCommand3)
        };

        // Solo retornar las del usuario con ID 2
        mockQueryService
            .Setup(s => s.Handle(It.Is<GetNotificationsByUserIdQuery>(q => q.UserId == 2)))
            .ReturnsAsync(allNotifications.Where(n => n.UserId == 2).ToList());

        var controller = new NotificationController(mockCommandService.Object, mockQueryService.Object);

        // Act
        var result = await controller.GetNotificationsByUserId(2);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var notifications = Assert.IsAssignableFrom<IEnumerable<NotificationResource>>(okResult.Value);

        Assert.Equal(2, notifications.Count());
        Assert.All(notifications, n => Assert.Equal(2, n.UserId));
    }
    
    [Fact]
    public async Task DeleteNotification_ReturnsOkWithDeletedNotification()
    {
        // Arrange
        var mockCommandService = new Mock<INotificationCommandService>();
        var mockQueryService = new Mock<INotificationQueryService>();

        var deletedNotification = new Notification(new CreateNotificationCommand("Titulo 1", "Descripcion 1", 1));
        deletedNotification.Id = 1;

        mockCommandService
            .Setup(s => s.Handle(It.Is<DeleteNotificationCommand>(c => c.Id == 1)))
            .ReturnsAsync(deletedNotification);

        var controller = new NotificationController(mockCommandService.Object, mockQueryService.Object);

        // Act
        var result = await controller.DeleteNotification(1);

        // Assert
        var okResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(deletedNotification, okResult.Value);
    }
}