using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using TodoList.API.Controllers;
using TodoList.Application.DTOs.TodoTasks;
using TodoList.Application.Interfaces.Services;
using TodoList.Domain.Common;
using TodoList.Domain.Enums;
using Xunit;
using TaskStatus = TodoList.Domain.Enums.TaskStatus;

namespace TodoList.API.Tests.Controllers;

public class TodoTasksControllerTests
{
    private readonly Mock<ITodoTaskService> _todoTaskServiceMock;
    private readonly Mock<ILogger<TodoTasksController>> _loggerMock;
    private readonly TodoTasksController _controller;
    private readonly Guid _userId;

    public TodoTasksControllerTests()
    {
        _todoTaskServiceMock = new Mock<ITodoTaskService>();
        _loggerMock = new Mock<ILogger<TodoTasksController>>();
        _controller = new TodoTasksController(_todoTaskServiceMock.Object, _loggerMock.Object);

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
    public async Task GetAll_ShouldReturnOkWithTasks()
    {
        // Arrange
        var tasks = new List<TodoTaskDto>
        {
            new TodoTaskDto { Id = Guid.NewGuid(), Title = "Task 1", Status = TaskStatus.Pending },
            new TodoTaskDto { Id = Guid.NewGuid(), Title = "Task 2", Status = TaskStatus.Completed }
        };

        _todoTaskServiceMock
            .Setup(x => x.GetAllAsync(_userId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IEnumerable<TodoTaskDto>>.Success(tasks));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTasks = okResult.Value.Should().BeAssignableTo<IEnumerable<TodoTaskDto>>().Subject;
        returnedTasks.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_WithStatusFilter_ShouldReturnFilteredTasks()
    {
        // Arrange
        var completedTasks = new List<TodoTaskDto>
        {
            new TodoTaskDto { Id = Guid.NewGuid(), Title = "Completed Task", Status = TaskStatus.Completed }
        };

        _todoTaskServiceMock
            .Setup(x => x.GetAllAsync(_userId, TaskStatus.Completed, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IEnumerable<TodoTaskDto>>.Success(completedTasks));

        // Act
        var result = await _controller.GetAll(TaskStatus.Completed);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTasks = okResult.Value.Should().BeAssignableTo<IEnumerable<TodoTaskDto>>().Subject;
        returnedTasks.Should().HaveCount(1);
        returnedTasks.First().Status.Should().Be(TaskStatus.Completed);
    }

    [Fact]
    public async Task GetById_WithExistingTask_ShouldReturnOk()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TodoTaskDto
        {
            Id = taskId,
            Title = "Test Task",
            Description = "Description",
            Status = TaskStatus.Pending
        };

        _todoTaskServiceMock
            .Setup(x => x.GetByIdAsync(taskId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TodoTaskDto>.Success(task));

        // Act
        var result = await _controller.GetById(taskId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTask = okResult.Value.Should().BeAssignableTo<TodoTaskDto>().Subject;
        returnedTask.Id.Should().Be(taskId);
    }

    [Fact]
    public async Task GetById_WithNonExistingTask_ShouldReturnNotFound()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _todoTaskServiceMock
            .Setup(x => x.GetByIdAsync(taskId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TodoTaskDto>.Failure("Task not found"));

        // Act
        var result = await _controller.GetById(taskId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var createDto = new CreateTodoTaskDto
        {
            Title = "New Task",
            Description = "New Description"
        };

        var createdTask = new TodoTaskDto
        {
            Id = Guid.NewGuid(),
            Title = createDto.Title,
            Description = createDto.Description,
            Status = TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _todoTaskServiceMock
            .Setup(x => x.CreateAsync(createDto, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TodoTaskDto>.Success(createdTask));

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedTask = createdResult.Value.Should().BeAssignableTo<TodoTaskDto>().Subject;
        returnedTask.Title.Should().Be(createDto.Title);
        createdResult.ActionName.Should().Be(nameof(TodoTasksController.GetById));
    }

    [Fact]
    public async Task Update_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var updateDto = new UpdateTodoTaskDto
        {
            Title = "Updated Title",
            Description = "Updated Description"
        };

        var updatedTask = new TodoTaskDto
        {
            Id = taskId,
            Title = updateDto.Title,
            Description = updateDto.Description,
            Status = TaskStatus.Pending
        };

        _todoTaskServiceMock
            .Setup(x => x.UpdateAsync(taskId, updateDto, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TodoTaskDto>.Success(updatedTask));

        // Act
        var result = await _controller.Update(taskId, updateDto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTask = okResult.Value.Should().BeAssignableTo<TodoTaskDto>().Subject;
        returnedTask.Title.Should().Be(updateDto.Title);
    }

    [Fact]
    public async Task Delete_WithExistingTask_ShouldReturnNoContent()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _todoTaskServiceMock
            .Setup(x => x.DeleteAsync(taskId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _controller.Delete(taskId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_WithNonExistingTask_ShouldReturnNotFound()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _todoTaskServiceMock
            .Setup(x => x.DeleteAsync(taskId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Task not found"));

        // Act
        var result = await _controller.Delete(taskId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ToggleStatus_WithExistingTask_ShouldReturnOk()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var toggledTask = new TodoTaskDto
        {
            Id = taskId,
            Title = "Task",
            Status = TaskStatus.Completed,
            CompletedAt = DateTime.UtcNow
        };

        _todoTaskServiceMock
            .Setup(x => x.ToggleStatusAsync(taskId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TodoTaskDto>.Success(toggledTask));

        // Act
        var result = await _controller.ToggleStatus(taskId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTask = okResult.Value.Should().BeAssignableTo<TodoTaskDto>().Subject;
        returnedTask.Status.Should().Be(TaskStatus.Completed);
    }
}