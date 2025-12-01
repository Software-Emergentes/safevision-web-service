using AlquilaFacilPlatform.Locals.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Locals.Domain.Model.Commands;
using AlquilaFacilPlatform.Locals.Domain.Model.Queries;
using AlquilaFacilPlatform.Locals.Domain.Services;
using AlquilaFacilPlatform.Locals.Interfaces.REST;
using AlquilaFacilPlatform.Locals.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AlquilaFacilPlatform.Tests.CoreIntegrationTests;

public class ReportControllerTests
{
    [Fact]
    public async Task CreateReport_ReturnsCreatedResult_WithCreatedReport()
    {
        // Arrange
        var mockQueryService = new Mock<IReportQueryService>();
        var mockCommandService = new Mock<IReportCommandService>();

        var createResource = new CreateReportResource
        (
            1,
            "Incidencia importante",
            2,
            "Descripción de la incidencia"
        );
        
        var createCommand = new CreateReportCommand(
            createResource.LocalId,
            createResource.Title,
            createResource.UserId,
            createResource.Description
        );

        var createdReport = new Report(createCommand);

        mockCommandService
            .Setup(s => s.Handle(It.IsAny<CreateReportCommand>()))
            .ReturnsAsync(createdReport);

        var controller = new ReportController(mockQueryService.Object, mockCommandService.Object);

        // Act
        var result = await controller.CreateReport(createResource);

        // Assert
        var createdResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
        var reportResource = Assert.IsType<ReportResource>(createdResult.Value);
        Assert.Equal(createResource.Description, reportResource.Description);
    }
    
    [Fact]
    public async Task GetReportsByUserId_ReturnsOkResult_WithFilteredReports()
    {
        var mockQueryService = new Mock<IReportQueryService>();
        var mockCommandService = new Mock<IReportCommandService>();

        var reports = new List<Report>
        {
            new Report(new CreateReportCommand(1, "Titulo 1", 2, "Descripción 1")),
            new Report(new CreateReportCommand(2, "Titulo 2", 1,"Descripción 2")),
            new Report(new CreateReportCommand(3, "Titulo 3", 2, "Otro usuario"))
        };

        mockQueryService
            .Setup(s => s.Handle(It.IsAny<GetReportsByUserIdQuery>()))
            .ReturnsAsync(reports.Where(r => r.UserId == 1).ToList());

        var controller = new ReportController(mockQueryService.Object, mockCommandService.Object);

        var result = await controller.GetReportsByUserId(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var reportResources = Assert.IsAssignableFrom<IEnumerable<ReportResource>>(okResult.Value);
        Assert.All(reportResources, r => Assert.Equal(1, r.UserId));
    }
    
    [Fact]
    public async Task GetReportsByLocalId_ReturnsOkResult_WithFilteredReports()
    {
        var mockQueryService = new Mock<IReportQueryService>();
        var mockCommandService = new Mock<IReportCommandService>();

        var reports = new List<Report>
        {
            new Report(new CreateReportCommand(1, "Titulo 1", 2, "Descripción 1")),
            new Report(new CreateReportCommand(2, "Titulo 2", 1,"Descripción 2")),
            new Report(new CreateReportCommand(2, "Titulo 3", 2, "Otro usuario"))
        };

        mockQueryService
            .Setup(s => s.Handle(It.IsAny<GetReportsByLocalIdQuery>()))
            .ReturnsAsync(reports.Where(r => r.LocalId == 2).ToList());

        var controller = new ReportController(mockQueryService.Object, mockCommandService.Object);

        var result = await controller.GetReportsByLocalId(2);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var reportResources = Assert.IsAssignableFrom<IEnumerable<ReportResource>>(okResult.Value);
        Assert.All(reportResources, r => Assert.Equal(2, r.LocalId));
    }


    [Fact]
    public async Task DeleteReport_ReturnsOkResult_WhenReportIsDeleted()
    {
        // Arrange
        var mockQueryService = new Mock<IReportQueryService>();
        var mockCommandService = new Mock<IReportCommandService>();

        var deletedReport = new Report(new CreateReportCommand(1, "Titulo 1", 2, "Descripción 1"));
        deletedReport.Id = 1;

        mockCommandService
            .Setup(s => s.Handle(It.IsAny<DeleteReportCommand>()))
            .ReturnsAsync(deletedReport);  // Devuelve un Report válido

        var controller = new ReportController(mockQueryService.Object, mockCommandService.Object);

        // Act
        var result = await controller.DeleteReport(1);

        // Assert
        var okResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        var report = Assert.IsType<Report>(okResult.Value);
        Assert.Equal(1, report.Id);  // O cualquier otra validación
    }

}