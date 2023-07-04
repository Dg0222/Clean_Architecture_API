namespace CleanArchitecture.Domain.Entities;

public class TodoList : BaseAuditableEntity
{
    public TodoList()
    {
        Color = Color.White;
        Items = new List<TodoItem>();
    }

    public int ListId { get; set; }

    public string Title { get; set; }

    public Color Color { get; set; }

    public IList<TodoItem> Items { get; private set; }
}