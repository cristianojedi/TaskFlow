using TaskFlow.Domain;

namespace TaskFlow.Application.Interfaces;

public interface ITaskRepository
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
    void Remove(TaskItem task);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
