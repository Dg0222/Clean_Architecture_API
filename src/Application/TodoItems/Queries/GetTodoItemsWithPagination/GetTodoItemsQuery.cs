using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Mappers;
using CleanArchitecture.Application.Common.Models;

namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public record GetTodoItemsQuery : IRequest<IQueryable<TodoItemBriefDto>>
{
    public int ListId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetTodoItemsQueryHandler : IRequestHandler<GetTodoItemsQuery, IQueryable<TodoItemBriefDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTodoItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<IQueryable<TodoItemBriefDto>> Handle(GetTodoItemsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_context.TodoItems
            .Where(x => x.ListId == request.ListId)
            .OrderBy(x => x.Title)
            .ProjectToDto());
    }
}