namespace TaskFlow.Application.DTOs;

public record CreateTaskRequest(string Title, string? Description, DateTime? DueDate);
