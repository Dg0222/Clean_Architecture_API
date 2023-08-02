using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Mappers;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Application.TodoLists.Commands.UpdateTodoList;

public record UpdateTodoListCommand : IRequest
{
    public int Id { get; init; }

    public string Title { get; init; }

    public Color Color { get; set; }
}

public class UpdateTodoListCommandHandler : IRequestHandler<UpdateTodoListCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateTodoListCommand request, CancellationToken cancellationToken)
    {
        var todoList = await _context.TodoLists
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, todoList);

        request.MapToTodoList(todoList);

        await _context.SaveChangesAsync(cancellationToken);

    }
}