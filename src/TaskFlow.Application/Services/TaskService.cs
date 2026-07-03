using TaskFlow.Application.DTOs;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain;

namespace TaskFlow.Application.Services;

public class TaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IReadOnlyList<TaskResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tasks = await _taskRepository.GetAllAsync(cancellationToken);
        return tasks.Select(TaskResponse.FromDomain).ToList();
    }

    public async Task<TaskResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
        return task is null ? null : TaskResponse.FromDomain(task);
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = TaskItem.Create(request.Title, request.Description, request.DueDate);
        await _taskRepository.AddAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);
        return TaskResponse.FromDomain(task);
    }

    public async Task<TaskResponse?> UpdateAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return null;
        }

        task.Update(request.Title, request.Description, request.IsDone, request.DueDate);
        await _taskRepository.SaveChangesAsync(cancellationToken);
        return TaskResponse.FromDomain(task);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return false;
        }

        _taskRepository.Remove(task);
        await _taskRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
