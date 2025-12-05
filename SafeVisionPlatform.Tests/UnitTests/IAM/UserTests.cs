using SafeVisionPlatform.IAM.Domain.Model.Aggregates;
using SafeVisionPlatform.IAM.Domain.Model.ValueObjects;
using Xunit;

namespace SafeVisionPlatform.Tests.UnitTests.IAM;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldCreateUserWithDefaultRole()
    {
        // Arrange
        var username = "testuser";
        var passwordHash = "hashedpassword";
        var email = "test@email.com";

        // Act
        var user = new User(username, passwordHash, email);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(username, user.Username);
        Assert.Equal(email, user.Email);
        Assert.Equal((int)EUserRoles.User, user.RoleId);
    }

    [Fact]
    public void UpdateUsername_ShouldChangeUsername()
    {
        // Arrange
        var user = new User("olduser", "hash", "test@email.com");
        var newUsername = "newuser";

        // Act
        user.UpdateUsername(newUsername);

        // Assert
        Assert.Equal(newUsername, user.Username);
    }

    [Fact]
    public void UpdatePasswordHash_ShouldChangePasswordHash()
    {
        // Arrange
        var user = new User("testuser", "oldhash", "test@email.com");
        var newHash = "newhash";

        // Act
        user.UpdatePasswordHash(newHash);

        // Assert
        Assert.Equal(newHash, user.PasswordHash);
    }

    [Fact]
    public void UpgradeToAdmin_ShouldChangeRoleToAdmin()
    {
        // Arrange
        var user = new User("testuser", "hash", "test@email.com");

        // Act
        user.UpgradeToAdmin();

        // Assert
        Assert.Equal((int)EUserRoles.Admin, user.RoleId);
    }

    [Fact]
    public void UpdateMfaKey_ShouldSetMfaKey()
    {
        // Arrange
        var user = new User("testuser", "hash", "test@email.com");
        var mfaKey = "JBSWY3DPEHPK3PXP";

        // Act
        user.UpdateMfaKey(mfaKey);

        // Assert
        Assert.Equal(mfaKey, user.MfaKey);
    }
}

