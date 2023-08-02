using CleanArchitecture.Application.Common.Mappers;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Application.TodoLists.Commands.UpdateTodoList;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using NUnit.Framework;

namespace CleanArchitecture.Application.UnitTests.Common.Mappings
{
    public class TodoListMappingTests
    {
        [Test]
        public void ShouldProjectToTodoListDtoQueryable()
        {
            var todoLists = new List<TodoList>
            {
                new()
                {
                    ListId = 0,
                    Title = "Test",
                    Color = Color.Green
                }
            }.AsQueryable();

            var mappedResult = todoLists.ProjectToDto();

            Assert.IsInstanceOf<IQueryable<TodoListDto>>(mappedResult);
            Assert.IsNotNull(mappedResult);
        }

        [Test]
        public void ShouldSupportMappingFromTodoListCreateCommandToTodoList()
        {
            var todoListCreateCommand = new CreateTodoListCommand
            {
                Title = "Test"
            };

            var mappedResult = todoListCreateCommand.MapToTodoList();

            Assert.IsInstanceOf<TodoList>(mappedResult);
            Assert.AreEqual("Test", mappedResult?.Title);
        }

        [Test]
        public void ShouldSupportMappingFromTodoListUpdateCommandToTodoList()
        {
            var todoListUpdateCommand = new UpdateTodoListCommand
            {
                Id = 0,
                Title = "Test",
                Color = Color.Green
            };

            var todoList = new TodoList
            {
                ListId = 0,
                Title = "Test123",
                Color = Color.Blue
            };

            todoListUpdateCommand.MapToTodoList(todoList);

            Assert.AreEqual("Test", todoList.Title);
            Assert.AreEqual(Color.Green, todoList.Color);
        }
    }
}
