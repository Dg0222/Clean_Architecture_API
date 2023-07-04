using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.TodoLists.Commands.DeleteTodoList;

public record DeleteTodoListCommand(int ListId) : IRequest;

public class DeleteTodoListCommandHandler : IRequestHandler<DeleteTodoListCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoLists
            .Where(l => l.ListId == request.ListId)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.ListId, entity);

        _context.TodoLists.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}