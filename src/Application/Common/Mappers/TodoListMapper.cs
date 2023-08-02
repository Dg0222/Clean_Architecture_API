using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItemDetail;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using CleanArchitecture.Domain.Entities;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Application.TodoLists.Commands.UpdateTodoList;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;

namespace CleanArchitecture.Application.Common.Mappers
{
    [Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName)]
    public static partial class TodoListMapper
    {
        public static partial IQueryable<TodoListDto> ProjectToDto(this IQueryable<TodoList> q);

        public static partial TodoList MapToTodoList(this CreateTodoListCommand command);

        public static partial void MapToTodoList(this UpdateTodoListCommand command, TodoList list);

        //public static partial void MapToTodoList(this UpdateTodoItemDetailCommand command, TodoItem item);
    }
}
