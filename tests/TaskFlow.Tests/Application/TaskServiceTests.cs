using TaskFlow.Application.DTOs;
using TaskFlow.Application.Services;

namespace TaskFlow.Tests.Application;

public class TaskServiceTests
{
    private static TaskService CreateService(out FakeTaskRepository repository)
    {
        repository = new FakeTaskRepository();
        return new TaskService(repository);
    }

    [Fact]
    public async Task CreateAsync_AddsTaskAndReturnsResponse()
    {
        var service = CreateService(out _);

        var response = await service.CreateAsync(new CreateTaskRequest("Escrever testes", null, null));

        Assert.True(response.Id > 0);
        Assert.Equal("Escrever testes", response.Title);
        Assert.False(response.IsDone);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCreatedTasks()
    {
        var service = CreateService(out _);
        await service.CreateAsync(new CreateTaskRequest("Tarefa 1", null, null));
        await service.CreateAsync(new CreateTaskRequest("Tarefa 2", null, null));

        var all = await service.GetAllAsync();

        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WithUnknownId_ReturnsNull()
    {
        var service = CreateService(out _);

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingId_UpdatesTask()
    {
        var service = CreateService(out _);
        var created = await service.CreateAsync(new CreateTaskRequest("Original", null, null));

        var updated = await service.UpdateAsync(created.Id, new UpdateTaskRequest("Atualizado", "Desc", true, null));

        Assert.NotNull(updated);
        Assert.Equal("Atualizado", updated!.Title);
        Assert.True(updated.IsDone);
    }

    [Fact]
    public async Task UpdateAsync_WithUnknownId_ReturnsNull()
    {
        var service = CreateService(out _);

        var result = await service.UpdateAsync(999, new UpdateTaskRequest("Título", null, false, null));

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_RemovesTaskAndReturnsTrue()
    {
        var service = CreateService(out _);
        var created = await service.CreateAsync(new CreateTaskRequest("Para remover", null, null));

        var deleted = await service.DeleteAsync(created.Id);
        var afterDelete = await service.GetByIdAsync(created.Id);

        Assert.True(deleted);
        Assert.Null(afterDelete);
    }

    [Fact]
    public async Task DeleteAsync_WithUnknownId_ReturnsFalse()
    {
        var service = CreateService(out _);

        var deleted = await service.DeleteAsync(999);

        Assert.False(deleted);
    }
}
