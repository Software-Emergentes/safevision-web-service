using AlquilaFacilPlatform.Subscriptions.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Subscriptions.Domain.Model.Commands;
using AlquilaFacilPlatform.Subscriptions.Domain.Model.ValueObjects;

namespace AlquilaFacilPlatform.Tests.CoreEntitiesUnitTests;

public class SubscriptionTests
{
    [Fact]
    public void Subscription_Constructor_WithCommand_ShouldInitializeProperties()
    {
        // Arrange
        var command = new CreateSubscriptionCommand(1, 1, "http://example.com/voucher.jpg");

        // Act
        var subscription = new Subscription(command);

        // Assert
        Assert.Equal(command.UserId, subscription.UserId);
        Assert.Equal(command.PlanId, subscription.PlanId);
        Assert.Equal(command.VoucherImageUrl, subscription.VoucherImageUrl);
        Assert.Equal((int)ESubscriptionStatus.Pending, subscription.SubscriptionStatusId);
    }

    [Fact]
    public void ActiveSubscriptionStatus_ShouldSetSubscriptionStatusToActive()
    {
        // Arrange
        var command = new CreateSubscriptionCommand(1, 1, "http://example.com/voucher.jpg");
        var subscription = new Subscription(command);

        // Act
        subscription.ActiveSubscriptionStatus();

        // Assert
        Assert.Equal((int)ESubscriptionStatus.Active, subscription.SubscriptionStatusId);
    }    
}