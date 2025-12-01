using AlquilaFacilPlatform.Locals.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Locals.Domain.Model.Commands;
using AlquilaFacilPlatform.Locals.Domain.Model.Queries;
using AlquilaFacilPlatform.Locals.Domain.Services;
using AlquilaFacilPlatform.Locals.Interfaces.REST;
using AlquilaFacilPlatform.Locals.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AlquilaFacilPlatform.Tests.CoreIntegrationTests;

public class LocalControllerTests
{
    [Fact]
    public async Task CreateLocal_ReturnsCreatedLocal_WhenSuccess()
    {
        // Arrange
        var mockCommandService = new Mock<ILocalCommandService>();
        var mockQueryService = new Mock<ILocalQueryService>();

        var resource = new CreateLocalResource(
            District: "Miraflores",
            Street: "Av. Larco 123",
            LocalName: "Sala Ejecutiva Premium",
            Country: "Perú",
            City: "Lima",
            Price: 250,
            PhotoUrls: ["https://example.com/photo.jpg"],
            DescriptionMessage: "Sala con proyector y aire acondicionado",
            LocalCategoryId: 2,
            UserId: 10,
            Features: "WiFi, Proyector, Aire Acondicionado",
            Capacity: 30
        );

        var expectedLocal = new Local(
            resource.LocalName,
            resource.DescriptionMessage,
            resource.Country,
            resource.City,
            resource.District,
            resource.Street,
            resource.Price,
            resource.Capacity,
            resource.Features,
            resource.LocalCategoryId,
            resource.UserId
        )
        {
            Id = 456 // Simulación de que ya fue creado con ID
        };

        mockCommandService
            .Setup(s => s.Handle(It.IsAny<CreateLocalCommand>()))
            .ReturnsAsync(expectedLocal);

        var controller = new LocalsController(mockCommandService.Object, mockQueryService.Object);

        // Act
        var result = await controller.CreateLocal(resource);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var localResource = Assert.IsType<LocalResource>(createdResult.Value);

        Assert.Equal(456, localResource.Id);
        Assert.Equal("Sala Ejecutiva Premium", localResource.LocalName);
        Assert.Equal(250, localResource.Price);
        Assert.Equal(30, localResource.Capacity);
        Assert.Equal(10, localResource.UserId);
    }
    
    [Fact]
    public async Task GetAllLocals_ReturnsAllLocals()
    {
        // Arrange
        var mockCommandService = new Mock<ILocalCommandService>();
        var mockQueryService = new Mock<ILocalQueryService>();

        var locals = new List<Local>
        {
            new("Sala A", "Desc A", "Perú", "Lima", "Miraflores", "Av. Larco", 200, 10, "WiFi", 1, 1),
            new("Sala B", "Desc B", "Perú", "Lima", "San Isidro", "Calle Uno", 300, 20, "WiFi, Proyector", 2, 2)
        };

        mockQueryService
            .Setup(s => s.Handle(It.IsAny<GetAllLocalsQuery>()))
            .ReturnsAsync(locals);

        var controller = new LocalsController(mockCommandService.Object, mockQueryService.Object);

        // Act
        var result = await controller.GetAllLocals();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedLocals = Assert.IsAssignableFrom<IEnumerable<LocalResource>>(okResult.Value);
        Assert.Equal(2, returnedLocals.Count());
    }

