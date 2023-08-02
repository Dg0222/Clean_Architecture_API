using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.TodoLists.Queries.GetTodos;

public class TodoListDto
{
    public TodoListDto()
    {
        Items = Array.Empty<TodoItemDto>();
    }

    public int ListId { get; init; }

    public string Title { get; init; }

    public string Color { get; init; }

    public IReadOnlyCollection<TodoItemDto> Items { get; init; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<TodoList, TodoListDto>()
            .ForMember(x => x.Items, opt => opt.MapFrom(s => s.Items));
    }

}
