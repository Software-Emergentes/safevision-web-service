using SafeVisionPlatform.Driver.Domain.Model.ValueObjects;
using Xunit;

namespace SafeVisionPlatform.Tests.UnitTests.Driver;

public class LicenseNumberTests
{
    [Fact]
    public void Create_WithValidValue_ShouldCreateLicenseNumber()
    {
        // Arrange
        var value = "ABC123";

        // Act
        var licenseNumber = LicenseNumber.Create(value);

        // Assert
        Assert.NotNull(licenseNumber);
        Assert.Equal(value, licenseNumber.Value);
    }

    [Fact]
    public void Create_WithEmptyValue_ShouldThrowException()
    {
        // Arrange
        var value = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => LicenseNumber.Create(value));
    }

    [Fact]
    public void Create_WithTooShortValue_ShouldThrowException()
    {
        // Arrange
        var value = "AB";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => LicenseNumber.Create(value));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        var value = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => LicenseNumber.Create(value));
    }

    [Fact]
    public void Equals_WithSameValue_ShouldReturnTrue()
    {
        // Arrange
        var license1 = LicenseNumber.Create("ABC123");
        var license2 = LicenseNumber.Create("ABC123");

        // Act
        var result = license1.Equals(license2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_WithDifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var license1 = LicenseNumber.Create("ABC123");
        var license2 = LicenseNumber.Create("XYZ789");

        // Act
        var result = license1.Equals(license2);

        // Assert
        Assert.False(result);
    }
}

