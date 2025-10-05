using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Application.Interfaces.Infrastructure;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TaskStatus = TodoList.Domain.Enums.TaskStatus;

namespace TodoList.Infrastructure.Data;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        // Check if database is already seeded
        if (_context.Users.Any())
            return;

        // Create users
        var adminUser = new User
        {
            Email = "admin@todolist.com",
            PasswordHash = _passwordHasher.HashPassword("Admin123!"),
            FullName = "Admin User"
        };

        var regularUser = new User
        {
            Email = "user@todolist.com",
            PasswordHash = _passwordHasher.HashPassword("User123!"),
            FullName = "Regular User"
        };

        _context.Users.AddRange(adminUser, regularUser);
        await _context.SaveChangesAsync();

        // Create sample tasks for admin user
        var sampleTasks = new List<TodoTask>
        {
            new TodoTask
            {
                Title = "Complete project documentation",
                Description = "Write comprehensive documentation for the Todo List API",
                Status = TaskStatus.Pending,
                UserId = adminUser.Id
            },
            new TodoTask
            {
                Title = "Review pull requests",
                Description = "Review and merge pending pull requests",
                Status = TaskStatus.Completed,
                UserId = adminUser.Id,
                CompletedAt = DateTime.UtcNow.AddDays(-1)
            },
            new TodoTask
            {
                Title = "Update dependencies",
                Description = "Update all NuGet packages to latest stable versions",
                Status = TaskStatus.Pending,
                UserId = adminUser.Id
            }
        };

        // Create sample tasks for regular user
        var userTasks = new List<TodoTask>
        {
            new TodoTask
            {
                Title = "Buy groceries",
                Description = "Milk, bread, eggs, and fruits",
                Status = TaskStatus.Pending,
                UserId = regularUser.Id
            },
            new TodoTask
            {
                Title = "Morning exercise",
                Description = "30 minutes cardio workout",
                Status = TaskStatus.Completed,
                UserId = regularUser.Id,
                CompletedAt = DateTime.UtcNow
            }
        };

        _context.TodoTasks.AddRange(sampleTasks);
        _context.TodoTasks.AddRange(userTasks);
        await _context.SaveChangesAsync();
    }
}