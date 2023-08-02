

namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public class TodoItemBriefDto 
{
    public int ItemId { get; set; }

    public int ListId { get; set; }

    public string Title { get; set; }

    public bool Done { get; set; }
}
