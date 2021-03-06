using AutoMapper;
using CleanArchitecture.Application.Common.Mappings;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public class TodoItemDto : IMapFrom<TodoItem>
{
    public int ItemId { get; set; }

    public int ListId { get; set; }

    public string Title { get; set; }

    public bool Done { get; set; }

    public int Priority { get; set; }

    public string Note { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<TodoItem, TodoItemDto>()
            .ForMember(d => d.Priority, opt => opt.MapFrom(s => (int)s.Priority));
    }
}
