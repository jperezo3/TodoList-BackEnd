using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using TodoList.Application.DTOs.TodoTasks;

namespace TodoList.Application.Validators;

public class UpdateTodoTaskValidator : AbstractValidator<UpdateTodoTaskDto>
{
    public UpdateTodoTaskValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
