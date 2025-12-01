using AlquilaFacilPlatform.Locals.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Locals.Domain.Model.Commands;
using Moq;
using AlquilaFacilPlatform.Locals.Interfaces.REST;
using AlquilaFacilPlatform.Locals.Domain.Services;
using AlquilaFacilPlatform.Locals.Domain.Model.Queries;
using AlquilaFacilPlatform.Locals.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;

namespace AlquilaFacilPlatform.Tests.CoreIntegrationTests;

public class CommentControllerTests
{
    [Fact]
    public async Task GetAllCommentsByLocalId_ReturnsOkResult_WithFilteredComments()
    {
        // Arrange
        var mockQueryService = new Mock<ICommentQueryService>();
        var mockCommandService = new Mock<ICommentCommandService>();

        // Crear una lista de comentarios con diferentes localIds
        var expectedComments = new List<Comment>
        {
            new Comment(1, 1, "Comment 1", 5),
            new Comment(2, 1, "Comment 2", 4),
            new Comment(3, 2, "Comment 3", 3),
            new Comment(4, 2, "Comment 4", 2) 
        };
        
        mockQueryService
            .Setup(s => s.Handle(It.IsAny<GetAllCommentsByLocalIdQuery>()))
            .ReturnsAsync((GetAllCommentsByLocalIdQuery query) =>
                expectedComments.Where(c => c.LocalId == query.LocalId).ToList());

        var controller = new CommentController(mockCommandService.Object, mockQueryService.Object);  

        // Act
        var result = await controller.GetAllCommentsByLocalId(1); 

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);  // Verifica que la respuesta es un OkObjectResult
        var returnedComments = Assert.IsAssignableFrom<IEnumerable<CommentResource>>(okResult.Value);  // Verifica que el valor es IEnumerable<CommentResource>
        // Verifica que el número de comentarios devueltos sea 2 (para localId = 1)
        Assert.Equal(2, returnedComments.Count());
    }
    
    [Fact]
    public async Task CreateComment_ReturnsCreatedResult_WithCreatedComment()
    {
        // Arrange
        var mockQueryService = new Mock<ICommentQueryService>();
        var mockCommandService = new Mock<ICommentCommandService>();

        var resource = new CreateCommentResource
        (
            1,
            1,
            "Nuevo comentario",
            5
        );

        // Simulamos el comentario devuelto por el comando
        var createdComment = new Comment(1, resource.LocalId, resource.Text, resource.Rating);

        mockCommandService
            .Setup(s => s.Handle(It.IsAny<CreateCommentCommand>()))
            .ReturnsAsync(createdComment);

        var controller = new CommentController(mockCommandService.Object, mockQueryService.Object);

        // Act
        var result = await controller.CreateComment(resource);

        // Assert
        var createdResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedComment = Assert.IsType<CommentResource>(createdResult.Value);
        Assert.Equal(resource.LocalId, returnedComment.LocalId);
        Assert.Equal(resource.Text, returnedComment.Text);
        Assert.Equal(resource.Rating, returnedComment.Rating);
    }

}
