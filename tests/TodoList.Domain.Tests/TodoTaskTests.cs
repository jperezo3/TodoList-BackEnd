using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using Xunit;
using FluentAssertions;
using TaskStatus = TodoList.Domain.Enums.TaskStatus;

namespace TodoList.Domain.Tests.Entities;

public class TodoTaskTests
{
    [Fact]
    public void TodoTask_ShouldInitialize_WithPendingStatus()
    {
        // Arrange & Act
        var task = new TodoTask
        {
            Title = "Test Task",
            Description = "Test Description",
            UserId = Guid.NewGuid()
        };

        // Assert
        task.Status.Should().Be(TaskStatus.Pending);
        task.CompletedAt.Should().BeNull();
        task.Id.Should().NotBeEmpty();
        task.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void MarkAsCompleted_ShouldUpdateStatusAndCompletedAt()
    {
        // Arrange
        var task = new TodoTask
        {
            Title = "Test Task",
            Description = "Test Description",
            UserId = Guid.NewGuid()
        };

        // Act
        task.MarkAsCompleted();

        // Assert
        task.Status.Should().Be(TaskStatus.Completed);
        task.CompletedAt.Should().NotBeNull();
        task.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        task.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsPending_ShouldUpdateStatusAndClearCompletedAt()
    {
        // Arrange
        var task = new TodoTask
        {
            Title = "Test Task",
            Description = "Test Description",
            UserId = Guid.NewGuid()
        };
        task.MarkAsCompleted();

        // Act
        task.MarkAsPending();

        // Assert
        task.Status.Should().Be(TaskStatus.Pending);
        task.CompletedAt.Should().BeNull();
        task.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsCompleted_MultipleTimes_ShouldUpdateTimestamp()
    {
        // Arrange
        var task = new TodoTask
        {
            Title = "Test Task",
            Description = "Test Description",
            UserId = Guid.NewGuid()
        };
        task.MarkAsCompleted();
        var firstCompletedAt = task.CompletedAt;

        // Act
        Thread.Sleep(100); // delay to ensure timestamp changes
        task.MarkAsPending();
        task.MarkAsCompleted();

        // Assert
        task.CompletedAt.Should().BeAfter(firstCompletedAt!.Value);
    }
}