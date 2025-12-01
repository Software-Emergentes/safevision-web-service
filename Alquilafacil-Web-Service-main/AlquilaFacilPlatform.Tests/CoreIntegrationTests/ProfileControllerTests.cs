using AlquilaFacilPlatform.Profiles.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Profiles.Domain.Model.Commands;
using AlquilaFacilPlatform.Profiles.Domain.Model.Queries;
using AlquilaFacilPlatform.Profiles.Domain.Services;
using AlquilaFacilPlatform.Profiles.Interfaces.REST;
using AlquilaFacilPlatform.Profiles.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AlquilaFacilPlatform.Tests.CoreIntegrationTests;

public class ProfileControllerTests
{
    [Fact]
    public async Task GetProfileByUserId_ReturnsOkWithProfile()
    {
        // Arrange
        var mockCommandService = new Mock<IProfileCommandService>();
        var mockQueryService = new Mock<IProfileQueryService>();

        var profile = new Profile(new CreateProfileCommand("Nombre", "ApePat", "ApeMat", "20/01/2025", "000000000", "999999999", 1));

        mockQueryService
            .Setup(s => s.Handle(It.Is<GetProfileByUserIdQuery>(q => q.UserId == 1)))
            .ReturnsAsync(profile);

        var controller = new ProfilesController(mockCommandService.Object, mockQueryService.Object);

        // Act
        var result = await controller.GetProfileByUserId(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProfile = Assert.IsAssignableFrom<ProfileResource>(okResult.Value);
        Assert.Equal("Nombre ApePat ApeMat", returnedProfile.FullName); // Ajusta según tu recurso
    }
    
    [Fact]
    public async Task GetProfileByUserId_ProfileNotFound_ReturnsNotFound()
    {
        // Arrange
        var mockCommandService = new Mock<IProfileCommandService>();
        var mockQueryService = new Mock<IProfileQueryService>();

        mockQueryService
            .Setup(s => s.Handle(It.Is<GetProfileByUserIdQuery>(q => q.UserId == 999)))
            .ReturnsAsync((Profile)null!);

        var controller = new ProfilesController(mockCommandService.Object, mockQueryService.Object);

        // Act
        var result = await controller.GetProfileByUserId(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task UpdateProfile_ValidRequest_ReturnsUpdatedProfile()
    {
        // Arrange
        var mockCommandService = new Mock<IProfileCommandService>();
        var mockQueryService = new Mock<IProfileQueryService>();

        var updateResource = new UpdateProfileResource("NewName", "NewLastName", "new@mail.com", "999999999", "00000000", "01/01/01", "ABC", "AAA"); 
        var updatedProfile = new Profile(new CreateProfileCommand("NewName", "NewLastName", "NewLastName", "01/01/01", "00000000", "999999999", 1));

        mockCommandService
            .Setup(s => s.Handle(It.IsAny<UpdateProfileCommand>()))
            .ReturnsAsync(updatedProfile);

        var controller = new ProfilesController(mockCommandService.Object, mockQueryService.Object);

        // Act
        var result = await controller.UpdateProfile(1, updateResource);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProfile = Assert.IsAssignableFrom<ProfileResource>(okResult.Value);
        Assert.Equal("NewName NewLastName NewLastName", returnedProfile.FullName);
    }
    
    [Fact]
    public async Task GetProfileBankAccountsByUserId_ReturnsOkWithAccounts()
    {
        // Arrange
        var mockCommandService = new Mock<IProfileCommandService>();
        var mockQueryService = new Mock<IProfileQueryService>();

        var profile = new Profile(new CreateProfileCommand("Nombre", "ApePat", "ApeMat", "20/01/2025", "000000000", "999999999", 1));
        profile.Id = 1;
        var updateProfileCommand = new UpdateProfileCommand("Nombre", "ApePat", "ApeMat", "20/01/2025", "000000000", "999999999", "ABC", "AAA", 1);
        profile.Update(updateProfileCommand);
        
        var bankAccounts = new List<string> { "ABC", "AAA" };
        
        mockQueryService
            .Setup(s => s.Handle(It.Is<GetProfileBankAccountsByUserIdQuery>(q => q.UserId == 1)))
            .ReturnsAsync(bankAccounts);
        
        var controller = new ProfilesController(mockCommandService.Object, mockQueryService.Object);

        // Act
        var result = await controller.GetProfileBankAccountsByUserId(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resultAccounts = Assert.IsAssignableFrom<IEnumerable<string>>(okResult.Value);
        Assert.Equal(2, resultAccounts.Count());
        Assert.Equal("ABC", resultAccounts.ElementAt(0));
        Assert.Equal("AAA", resultAccounts.ElementAt(1));
    }
}