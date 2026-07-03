namespace TaskFlow.Application.DTOs;

public record UpdateTaskRequest(string Title, string? Description, bool IsDone, DateTime? DueDate);
