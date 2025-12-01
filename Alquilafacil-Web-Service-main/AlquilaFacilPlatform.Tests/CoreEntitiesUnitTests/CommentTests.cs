using AlquilaFacilPlatform.Locals.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Locals.Domain.Model.Commands;

namespace AlquilaFacilPlatform.Tests.CoreEntitiesUnitTests;

public class CommentTests
{
    [Fact]
    public void Comment_Constructor_WithParameters_ShouldInitializeProperties()
    {
        // Arrange
        var userId = 1;
        var localId = 1;
        var text = "Excelente lugar";
        var rating = 5;

        // Act
        var comment = new Comment(userId, localId, text, rating);

        // Assert
        Assert.Equal(userId, comment.UserId);
        Assert.Equal(localId, comment.LocalId);
        Assert.Equal(text, comment.CommentText);
        Assert.Equal(rating, comment.CommentRating);
    }
    
    [Fact]
    public void Comment_Constructor_WithCommand_ShouldInitializeProperties()
    {
        // Arrange
        var command = new CreateCommentCommand(1, 1, "Excelente lugar", 5);

        // Act
        var comment = new Comment(command);

        // Assert
        Assert.Equal(command.UserId, comment.UserId);
        Assert.Equal(command.LocalId, comment.LocalId);
        Assert.Equal(command.Text, comment.CommentText);
        Assert.Equal(command.Rating, comment.CommentRating);
    }
    
}