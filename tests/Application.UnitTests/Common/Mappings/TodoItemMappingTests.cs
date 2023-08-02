using CleanArchitecture.Application.Common.Mappers;
using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using NUnit.Framework;

namespace CleanArchitecture.Application.UnitTests.Common.Mappings;

public class TodoItemMappingTests
{

    [Test]
    public void ShouldProjectToTodoItemBriefDtoQueryable()
    {
        var todoItems = new List<TodoItem>
        {
            new()
            {
                ItemId = 0,
                Title = "Test",
                Note = "Test",
                Priority = PriorityLevel.None,
                Reminder = null,
                Done = false,
                ListId = 0,
            }
        }.AsQueryable();

        var mappedResult = todoItems.ProjectToDto();

        Assert.IsInstanceOf<IQueryable<TodoItemBriefDto>>(mappedResult);
        Assert.IsNotNull(mappedResult);
    }

    [Test]
    public void ShouldSupportMappingFromTodoItemCreateCommandToTodoItem()
    {
        var todoItemCreateCommand = new CreateTodoItemCommand
        {
            ListId = 0,
            Title = "Test"
        };

        var mappedResult = todoItemCreateCommand.MapToTodoItem();

        Assert.IsInstanceOf<TodoItem>(mappedResult);
        Assert.AreEqual("Test", mappedResult?.Title);
    }

    [Test]
    public void ShouldSupportMappingFromTodoItemUpdateCommandToTodoItem()
    {
        var todoItemUpdateCommand = new UpdateTodoItemCommand
        {
            Id = 0,
            Title = "Test23",
            Done = true
        };

        var todoItem = new TodoItem
        {
            ItemId = 0,
            Title = "Test",
            Note = "Test",
            Priority = PriorityLevel.None,
            Reminder = null,
            Done = false,
            ListId = 0,
        };

        todoItemUpdateCommand.MapToTodoItem(todoItem);

        Assert.AreEqual("Test23", todoItem.Title);
        Assert.IsTrue(todoItem.Done);
    }
}
