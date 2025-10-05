using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Domain.Interfaces;
using TodoList.Infrastructure.Data;
using TaskStatus = TodoList.Domain.Enums.TaskStatus;

namespace TodoList.Infrastructure.Repositories;

public class TodoTaskRepository : Repository<TodoTask>, ITodoTaskRepository
{
    public TodoTaskRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TodoTask>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TodoTask>> GetByUserIdAndStatusAsync(
        Guid userId,
        TaskStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.UserId == userId && t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(t => t.UserId == userId, cancellationToken);
    }

    public async Task<int> CountByUserIdAndStatusAsync(
        Guid userId,
        TaskStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(t => t.UserId == userId && t.Status == status, cancellationToken);
    }
}