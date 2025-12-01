using AlquilaFacilPlatform.Locals.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Locals.Domain.Model.Commands;

namespace AlquilaFacilPlatform.Tests.CoreEntitiesUnitTests;

public class LocalTests
{
    [Fact]
    public void Local_Constructor_WithParameters_ShouldInitializeProperties()
    {
        // Arrange
        var district = "Miraflores";
        var street = "Malecon Cisneros";
        var localName = "Casa Urbana";
        var country = "Perú";
        var city = "Lima";
        var price = 125;
        var descriptionMessage = "Esta casa urbana combina diseño contemporáneo con comodidades de lujo. Disfruta de espacios amplios, luz natural y una ubicación privilegiada cerca de restaurantes, tiendas y parques.";
        var localCategoryId = 1;
        var userId = 1;
        var features = "Wi-Fi;Baños";
        var capacity = 0;

        // Act
        var local = new Local(
            localName,
            descriptionMessage,
            country,
            city,
            district,
            street,
            price,
            capacity,
            features,
            localCategoryId,
            userId
            );
        
        // Assert
        Assert.Equal(localName, local.LocalName);
        Assert.Equal(district, local.District.Value);
        Assert.Equal(street, local.Street.Value);
        Assert.Equal(price, local.PricePerHour.Value);
        Assert.Equal(country, local.Country.Value);
        Assert.Equal(city, local.City.Value);
        Assert.Equal(descriptionMessage, local.DescriptionMessage);
        Assert.Equal(localCategoryId, local.LocalCategoryId);
        Assert.Equal(userId, local.UserId);
        Assert.Equal(features, local.Features);
        Assert.Equal(capacity, local.Capacity);
    }

    [Fact]
    public void Local_Constructor_WithCommand_ShouldInitializeProperties()
    {
        // Arrange
        var createLocal = new CreateLocalCommand(
            "Local 1",
            "Descripcion 1",
            "Perú",
            "Lima",
            "San Isidro",
            "Calle Falsa 123",
            10,
            15,
            ["photo1.jpg", "photo2.jpg"],
            "Wi-Fi, Baños",
            1,
            1
            );
        
        // Act
        var local = new Local(createLocal);

        // Assert
        Assert.Equal(createLocal.LocalName, local.LocalName);
        Assert.Equal(createLocal.District, local.District.Value);
        Assert.Equal(createLocal.Street, local.Street.Value);
        Assert.Equal(createLocal.Price, local.PricePerHour.Value);
        Assert.Equal(createLocal.Country, local.Country.Value);
        Assert.Equal(createLocal.City, local.City.Value);
        Assert.Equal(createLocal.LocalCategoryId, local.LocalCategoryId);    
        Assert.Equal(createLocal.Features, local.Features);
        Assert.Equal(createLocal.Capacity, local.Capacity);
    }
}