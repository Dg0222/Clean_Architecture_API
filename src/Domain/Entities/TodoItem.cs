using System.ComponentModel.DataAnnotations;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.Events.TodoItems;

namespace CleanArchitecture.Domain.Entities;

public class TodoItem : BaseAuditableEntity
{
    [Key]
    public int ItemId { get; set; }

    public string Title { get; set; }

    public string Note { get; set; }

    public PriorityLevel Priority { get; set; }

    public DateTime? Reminder { get; set; }

    private bool _done;
    public bool Done
    {
        get => _done;
        set
        {
            if (value == true && _done == false)
            {
                AddDomainEvent(new TodoItemCompletedEvent(this));
            }

            _done = value;
        }
    }

    public int ListId { get; set; }
    public virtual TodoList Todolist { get; set; }
}