    [Fact]
    public async Task GetLocalById_ReturnsLocal_WhenExists()
    {
        // Arrange
        var mockCommandService = new Mock<ILocalCommandService>();
        var mockQueryService = new Mock<ILocalQueryService>();

        var local = new Local("Sala A", "Desc A", "Perú", "Lima", "Miraflores", "Av. Larco", 200, 10, "WiFi", 1, 1)
        {
            Id = 10
        };

        mockQueryService
            .Setup(s => s.Handle(It.Is<GetLocalByIdQuery>(q => q.LocalId == 10)))
            .ReturnsAsync(local);

        var controller = new LocalsController(mockCommandService.Object, mockQueryService.Object);

        // Act
        var result = await controller.GetLocalById(10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var localResource = Assert.IsType<LocalResource>(okResult.Value);
        Assert.Equal(10, localResource.Id);
    }

    [Fact]
    public async Task GetLocalById_ReturnsNotFound_WhenNotExists()
    {
        // Arrange
        var mockCommandService = new Mock<ILocalCommandService>();
        var mockQueryService = new Mock<ILocalQueryService>();

        mockQueryService
            .Setup(s => s.Handle(It.IsAny<GetLocalByIdQuery>()))
            .ReturnsAsync((Local?)null);

        var controller = new LocalsController(mockCommandService.Object, mockQueryService.Object);

        // Act
        var result = await controller.GetLocalById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateLocal_ReturnsUpdatedLocal_WhenSuccess()
    {
        // Arrange
        var mockCommandService = new Mock<ILocalCommandService>();
        var mockQueryService = new Mock<ILocalQueryService>();

        var updateResource = new UpdateLocalResource(
            "Sala Actualizada",
            "Actualizada",
            "Perú",
            "Lima",
            "San Isidro",
            "Av. Actualizada",
            10,
            10,
            "WiFi, Proyector",
            1,
            1
        );

        var updatedLocal = new Local(
            updateResource.LocalName,
            updateResource.DescriptionMessage,
            updateResource.Country,
            updateResource.City,
            updateResource.District,
            updateResource.Street,
            updateResource.Price,
            updateResource.Capacity,
            updateResource.Features,
            updateResource.LocalCategoryId,
            updateResource.UserId
            )
        {
            Id = 789
        };

        mockCommandService
            .Setup(s => s.Handle(It.IsAny<UpdateLocalCommand>()))
            .ReturnsAsync(updatedLocal);

        var controller = new LocalsController(mockCommandService.Object, mockQueryService.Object);

        // Act
        var result = await controller.UpdateLocal(789, updateResource);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resource = Assert.IsType<LocalResource>(okResult.Value);
        Assert.Equal(789, resource.Id);
        Assert.Equal("Sala Actualizada", resource.LocalName);
    }

    [Fact]
    public async Task GetUserLocals_ReturnsLocals_WhenExists()
    {
        // Arrange
        var mockCommandService = new Mock<ILocalCommandService>();
        var mockQueryService = new Mock<ILocalQueryService>();

        var userId = 5;
        var locals = new List<Local>
        {
            new("Sala A", "Desc A", "Perú", "Lima", "Miraflores", "Av. Larco", 200, 10, "WiFi", 1, 1),
            new("Sala B", "Desc B", "Perú", "Lima", "San Isidro", "Calle Uno", 300, 20, "WiFi, Proyector", 2, userId)
        };

        mockQueryService
            .Setup(s => s.Handle(It.Is<GetLocalsByUserIdQuery>(q => q.UserId == userId)))
            .ReturnsAsync(locals);

        var controller = new LocalsController(mockCommandService.Object, mockQueryService.Object);

        // Act
        var result = await controller.GetUserLocals(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var localResources = Assert.IsAssignableFrom<IEnumerable<LocalResource>>(okResult.Value);
        Assert.Equal(2, localResources.Count());
    }

    [Fact]
    public async Task SearchByCategoryIdAndCapacityRange_ReturnsMatchingLocals()
    {
        // Arrange
        var mockCommandService = new Mock<ILocalCommandService>();
        var mockQueryService = new Mock<ILocalQueryService>();

        int categoryId = 2, min = 10, max = 50;

        var locals = new List<Local>
        {
            new("Sala A", "Desc A", "Perú", "Lima", "Miraflores", "Av. Larco", 200, 20, "WiFi", categoryId, 1)
        };

        mockQueryService
            .Setup(s => s.Handle(It.Is<GetLocalsByCategoryIdAndCapacityRangeQuery>(q =>
                q.LocalCategoryId == categoryId && q.MinCapacity == min && q.MaxCapacity == max)))
            .ReturnsAsync(locals);

        var controller = new LocalsController(mockCommandService.Object, mockQueryService.Object);

        // Act
        var result = await controller.SearchByCategoryIdAndCapacityRange(categoryId, min, max);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var localResources = Assert.IsAssignableFrom<IEnumerable<LocalResource>>(okResult.Value);
        Assert.Single(localResources);
    }

    [Fact]
    public async Task GetAllDistricts_ReturnsDistricts()
    {
        // Arrange
        var mockCommandService = new Mock<ILocalCommandService>();
        var mockQueryService = new Mock<ILocalQueryService>();

        var expectedDistricts = new HashSet<string>
        {
            "Miraflores",
            "San Isidro",
            "Barranco"
        };
        
        
        mockQueryService.Setup(s => s.Handle(It.IsAny<GetAllLocalDistrictsQuery>())).ReturnsAsync(expectedDistricts);

        var controller = new LocalsController(mockCommandService.Object, mockQueryService.Object);

        // Act
        var result = await controller.GetAllDistricts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result); 
        var returnedDistricts = Assert.IsType<HashSet<string>>(okResult.Value);
        Assert.Equal(expectedDistricts.Count, returnedDistricts.Count);
    }
}