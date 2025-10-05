using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Domain.Enums;
using TaskStatus = TodoList.Domain.Enums.TaskStatus;

namespace TodoList.Application.DTOs.TodoTasks;

public class TodoTaskFilterDto
{
    public TaskStatus? Status { get; set; }
}
