using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Application.DTOs.Dashboard;
using TodoList.Application.Interfaces.Services;
using TodoList.Domain.Common;
using TodoList.Domain.Enums;
using TodoList.Domain.Interfaces;
using TaskStatus = TodoList.Domain.Enums.TaskStatus;

namespace TodoList.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly ITodoTaskRepository _taskRepository;

    public DashboardService(ITodoTaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result<DashboardMetricsDto>> GetMetricsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var totalTasks = await _taskRepository.CountByUserIdAsync(userId, cancellationToken);
        var completedTasks = await _taskRepository.CountByUserIdAndStatusAsync(userId, TaskStatus.Completed, cancellationToken);
        var pendingTasks = await _taskRepository.CountByUserIdAndStatusAsync(userId, TaskStatus.Pending, cancellationToken);

        var completionPercentage = totalTasks > 0
            ? Math.Round((double)completedTasks / totalTasks * 100, 2)
            : 0;

        var metrics = new DashboardMetricsDto
        {
            TotalTasks = totalTasks,
            CompletedTasks = completedTasks,
            PendingTasks = pendingTasks,
            CompletionPercentage = completionPercentage
        };

        return Result<DashboardMetricsDto>.Success(metrics);
    }
}