using AutoMapper;
using CleanArchitecture.Application.Common.Mappings;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.TodoLists.Queries.GetTodos;

public class TodoListDto : IMapFrom<TodoList>
{
    public int ListId { get; set; }

    public string Title { get; set; }

    public List<TodoItemDto> TodoItems { get; set; }


    public class TodoItemDto : IMapFrom<TodoItem>
    {
        public int ItemId { get; set; }

        public string Title { get; set; }

        public string Note { get; set; }

        public PriorityLevel Priority { get; set; }

        public DateTime? Reminder { get; set; }

    }
}
