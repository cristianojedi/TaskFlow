using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain;

namespace TaskFlow.Infrastructure;

public class TaskRepository : ITaskRepository
{
    private readonly TaskFlowDbContext _context;

    public TaskRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Tasks.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default) =>
        await _context.Tasks.AddAsync(task, cancellationToken);

    public void Remove(TaskItem task) => _context.Tasks.Remove(task);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}
