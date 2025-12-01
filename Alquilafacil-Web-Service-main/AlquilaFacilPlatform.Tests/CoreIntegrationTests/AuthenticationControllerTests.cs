using AlquilaFacilPlatform.IAM.Domain.Model.Aggregates;
using AlquilaFacilPlatform.IAM.Domain.Model.Commands;
using AlquilaFacilPlatform.IAM.Domain.Services;
using AlquilaFacilPlatform.IAM.Interfaces.REST;
using AlquilaFacilPlatform.IAM.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AlquilaFacilPlatform.Tests.CoreIntegrationTests;

public class AuthenticationControllerTests
{
    [Fact]
    public async Task SignIn_WithValidCredentials_ReturnsOkWithAuthenticatedUserResource()
    {
        // Arrange
        var mockUserCommandService = new Mock<IUserCommandService>();

        var validSignInRequest = new SignInResource
        (
            "user@example.com",
            "StrongPassw0rd!"
        );

        var expectedUser = new User("johndoe", "hashed-password", "user@example.com");
        var expectedJwtToken = "mock-jwt-token";

        mockUserCommandService
            .Setup(service => service.Handle(It.Is<SignInCommand>(cmd =>
                cmd.Email == validSignInRequest.Email && cmd.Password == validSignInRequest.Password)))
            .ReturnsAsync((expectedUser, expectedJwtToken));

        var authenticationController = new AuthenticationController(mockUserCommandService.Object);

        // Act
        var actionResult = await authenticationController.SignIn(validSignInRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var actualResource = Assert.IsType<AuthenticatedUserResource>(okResult.Value);

        Assert.Equal(expectedUser.Username, actualResource.Username);
        Assert.Equal(expectedJwtToken, actualResource.Token);
    }
    [Fact]
    public async Task SignIn_WithInvalidCredentials_ThrowsException()
    {
        // Arrange
        var mockUserCommandService = new Mock<IUserCommandService>();
        
        var invalidCredentials = new SignInResource
        (
            "wrong@example.com",
            "WrongPassword123"
        );
        
        mockUserCommandService
            .Setup(service => service.Handle(It.IsAny<SignInCommand>()))
            .ThrowsAsync(new Exception("Invalid email or password"));
    
        var controller = new AuthenticationController(mockUserCommandService.Object);
        
        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.SignIn(invalidCredentials)
        );

        // Assert
        Assert.Equal("Invalid email or password", exception.Message);
    }
    [Fact]
    public async Task SignUp_WithValidData_ReturnsSuccessMessage()
    {
        // Arrange
        var mockUserCommandService = new Mock<IUserCommandService>();

        var validSignUpResource = new SignUpResource
        (
            "Username",
            "ValidPassword123!",
            "user@email.com",
            "NewUser",
            "FatherName",
            "MotherName",
            "1998-05-10",
            "1234567890",
            "987654321"
        );

        var user = new User
        (
            "Username",
            "hashed-password",
            "user@email.com"
        );

        mockUserCommandService
            .Setup(service => service.Handle(It.IsAny<SignUpCommand>()))
            .ReturnsAsync(user); // Simulate successful user creation

        var controller = new AuthenticationController(mockUserCommandService.Object);

        // Act
        var result = await controller.SignUp(validSignUpResource);
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
    [Fact]
    public async Task SignUp_WithInvalidPassword_ThrowsException()
    {
        // Arrange
        var mockUserCommandService = new Mock<IUserCommandService>();
        var invalidPasswordSignUpResource = new SignUpResource(
            "Username", "invalid", "user@email.com", "NewUser", "FatherName", "MotherName", 
            "1998-05-10", "1234567890", "987654321"
        );

        // Configura el mock para lanzar la excepción cuando se llame a Handle
        mockUserCommandService
            .Setup(service => service.Handle(It.IsAny<SignUpCommand>()))
            .ThrowsAsync(new Exception("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit and one special character"));

        var controller = new AuthenticationController(mockUserCommandService.Object);

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.SignUp(invalidPasswordSignUpResource)
        );

        // Assert
        Assert.Equal("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit and one special character", exception.Message);
    }
    [Fact]
    public async Task SignUp_WithInvalidEmail_ThrowsException()
    {
        // Arrange
        var mockUserCommandService = new Mock<IUserCommandService>();
        var invalidEmailSignUpResource = new SignUpResource(
            "Username", "ValidPassword123!", "invalid-email", "NewUser", "FatherName", "MotherName", 
            "1998-05-10", "1234567890", "987654321"
        );

        // Configura el mock para lanzar la excepción cuando se llame a Handle
        mockUserCommandService
            .Setup(service => service.Handle(It.IsAny<SignUpCommand>()))
            .ThrowsAsync(new Exception("Invalid email address"));

        var controller = new AuthenticationController(mockUserCommandService.Object);

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.SignUp(invalidEmailSignUpResource)
        );

        // Assert
        Assert.Equal("Invalid email address", exception.Message);
    }
    [Fact]
    public async Task SignUp_WithInvalidPhone_ThrowsException()
    {
        // Arrange
        var mockUserCommandService = new Mock<IUserCommandService>();
        var invalidPhoneSignUpResource = new SignUpResource(
            "Username", "ValidPassword123!", "user@email.com", "NewUser", "FatherName", "MotherName", 
            "1998-05-10", "12345", "987654321"
        );

        // Configura el mock para lanzar la excepción cuando se llame a Handle
        mockUserCommandService
            .Setup(service => service.Handle(It.IsAny<SignUpCommand>()))
            .ThrowsAsync(new Exception("Phone number must to be valid"));

        var controller = new AuthenticationController(mockUserCommandService.Object);

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.SignUp(invalidPhoneSignUpResource)
        );

        // Assert
        Assert.Equal("Phone number must to be valid", exception.Message);
    }
    [Fact]
    public async Task SignUp_WithUsernameAlreadyTaken_ThrowsException()
    {
        // Arrange
        var mockUserCommandService = new Mock<IUserCommandService>();
        var usernameTakenSignUpResource = new SignUpResource(
            "ExistingUsername", "ValidPassword123!", "user@email.com", "NewUser", "FatherName", "MotherName", 
            "1998-05-10", "1234567890", "987654321"
        );

        // Configura el mock para lanzar la excepción cuando el nombre de usuario ya existe
        mockUserCommandService
            .Setup(service => service.Handle(It.IsAny<SignUpCommand>()))
            .ThrowsAsync(new Exception("Username ExistingUsername is already taken"));

        var controller = new AuthenticationController(mockUserCommandService.Object);

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.SignUp(usernameTakenSignUpResource)
        );

        // Assert
        Assert.Equal("Username ExistingUsername is already taken", exception.Message);
    }
}