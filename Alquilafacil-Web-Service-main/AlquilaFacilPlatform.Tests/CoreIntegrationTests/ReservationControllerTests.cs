using AlquilaFacilPlatform.Booking.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Booking.Domain.Model.Commands;
using AlquilaFacilPlatform.Booking.Domain.Model.Queries;
using AlquilaFacilPlatform.Booking.Domain.Services;
using AlquilaFacilPlatform.Booking.Interfaces.REST;
using AlquilaFacilPlatform.Booking.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AlquilaFacilPlatform.Tests.CoreIntegrationTests;

public class ReservationControllerTests
{
    [Fact]
    public async Task CreateReservation_ReturnsCreatedResult()
    {
        var mockCommandService = new Mock<IReservationCommandService>();
        var mockQueryService = new Mock<IReservationQueryService>();

        var resource = new CreateReservationResource
        (
            DateTime.UtcNow.AddDays(2),
            DateTime.UtcNow.AddDays(3),
            1,
            2,
            10.5f,
            "voucherCode"
        );

        var command = new CreateReservationCommand(
            resource.StartDate,
            resource.EndDate,
            resource.UserId,
            resource.LocalId,
            resource.price,
            resource.voucherImageUrl
        );
        
        var createdReservation = new Reservation(command);

        mockCommandService
            .Setup(s => s.Handle(It.IsAny<CreateReservationCommand>()))
            .ReturnsAsync(createdReservation);

        var controller = new ReservationController(mockCommandService.Object, mockQueryService.Object);

        var result = await controller.CreateReservationAsync(resource);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(201, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task UpdateReservation_ReturnsOkResult()
    {
        var mockCommandService = new Mock<IReservationCommandService>();
        var mockQueryService = new Mock<IReservationQueryService>();

        var resource = new UpdateReservationResource
        (
            DateTime.UtcNow.AddDays(2),
            DateTime.UtcNow.AddDays(3),
            1,
            2
        );
        
        var createCommand = new CreateReservationCommand(
            resource.StartDate,
            resource.EndDate,
            resource.UserId,
            resource.LocalId,
            10.5f,
            "voucherCode"
        );

        var updatedReservation = new Reservation(createCommand);

        mockCommandService
            .Setup(s => s.Handle(It.IsAny<UpdateReservationDateCommand>()))
            .ReturnsAsync(updatedReservation);

        var controller = new ReservationController(mockCommandService.Object, mockQueryService.Object);

        var result = await controller.UpdateReservationAsync(1, resource);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
    
    [Fact]
    public async Task DeleteReservation_ReturnsOk()
    {
        var mockCommandService = new Mock<IReservationCommandService>();
        var mockQueryService = new Mock<IReservationQueryService>();
        
        
        var deletedReservation = new Reservation();
        deletedReservation.Id = 1;

        mockCommandService
            .Setup(s => s.Handle(It.IsAny<DeleteReservationCommand>()))
            .ReturnsAsync(deletedReservation);

        var controller = new ReservationController(mockCommandService.Object, mockQueryService.Object);

        var result = await controller.DeleteReservationAsync(1);

        var okResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal("Reservation deleted", okResult.Value);
    }

    [Fact]
    public async Task GetReservationsByUserId_ReturnsOkWithReservations()
    {
        var mockCommandService = new Mock<IReservationCommandService>();
        var mockQueryService = new Mock<IReservationQueryService>();
        
        var createCommand1 = new CreateReservationCommand(
            
            DateTime.UtcNow.AddDays(2),
            DateTime.UtcNow.AddDays(3),
            1,
            2,
            10.5f,
            "voucherCode"
        );
        var createCommand2 = new CreateReservationCommand(
            DateTime.UtcNow.AddDays(4),
            DateTime.UtcNow.AddDays(5),
            2,
            3,
            20.0f,
            "voucherCode"
        );
        var createCommand3 = new CreateReservationCommand(
            DateTime.UtcNow.AddDays(6),
            DateTime.UtcNow.AddDays(7),
            1,
            4,
            30.0f,
            "voucherCode"
        );

        var allReservations = new List<Reservation>
        {
           new Reservation(createCommand1),
           new Reservation(createCommand2),
           new Reservation(createCommand3)
        };

        mockQueryService
            .Setup(s => s.GetReservationsByUserIdAsync(It.IsAny<GetReservationsByUserId>()))
            .ReturnsAsync((GetReservationsByUserId query) =>
                allReservations.Where(r => r.UserId == query.UserId).ToList());

        var controller = new ReservationController(mockCommandService.Object, mockQueryService.Object);

        var result = await controller.GetReservationsByUserIdAsync(2);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        var reservationsResult = Assert.IsAssignableFrom<IEnumerable<ReservationResource>>(okResult.Value);
        Assert.Single(reservationsResult);
        Assert.All(reservationsResult, r => Assert.Equal(2, r.UserId));
    }
    
    [Fact]
    public async Task GetReservationUserDetails_ReturnsOnlyReservationsForGivenUser()
    {
        // Arrange
        var mockCommandService = new Mock<IReservationCommandService>();
        var mockQueryService = new Mock<IReservationQueryService>();

        var allReservations = new List<LocalReservationResource>
        {
            new(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 2, 10, true, "https://example.com/voucher1.png"),
            new(2, DateTime.UtcNow, DateTime.UtcNow.AddDays(2), 3, 11, false, "https://example.com/voucher2.png"),
            new(3, DateTime.UtcNow, DateTime.UtcNow.AddDays(3), 2, 12, true, "https://example.com/voucher3.png")
        };

        // Solo debería retornar las reservas del UserId 2
        mockQueryService
            .Setup(s => s.GetReservationsByOwnerIdAsync(It.Is<GetReservationsByOwnerIdQuery>(q => q.OwnerId == 2)))
            .ReturnsAsync(allReservations.Where(r => r.UserId == 2));

        var controller = new ReservationController(mockCommandService.Object, mockQueryService.Object);

        // Act
        var result = await controller.GetReservationUserDetailsAsync(2);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var details = Assert.IsType<ReservationDetailsResource>(okResult.Value);

        Assert.Equal(2, details.Reservations.Count());
        Assert.All(details.Reservations, r => Assert.Equal(2, r.UserId));
    }

}