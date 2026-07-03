using TaskFlow.Application.DTOs;
using TaskFlow.Application.Services;

namespace TaskFlow.Api.Endpoints;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/tasks");

        group.MapGet("/", async (TaskService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAllAsync(cancellationToken)));

        group.MapGet("/{id:int}", async (int id, TaskService service, CancellationToken cancellationToken) =>
        {
            var task = await service.GetByIdAsync(id, cancellationToken);
            return task is null ? Results.NotFound() : Results.Ok(task);
        });

        group.MapPost("/", async (CreateTaskRequest request, TaskService service, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.BadRequest(new { error = "Title é obrigatório." });
            }

            var created = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/tasks/{created.Id}", created);
        });

        group.MapPut("/{id:int}", async (int id, UpdateTaskRequest request, TaskService service, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.BadRequest(new { error = "Title é obrigatório." });
            }

            var updated = await service.UpdateAsync(id, request, cancellationToken);
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        group.MapDelete("/{id:int}", async (int id, TaskService service, CancellationToken cancellationToken) =>
        {
            var deleted = await service.DeleteAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });
    }
}
