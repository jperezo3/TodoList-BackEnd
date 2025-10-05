using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Application.DTOs.TodoTasks;
using TodoList.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TodoList.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TodoTask, TodoTaskDto>();
        CreateMap<CreateTodoTaskDto, TodoTask>();
        CreateMap<UpdateTodoTaskDto, TodoTask>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
