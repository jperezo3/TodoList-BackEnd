using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using TodoList.API.Controllers;
using TodoList.Application.DTOs.Dashboard;
using TodoList.Application.Interfaces.Services;
using TodoList.Domain.Common;
using Xunit;

namespace TodoList.API.Tests.Controllers;

public class DashboardControllerTests
{
    private readonly Mock<IDashboardService> _dashboardServiceMock;
    private readonly Mock<ILogger<DashboardController>> _loggerMock;
    private readonly DashboardController _controller;
    private readonly Guid _userId;

    public DashboardControllerTests()
    {
        _dashboardServiceMock = new Mock<IDashboardService>();
        _loggerMock = new Mock<ILogger<DashboardController>>();
        _controller = new DashboardController(_dashboardServiceMock.Object, _loggerMock.Object);

        _userId = Guid.NewGuid();

        // Setup fake user authentication
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _userId.ToString()),
            new Claim(ClaimTypes.Email, "test@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task GetMetrics_ShouldReturnOkWithMetrics()
    {
        // Arrange
        var metrics = new DashboardMetricsDto
        {
            TotalTasks = 10,
            CompletedTasks = 6,
            PendingTasks = 4,
            CompletionPercentage = 60.0
        };

        _dashboardServiceMock
            .Setup(x => x.GetMetricsAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DashboardMetricsDto>.Success(metrics));

        // Act
        var result = await _controller.GetMetrics();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedMetrics = okResult.Value.Should().BeAssignableTo<DashboardMetricsDto>().Subject;
        returnedMetrics.TotalTasks.Should().Be(10);
        returnedMetrics.CompletedTasks.Should().Be(6);
        returnedMetrics.PendingTasks.Should().Be(4);
        returnedMetrics.CompletionPercentage.Should().Be(60.0);
    }

    [Fact]
    public async Task GetMetrics_WithNoTasks_ShouldReturnZeroMetrics()
    {
        // Arrange
        var metrics = new DashboardMetricsDto
        {
            TotalTasks = 0,
            CompletedTasks = 0,
            PendingTasks = 0,
            CompletionPercentage = 0
        };

        _dashboardServiceMock
            .Setup(x => x.GetMetricsAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DashboardMetricsDto>.Success(metrics));

        // Act
        var result = await _controller.GetMetrics();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedMetrics = okResult.Value.Should().BeAssignableTo<DashboardMetricsDto>().Subject;
        returnedMetrics.TotalTasks.Should().Be(0);
        returnedMetrics.CompletionPercentage.Should().Be(0);
    }

    [Fact]
    public async Task GetMetrics_ShouldLogRequest()
    {
        // Arrange
        var metrics = new DashboardMetricsDto();

        _dashboardServiceMock
            .Setup(x => x.GetMetricsAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DashboardMetricsDto>.Success(metrics));

        // Act
        await _controller.GetMetrics();

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Getting dashboard metrics")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}