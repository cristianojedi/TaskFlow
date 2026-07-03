using TaskFlow.Domain;

namespace TaskFlow.Tests.Domain;

public class TaskItemTests
{
    [Fact]
    public void Create_WithValidTitle_SetsDefaults()
    {
        var task = TaskItem.Create("Estudar EF Core");

        Assert.Equal("Estudar EF Core", task.Title);
        Assert.False(task.IsDone);
        Assert.Null(task.Description);
        Assert.Null(task.DueDate);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithoutTitle_ThrowsArgumentException(string? title)
    {
        Assert.Throws<ArgumentException>(() => TaskItem.Create(title!));
    }

    [Fact]
    public void Update_WithValidTitle_UpdatesFields()
    {
        var task = TaskItem.Create("Título original");

        task.Update("Título atualizado", "Nova descrição", true, null);

        Assert.Equal("Título atualizado", task.Title);
        Assert.Equal("Nova descrição", task.Description);
        Assert.True(task.IsDone);
    }

    [Fact]
    public void Update_WithoutTitle_ThrowsArgumentException()
    {
        var task = TaskItem.Create("Título original");

        Assert.Throws<ArgumentException>(() => task.Update("", null, false, null));
    }
}
