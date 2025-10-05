using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TodoList.Application.DTOs.Dashboard;

public class DashboardMetricsDto
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public double CompletionPercentage { get; set; }
}
