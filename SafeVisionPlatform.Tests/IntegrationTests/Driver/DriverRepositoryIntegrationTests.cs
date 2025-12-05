using Microsoft.EntityFrameworkCore;
using SafeVisionPlatform.Driver.Domain.Model.Aggregates;
using SafeVisionPlatform.Driver.Domain.Model.Entities;
using SafeVisionPlatform.Driver.Domain.Model.ValueObjects;
using SafeVisionPlatform.Driver.Infrastructure.Persistence.EFC.Repositories;
using SafeVisionPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Xunit;

namespace SafeVisionPlatform.Tests.IntegrationTests.Driver;

public class DriverRepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly DriverRepository _repository;

    public DriverRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AppDbContext(options);
        _repository = new DriverRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddDriverToDatabase()
    {
        // Arrange
        var driver = CreateTestDriver();

        // Act
        await _repository.AddAsync(driver);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(driver.Id);
        Assert.NotNull(result);
        Assert.Equal(driver.UserId, result.UserId);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDriver()
    {
        // Arrange
        var driver = CreateTestDriver();
        await _repository.AddAsync(driver);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(driver.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(driver.Id, result.Id);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnDriver()
    {
        // Arrange
        var driver = CreateTestDriver();
        await _repository.AddAsync(driver);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync(driver.UserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(driver.UserId, result.UserId);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateDriverInDatabase()
    {
        // Arrange
        var driver = CreateTestDriver();
        await _repository.AddAsync(driver);
        await _context.SaveChangesAsync();

        // Act
        driver.UpdateProfile("Jane", "Smith", "999999999", "jane@test.com", "New Address", 10);
        await _repository.UpdateAsync(driver);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(driver.Id);
        Assert.NotNull(result);
        Assert.Equal("Jane", result.Profile.FirstName);
    }

    private DriverAggregate CreateTestDriver()
    {
        var credentials = DriverCredentials.Create("testuser", "hashedpassword");
        var contact = ContactInformation.Create("123456789", "test@email.com", "Test Address");
        var profile = new DriverProfile("John", "Doe", "123456789", "john@test.com", "Address", 5);
        var license = new DriverLicense(LicenseNumber.Create("ABC123"), "A", DateTime.UtcNow.AddYears(-1), DateTime.UtcNow.AddYears(5));
        
        return new DriverAggregate(1, credentials, contact, profile, license);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

