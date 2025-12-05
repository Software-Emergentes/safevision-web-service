using SafeVisionPlatform.Driver.Domain.Model.Aggregates;
using SafeVisionPlatform.Driver.Domain.Model.Entities;
using SafeVisionPlatform.Driver.Domain.Model.ValueObjects;
using Xunit;

namespace SafeVisionPlatform.Tests.UnitTests.Driver;

public class DriverAggregateTests
{
    [Fact]
    public void Constructor_ShouldCreateDriverWithInactiveStatus()
    {
        // Arrange
        var userId = 1;
        var credentials = DriverCredentials.Create("testuser", "hashedpassword");
        var contact = ContactInformation.Create("123456789", "test@email.com", "Test Address");
        var profile = new DriverProfile("John", "Doe", "123456789", "john@test.com", "Address", 5);
        var license = new DriverLicense(LicenseNumber.Create("ABC123"), "A", DateTime.UtcNow.AddYears(-1), DateTime.UtcNow.AddYears(5));

        // Act
        var driver = new DriverAggregate(userId, credentials, contact, profile, license);

        // Assert
        Assert.NotNull(driver);
        Assert.Equal(userId, driver.UserId);
        Assert.False(driver.Status.IsActive());
    }

    [Fact]
    public void Activate_WithValidLicense_ShouldSetStatusToActive()
    {
        // Arrange
        var driver = CreateTestDriver();
        driver.ValidateLicense(); // Validar la licencia antes de activar

        // Act
        driver.Activate();

        // Assert
        Assert.True(driver.Status.IsActive());
    }

    [Fact]
    public void Deactivate_ShouldSetStatusToInactive()
    {
        // Arrange
        var driver = CreateTestDriver();
        driver.ValidateLicense(); // Validar la licencia antes de activar
        driver.Activate();

        // Act
        driver.Deactivate();

        // Assert
        Assert.False(driver.Status.IsActive());
    }

    [Fact]
    public void SetInTrip_WhenActive_ShouldChangeStatusToInTrip()
    {
        // Arrange
        var driver = CreateTestDriver();
        driver.ValidateLicense(); // Validar la licencia antes de activar
        driver.Activate();

        // Act
        driver.SetInTrip();

        // Assert
        Assert.True(driver.Status.IsInTrip());
    }

    [Fact]
    public void SetInTrip_WhenInactive_ShouldThrowException()
    {
        // Arrange
        var driver = CreateTestDriver();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => driver.SetInTrip());
    }

    [Fact]
    public void EndTrip_WhenInTrip_ShouldReturnToActive()
    {
        // Arrange
        var driver = CreateTestDriver();
        driver.ValidateLicense(); // Validar la licencia antes de activar
        driver.Activate();
        driver.SetInTrip();

        // Act
        driver.EndTrip();

        // Assert
        Assert.True(driver.Status.IsActive());
        Assert.False(driver.Status.IsInTrip());
    }

    [Fact]
    public void UpdateProfile_ShouldUpdateProfileData()
    {
        // Arrange
        var driver = CreateTestDriver();
        var newFirstName = "Jane";
        var newLastName = "Smith";

        // Act
        driver.UpdateProfile(newFirstName, newLastName, "987654321", "jane@test.com", "New Address", 10);

        // Assert
        Assert.Equal(newFirstName, driver.Profile.FirstName);
        Assert.Equal(newLastName, driver.Profile.LastName);
    }

    [Fact]
    public void CanBeAssignedToTrip_WhenActive_ShouldReturnTrue()
    {
        // Arrange
        var driver = CreateTestDriver();
        driver.ValidateLicense(); // Validar la licencia antes de activar
        driver.Activate();

        // Act
        var result = driver.CanBeAssignedToTrip();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsAvailable_WhenActive_ShouldReturnTrue()
    {
        // Arrange
        var driver = CreateTestDriver();
        driver.ValidateLicense(); // Validar la licencia antes de activar
        driver.Activate();

        // Act
        var result = driver.IsAvailable();

        // Assert
        Assert.True(result);
    }

    private DriverAggregate CreateTestDriver()
    {
        var credentials = DriverCredentials.Create("testuser", "hashedpassword");
        var contact = ContactInformation.Create("123456789", "test@email.com", "Test Address");
        var profile = new DriverProfile("John", "Doe", "123456789", "john@test.com", "Address", 5);
        var license = new DriverLicense(LicenseNumber.Create("ABC123"), "A", DateTime.UtcNow.AddYears(-1), DateTime.UtcNow.AddYears(5));
        
        return new DriverAggregate(1, credentials, contact, profile, license);
    }
}
