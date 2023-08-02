using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Mappers;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Events;
using CleanArchitecture.Domain.Events.TodoItems;
using MediatR;

namespace CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;

public record CreateTodoItemCommand : IRequest<int>
{
    public int ListId { get; init; }

    public string Title { get; init; }
}

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var todoItem = request.MapToTodoItem();

        todoItem.Done = false;

        todoItem.AddDomainEvent(new TodoItemCreatedEvent(todoItem));

        _context.TodoItems.Add(todoItem);

        await _context.SaveChangesAsync(cancellationToken);

        return todoItem.ItemId;
    }
}