using AlquilaFacilPlatform.IAM.Domain.Model.Aggregates;
using AlquilaFacilPlatform.IAM.Domain.Model.Commands;
using AlquilaFacilPlatform.IAM.Domain.Model.Queries;
using AlquilaFacilPlatform.IAM.Domain.Services;
using AlquilaFacilPlatform.IAM.Interfaces.REST;
using AlquilaFacilPlatform.IAM.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AlquilaFacilPlatform.Tests.CoreIntegrationTests;

public class UserControllerTests
{
    [Fact]
    public async Task GetUserById_ReturnsUserResource_WhenUserExists()
    {
        // Arrange
        var mockUserQueryService = new Mock<IUserQueryService>();
        var mockUserCommandService = new Mock<IUserCommandService>();
        var userId = 1;
        var user = new User("Username" , "HashedPassword", "user@email.com"); // Ejemplo de usuario
        var userResource = new UserResource(userId, "Username");

        mockUserQueryService
            .Setup(service => service.Handle(It.IsAny<GetUserByIdQuery>()))
            .ReturnsAsync(user);

        var controller = new UsersController(mockUserQueryService.Object, mockUserCommandService.Object);

        // Act
        var result = await controller.GetUserById(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUserResource = Assert.IsType<UserResource>(okResult.Value);
        Assert.Equal(userResource.Username, returnedUserResource.Username);
    }
    
    [Fact]
    public async Task GetAllUsers_ReturnsAllUsers_WhenUsersExist()
    {
        // Arrange
        var mockUserQueryService = new Mock<IUserQueryService>();
        var mockUserCommandService = new Mock<IUserCommandService>();
        
        var users = new List<User>
        {
            new User("User1", "HashedPassword", "user1@email.com"),
            new User("User2", "HashedPassword", "user2@email.com")
        };

        mockUserQueryService
            .Setup(service => service.Handle(It.IsAny<GetAllUsersQuery>()))
            .ReturnsAsync(users);

        var controller = new UsersController(mockUserQueryService.Object, mockUserCommandService.Object);

        // Act
        var result = await controller.GetAllUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUserResources = Assert.IsAssignableFrom<IEnumerable<UserResource>>(okResult.Value);
        Assert.Equal(users.Count, returnedUserResources.Count());
    }
    
    [Fact]
    public async Task GetUsernameById_ReturnsUsername_WhenUserExists()
    {
        // Arrange
        var mockUserQueryService = new Mock<IUserQueryService>();
        var mockUserCommandService = new Mock<IUserCommandService>();
        var expectedUsername = "testuser";

        mockUserQueryService
            .Setup(service => service.Handle(It.Is<GetUsernameByIdQuery>(q => q.UserId == 1)))
            .ReturnsAsync(expectedUsername);

        var controller = new UsersController(mockUserQueryService.Object, mockUserCommandService.Object);

        // Act
        var result = await controller.GetUsernameById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualUsername = Assert.IsType<string>(okResult.Value);
        Assert.Equal(expectedUsername, actualUsername);
    }
    
    [Fact]
    public async Task UpdateUser_UpdatesAndReturnsUser_WhenValidRequest()
    {
        // Arrange
        var mockUserQueryService = new Mock<IUserQueryService>();
        var mockUserCommandService = new Mock<IUserCommandService>();
        var updateUsernameResource = new UpdateUsernameResource("newUsername");
        var updatedUser = new User("newUsername", "hashedPassword", "test@email.com") { Id = 1 };

        mockUserCommandService
            .Setup(service => service.Handle(It.IsAny<UpdateUsernameCommand>()))
            .ReturnsAsync(updatedUser);

        var controller = new UsersController(mockUserQueryService.Object, mockUserCommandService.Object);

        // Act
        var result = await controller.UpdateUser(1, updateUsernameResource);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var userResource = Assert.IsAssignableFrom<UserResource>(okResult.Value);
        Assert.Equal("newUsername", userResource.Username);
    }
}