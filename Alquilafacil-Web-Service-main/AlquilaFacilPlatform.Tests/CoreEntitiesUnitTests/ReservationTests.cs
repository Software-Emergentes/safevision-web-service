using AlquilaFacilPlatform.Booking.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Booking.Domain.Model.Commands;

namespace AlquilaFacilPlatform.Tests.CoreEntitiesUnitTests;

public class ReservationTests
{
    [Fact]
    public void Reservation_Constructor_WithCommand_ShouldInitializeProperties()
    {
        // Arrange
        var command = new CreateReservationCommand(DateTime.Now, DateTime.Now.AddDays(5), 1, 1, 100.0f, "voucherImageUrl");

        // Act
        var reservation = new Reservation(command);

        // Assert
        Assert.Equal(command.StartDate, reservation.StartDate);
        Assert.Equal(command.EndDate, reservation.EndDate);
        Assert.Equal(command.UserId, reservation.UserId);
        Assert.Equal(command.LocalId, reservation.LocalId);
        Assert.Equal(command.Price, reservation.Price);
        Assert.Equal(command.VoucherImageUrl, reservation.VoucherImageUrl);
    }
    
    [Fact]
    public void Reservation_UpdateDate_ShouldUpdateProperties()
    {
        // Arrange
        var command = new CreateReservationCommand(DateTime.Now, DateTime.Now.AddDays(5), 1, 1, 100.0f, "voucherImageUrl");
        var reservation = new Reservation(command);
        reservation.Id = 1;
        var updateCommand = new UpdateReservationDateCommand(reservation.Id, DateTime.Now.AddDays(1), DateTime.Now.AddDays(6));

        // Act
        reservation.UpdateDate(updateCommand);

        // Assert
        Assert.Equal(updateCommand.StartDate, reservation.StartDate);
        Assert.Equal(updateCommand.EndDate, reservation.EndDate);
    }
}