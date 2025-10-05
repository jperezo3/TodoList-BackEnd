using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Application.Interfaces.Infrastructure;
using TodoList.Domain.Interfaces;
using TodoList.Infrastructure.Data;
using TodoList.Infrastructure.Repositories;
using TodoList.Infrastructure.Security;

namespace TodoList.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database Configuration
        var useInMemoryDatabase = configuration.GetValue<bool>("DatabaseSettings:UseInMemoryDatabase");

        if (useInMemoryDatabase)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TodoListDb"));
        }
        else
        {
            var connectionString = configuration["DatabaseSettings:ConnectionString"];
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString!));
        }

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITodoTaskRepository, TodoTaskRepository>();

        // Security
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        // Database Seeder
        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}
