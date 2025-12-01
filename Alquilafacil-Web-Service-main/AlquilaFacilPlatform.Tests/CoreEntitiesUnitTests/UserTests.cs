using AlquilaFacilPlatform.IAM.Domain.Model.Aggregates;

namespace AlquilaFacilPlatform.Tests.CoreEntitiesUnitTests;

public class UserTests
{
    [Fact]
    public void User_Constructor_WithParameters_ShouldInitializeProperties()
    {
        // Arrange
        const string username = "testUser";
        const string passwordHash = "#1234567890";
        const string email = "testuser@gmail.com";

        // Act
        var user = new User(username, passwordHash, email);

        // Assert
        Assert.Equal(username, user.Username);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public void UpdateUsername_ShouldChangeUsername()
    {
        // Arrange
        const string initialUsername = "initialUsername";
        const string newUsername = "newUsername";
        const string email = "email@gmail.com";
        var user = new User(initialUsername, "passwordHash", email);

        // Act
        user.UpdateUsername(newUsername);

        // Assert
        Assert.Equal(newUsername, user.Username);
    }

    [Fact]
    public void UpdatePasswordHash_ShouldChangePasswordHash()
    {
        // Arrange
        const string username = "username";
        const string initialPasswordHash = "initialHash";
        const string newPasswordHash = "newHash";
        const string email = "email@gmail.com";
        var user = new User(username, initialPasswordHash, email);

        // Act
        user.UpdatePasswordHash(newPasswordHash);

        // Assert
        Assert.Equal(newPasswordHash, user.PasswordHash);
    }
}