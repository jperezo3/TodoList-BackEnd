using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Application.Interfaces.Services;
using TodoList.Application.Services;
using System.Reflection;

namespace TodoList.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITodoTaskService, TodoTaskService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
