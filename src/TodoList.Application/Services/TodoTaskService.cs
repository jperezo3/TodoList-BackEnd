using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TodoList.Application.DTOs.TodoTasks;
using TodoList.Application.Interfaces.Services;
using TodoList.Domain.Common;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Domain.Exceptions;
using TodoList.Domain.Interfaces;
using TaskStatus = TodoList.Domain.Enums.TaskStatus;

namespace TodoList.Application.Services;

public class TodoTaskService : ITodoTaskService
{
    private readonly ITodoTaskRepository _taskRepository;
    private readonly IMapper _mapper;

    public TodoTaskService(ITodoTaskRepository taskRepository, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<TodoTaskDto>>> GetAllAsync(
        Guid userId,
        TaskStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<TodoTask> tasks;

        if (status.HasValue)
        {
            tasks = await _taskRepository.GetByUserIdAndStatusAsync(userId, status.Value, cancellationToken);
        }
        else
        {
            tasks = await _taskRepository.GetByUserIdAsync(userId, cancellationToken);
        }

        var taskDtos = _mapper.Map<IEnumerable<TodoTaskDto>>(tasks);
        return Result<IEnumerable<TodoTaskDto>>.Success(taskDtos);
    }

    public async Task<Result<TodoTaskDto>> GetByIdAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);

        if (task == null || task.UserId != userId)
        {
            return Result<TodoTaskDto>.Failure("Task not found");
        }

        var taskDto = _mapper.Map<TodoTaskDto>(task);
        return Result<TodoTaskDto>.Success(taskDto);
    }

    public async Task<Result<TodoTaskDto>> CreateAsync(
        CreateTodoTaskDto dto,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var task = _mapper.Map<TodoTask>(dto);
        task.UserId = userId;

        await _taskRepository.AddAsync(task, cancellationToken);

        var taskDto = _mapper.Map<TodoTaskDto>(task);
        return Result<TodoTaskDto>.Success(taskDto);
    }

    public async Task<Result<TodoTaskDto>> UpdateAsync(
        Guid id,
        UpdateTodoTaskDto dto,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);

        if (task == null || task.UserId != userId)
        {
            return Result<TodoTaskDto>.Failure("Task not found");
        }

        if (!string.IsNullOrEmpty(dto.Title))
            task.Title = dto.Title;

        if (dto.Description != null)
            task.Description = dto.Description;

        if (dto.Status.HasValue)
        {
            if (dto.Status.Value == TaskStatus.Completed)
                task.MarkAsCompleted();
            else
                task.MarkAsPending();
        }

        task.UpdatedAt = DateTime.UtcNow;

        await _taskRepository.UpdateAsync(task, cancellationToken);

        var taskDto = _mapper.Map<TodoTaskDto>(task);
        return Result<TodoTaskDto>.Success(taskDto);
    }

    public async Task<Result<bool>> DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);

        if (task == null || task.UserId != userId)
        {
            return Result<bool>.Failure("Task not found");
        }

        await _taskRepository.DeleteAsync(task, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<TodoTaskDto>> ToggleStatusAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);

        if (task == null || task.UserId != userId)
        {
            return Result<TodoTaskDto>.Failure("Task not found");
        }

        if (task.Status == TaskStatus.Pending)
            task.MarkAsCompleted();
        else
            task.MarkAsPending();

        await _taskRepository.UpdateAsync(task, cancellationToken);

        var taskDto = _mapper.Map<TodoTaskDto>(task);
        return Result<TodoTaskDto>.Success(taskDto);
    }
}