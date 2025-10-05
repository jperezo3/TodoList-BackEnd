using AutoMapper;
using FluentAssertions;
using Moq;
using TodoList.Application.DTOs.TodoTasks;
using TodoList.Application.Mappings;
using TodoList.Application.Services;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Domain.Interfaces;
using Xunit;
using TaskStatus = TodoList.Domain.Enums.TaskStatus;

namespace TodoList.Application.Tests.Services;

public class TodoTaskServiceTests
{
    private readonly Mock<ITodoTaskRepository> _taskRepositoryMock;
    private readonly IMapper _mapper;
    private readonly TodoTaskService _todoTaskService;
    private readonly Guid _userId;

    public TodoTaskServiceTests()
    {
        _taskRepositoryMock = new Mock<ITodoTaskRepository>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _todoTaskService = new TodoTaskService(_taskRepositoryMock.Object, _mapper);
        _userId = Guid.NewGuid();
    }

    [Fact]
    public async Task GetAllAsync_WithoutFilter_ShouldReturnAllUserTasks()
    {
        // Arrange
        var tasks = new List<TodoTask>
        {
            new TodoTask { Title = "Task 1", UserId = _userId, Status = TaskStatus.Pending },
            new TodoTask { Title = "Task 2", UserId = _userId, Status = TaskStatus.Completed }
        };

        _taskRepositoryMock
            .Setup(x => x.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _todoTaskService.GetAllAsync(_userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_WithStatusFilter_ShouldReturnFilteredTasks()
    {
        // Arrange
        var completedTasks = new List<TodoTask>
        {
            new TodoTask { Title = "Completed Task", UserId = _userId, Status = TaskStatus.Completed }
        };

        _taskRepositoryMock
            .Setup(x => x.GetByUserIdAndStatusAsync(_userId, TaskStatus.Completed, It.IsAny<CancellationToken>()))
            .ReturnsAsync(completedTasks);

        // Act
        var result = await _todoTaskService.GetAllAsync(_userId, TaskStatus.Completed);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data!.First().Status.Should().Be(TaskStatus.Completed);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTaskAndReturnDto()
    {
        // Arrange
        var createDto = new CreateTodoTaskDto
        {
            Title = "New Task",
            Description = "Task Description"
        };

        _taskRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<TodoTask>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoTask task, CancellationToken ct) => task);

        // Act
        var result = await _todoTaskService.CreateAsync(createDto, _userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be(createDto.Title);
        result.Data.Description.Should().Be(createDto.Description);
        result.Data.Status.Should().Be(TaskStatus.Pending);
    }

    [Fact]
    public async Task UpdateAsync_WithValidTask_ShouldUpdateAndReturnDto()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TodoTask
        {
            Id = taskId,
            Title = "Original Title",
            Description = "Original Description",
            UserId = _userId,
            Status = TaskStatus.Pending
        };

        var updateDto = new UpdateTodoTaskDto
        {
            Title = "Updated Title",
            Description = "Updated Description"
        };

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _taskRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<TodoTask>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _todoTaskService.UpdateAsync(taskId, updateDto, _userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be(updateDto.Title);
        result.Data.Description.Should().Be(updateDto.Description);
    }

    [Fact]
    public async Task UpdateAsync_WithWrongUser_ShouldReturnFailure()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var existingTask = new TodoTask
        {
            Id = taskId,
            Title = "Task",
            UserId = differentUserId
        };

        var updateDto = new UpdateTodoTaskDto { Title = "Updated" };

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        // Act
        var result = await _todoTaskService.UpdateAsync(taskId, updateDto, _userId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Task not found");
    }

    [Fact]
    public async Task DeleteAsync_WithValidTask_ShouldDeleteAndReturnSuccess()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TodoTask
        {
            Id = taskId,
            Title = "Task to delete",
            UserId = _userId
        };

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _taskRepositoryMock
            .Setup(x => x.DeleteAsync(existingTask, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _todoTaskService.DeleteAsync(taskId, _userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    [Fact]
    public async Task ToggleStatusAsync_FromPendingToCompleted_ShouldUpdateStatus()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TodoTask
        {
            Id = taskId,
            Title = "Task",
            UserId = _userId,
            Status = TaskStatus.Pending
        };

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _taskRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<TodoTask>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _todoTaskService.ToggleStatusAsync(taskId, _userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Status.Should().Be(TaskStatus.Completed);
        result.Data.CompletedAt.Should().NotBeNull();
    }
}