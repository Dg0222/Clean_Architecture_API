using System.ComponentModel.DataAnnotations;
using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Entities;

public class TodoList : BaseAuditableEntity
{
    public TodoList()
    {
        TodoItems = new List<TodoItem>();
    }

    [Key]
    public int ListId { get; set; }

    public string Title { get; set; } = null!;

    public List<TodoItem> TodoItems { get; set; }
}
