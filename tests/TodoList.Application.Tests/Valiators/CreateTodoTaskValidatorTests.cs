using FluentAssertions;
using TodoList.Application.DTOs.TodoTasks;
using TodoList.Application.Validators;
using Xunit;

namespace TodoList.Application.Tests.Validators;

public class CreateTodoTaskValidatorTests
{
    private readonly CreateTodoTaskValidator _validator;

    public CreateTodoTaskValidatorTests()
    {
        _validator = new CreateTodoTaskValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        // Arrange
        var dto = new CreateTodoTaskDto
        {
            Title = "Valid Task",
            Description = "Valid Description"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyTitle_ShouldFail()
    {
        // Arrange
        var dto = new CreateTodoTaskDto
        {
            Title = "",
            Description = "Description"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_WithTitleTooLong_ShouldFail()
    {
        // Arrange
        var dto = new CreateTodoTaskDto
        {
            Title = new string('a', 201), // 201 characters
            Description = "Description"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_WithDescriptionTooLong_ShouldFail()
    {
        // Arrange
        var dto = new CreateTodoTaskDto
        {
            Title = "Valid Title",
            Description = new string('a', 1001) // 1001 characters
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }
}