using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoList.Application.DTOs.TodoTasks;
using TodoList.Application.Interfaces.Services;
using TodoList.Domain.Enums;
using TaskStatus = TodoList.Domain.Enums.TaskStatus;

namespace TodoList.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodoTasksController : ControllerBase
{
    private readonly ITodoTaskService _todoTaskService;
    private readonly ILogger<TodoTasksController> _logger;

    public TodoTasksController(
        ITodoTaskService todoTaskService,
        ILogger<TodoTasksController> logger)
    {
        _todoTaskService = todoTaskService;
        _logger = logger;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found"));
    }

    /// <summary>
    /// Get all tasks for the authenticated user
    /// </summary>
    /// <param name="status">Optional filter by task status</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoTaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] TaskStatus? status = null)
    {
        var userId = GetUserId();
        _logger.LogInformation("Getting all tasks for user: {UserId}", userId);

        var result = await _todoTaskService.GetAllAsync(userId, status);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    /// <summary>
    /// Get a specific task by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TodoTaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();
        var result = await _todoTaskService.GetByIdAsync(id, userId);

        if (!result.IsSuccess)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TodoTaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTodoTaskDto dto)
    {
        var userId = GetUserId();
        _logger.LogInformation("Creating new task for user: {UserId}", userId);

        var result = await _todoTaskService.CreateAsync(dto, userId);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
            result.Data);
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TodoTaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTodoTaskDto dto)
    {
        var userId = GetUserId();
        _logger.LogInformation("Updating task {TaskId} for user: {UserId}", id, userId);

        var result = await _todoTaskService.UpdateAsync(id, dto, userId);

        if (!result.IsSuccess)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        _logger.LogInformation("Deleting task {TaskId} for user: {UserId}", id, userId);

        var result = await _todoTaskService.DeleteAsync(id, userId);

        if (!result.IsSuccess)
            return NotFound(new { message = result.ErrorMessage });

        return NoContent();
    }

    /// <summary>
    /// Toggle task status between Pending and Completed
    /// </summary>
    [HttpPatch("{id:guid}/toggle-status")]
    [ProducesResponseType(typeof(TodoTaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        var userId = GetUserId();
        _logger.LogInformation("Toggling status for task {TaskId}", id);

        var result = await _todoTaskService.ToggleStatusAsync(id, userId);

        if (!result.IsSuccess)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }
}