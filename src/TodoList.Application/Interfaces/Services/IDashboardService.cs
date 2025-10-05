using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Application.DTOs.Dashboard;
using TodoList.Domain.Common;

namespace TodoList.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<Result<DashboardMetricsDto>> GetMetricsAsync(Guid userId, CancellationToken cancellationToken = default);
}
