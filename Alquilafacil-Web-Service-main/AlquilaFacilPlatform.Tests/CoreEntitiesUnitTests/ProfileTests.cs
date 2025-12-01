using AlquilaFacilPlatform.Profiles.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Profiles.Domain.Model.Commands;
using AlquilaFacilPlatform.Profiles.Domain.Model.Queries;
using AlquilaFacilPlatform.Profiles.Domain.Services;
using Moq;

namespace AlquilaFacilPlatform.Tests.CoreEntitiesUnitTests;

public class ProfileTests
{ 
    [Fact]
    public void Profile_Constructor_WithParameters_ShouldInitializeProperties()
    { 
        // Arrange
        var command = new CreateProfileCommand("Jane", "Johnson", "Doe", "1995-02-15", "87654321", "987654321",1);
        
        // Act
        var profile = new Profile(command);
        
        // Assert
        Assert.Equal(command.Name, profile.Name.Name);
        Assert.Equal(command.FatherName, profile.Name.FatherName);
        Assert.Equal(command.MotherName, profile.Name.MotherName);
        Assert.Equal(command.DateOfBirth, profile.Birth.BirthDate);
        Assert.Equal(command.DocumentNumber, profile.DocumentN.NumberDocument);
        Assert.Equal(command.Phone, profile.PhoneN.PhoneNumber);
    }
    
    [Fact]
    public void Profile_Constructor_WithCommand_ShouldInitializeProperties()
    {
        // Arrange
        var command = new CreateProfileCommand("Jane", "Johnson", "Doe", "1995-02-15", "87654321", "987654321",1);
        
        // Act
        var profile = new Profile(command);
        
        //Assert
        Assert.Equal(command.Name, profile.Name.Name);
        Assert.Equal(command.FatherName, profile.Name.FatherName);
        Assert.Equal(command.MotherName, profile.Name.MotherName);
        Assert.Equal(command.DateOfBirth, profile.Birth.BirthDate);
        Assert.Equal(command.DocumentNumber, profile.DocumentN.NumberDocument);
        Assert.Equal(command.Phone, profile.PhoneN.PhoneNumber);
    }

    [Fact]
    public void Profile_Update_ShouldUpdateProperties()
    {
        // Arrange
        var command = new CreateProfileCommand("Jane", "Johnson", "Doe", "1995-02-15", "87654321", "987654321",1);
        var profile = new Profile(command);
        var updateCommand = new UpdateProfileCommand("Jane", "Johnson", "Doe", "1995-02-15", "87654321", "987654321", "123-45678901-0-12", "00212345678901234567", 1);
        
        // Act
        profile.Update(updateCommand);
        
        //Assert
        Assert.Equal(updateCommand.Name, profile.Name.Name);
        Assert.Equal(updateCommand.FatherName, profile.Name.FatherName);
        Assert.Equal(updateCommand.MotherName, profile.Name.MotherName);
        Assert.Equal(updateCommand.DateOfBirth, profile.Birth.BirthDate);
        Assert.Equal(updateCommand.DocumentNumber, profile.DocumentN.NumberDocument);
        Assert.Equal(updateCommand.Phone, profile.PhoneN.PhoneNumber);
    }
}