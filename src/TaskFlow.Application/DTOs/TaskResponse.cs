using TaskFlow.Domain;

namespace TaskFlow.Application.DTOs;

public record TaskResponse(int Id, string Title, string? Description, bool IsDone, DateTime CreatedAt, DateTime? DueDate)
{
    public static TaskResponse FromDomain(TaskItem task) =>
        new(task.Id, task.Title, task.Description, task.IsDone, task.CreatedAt, task.DueDate);
}
