using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Mappers;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;

public record CreateTodoListCommand : IRequest<int>
{
    public string Title { get; init; }
}

public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {

       var todoList = request.MapToTodoList();

        _context.TodoLists.Add(todoList);

        await _context.SaveChangesAsync(cancellationToken);

        return todoList.ListId;
    }
}