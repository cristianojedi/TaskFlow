using System.Net;
using System.Net.Http.Json;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Tests.Integration;

public class TaskEndpointsTests : IClassFixture<TaskFlowApiFactory>
{
    private readonly HttpClient _client;

    public TaskEndpointsTests(TaskFlowApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostTasks_WithValidRequest_ReturnsCreated()
    {
        var request = new CreateTaskRequest("Tarefa de integração", "descrição", null);

        var response = await _client.PostAsJsonAsync("/tasks", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(body);
        Assert.Equal("Tarefa de integração", body!.Title);
        Assert.True(body.Id > 0);
    }

    [Fact]
    public async Task GetTaskById_AfterCreate_ReturnsTask()
    {
        var createResponse = await _client.PostAsJsonAsync("/tasks", new CreateTaskRequest("Buscar depois", null, null));
        var created = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();

        var getResponse = await _client.GetAsync($"/tasks/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetched = await getResponse.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal("Buscar depois", fetched.Title);
    }

    [Fact]
    public async Task GetTaskById_WithUnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/tasks/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
