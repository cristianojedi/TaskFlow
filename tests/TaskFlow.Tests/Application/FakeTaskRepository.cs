using TaskFlow.Application.Interfaces;
using TaskFlow.Domain;

namespace TaskFlow.Tests.Application;

public class FakeTaskRepository : ITaskRepository
{
    private readonly List<TaskItem> _tasks = [];
    private int _nextId = 1;

    public Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<TaskItem>>(_tasks.ToList());

    public Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_tasks.FirstOrDefault(t => t.Id == id));

    public Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        typeof(TaskItem).GetProperty(nameof(TaskItem.Id))!.SetValue(task, _nextId++);
        _tasks.Add(task);
        return Task.CompletedTask;
    }

    public void Remove(TaskItem task) => _tasks.Remove(task);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
