using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Application.DTOs.TodoTasks;
using TodoList.Domain.Common;
using TodoList.Domain.Enums;
using TaskStatus = TodoList.Domain.Enums.TaskStatus;

namespace TodoList.Application.Interfaces.Services;

public interface ITodoTaskService
{
    Task<Result<IEnumerable<TodoTaskDto>>> GetAllAsync(Guid userId, TaskStatus? status = null, CancellationToken cancellationToken = default);
    Task<Result<TodoTaskDto>> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<TodoTaskDto>> CreateAsync(CreateTodoTaskDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<TodoTaskDto>> UpdateAsync(Guid id, UpdateTodoTaskDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<TodoTaskDto>> ToggleStatusAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
