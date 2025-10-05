using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Domain.Enums;
using TaskStatus = TodoList.Domain.Enums.TaskStatus;

namespace TodoList.Domain.Entities;

public class TodoTask : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.Pending;
    public DateTime? CompletedAt { get; set; }

    // Foreign key
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public void MarkAsCompleted()
    {
        Status = TaskStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsPending()
    {
        Status = TaskStatus.Pending;
        CompletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
