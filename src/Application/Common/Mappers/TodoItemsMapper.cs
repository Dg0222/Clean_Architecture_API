using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItemDetail;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using CleanArchitecture.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace CleanArchitecture.Application.Common.Mappers
{
    [Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName)]
    public static partial class TodoItemsMapper
    {
        public static partial IQueryable<TodoItemBriefDto> ProjectToDto(this IQueryable<TodoItem> q);

        public static partial TodoItem MapToTodoItem(this CreateTodoItemCommand command);

        public static partial void MapToTodoItem(this UpdateTodoItemCommand command, TodoItem item);

        public static partial void MapToTodoItem(this UpdateTodoItemDetailCommand command, TodoItem item);
    }
}
