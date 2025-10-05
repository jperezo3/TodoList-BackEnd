using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TaskStatus = TodoList.Domain.Enums.TaskStatus;

namespace TodoList.Domain.Interfaces;

public interface ITodoTaskRepository : IRepository<TodoTask>
{
    Task<IEnumerable<TodoTask>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TodoTask>> GetByUserIdAndStatusAsync(Guid userId, TaskStatus status, CancellationToken cancellationToken = default);
    Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> CountByUserIdAndStatusAsync(Guid userId, TaskStatus status, CancellationToken cancellationToken = default);
}
