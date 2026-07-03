namespace TaskFlow.Domain;

public class TaskItem
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public bool IsDone { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? DueDate { get; private set; }

    private TaskItem(string title, string? description, DateTime? dueDate)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        IsDone = false;
        CreatedAt = DateTime.UtcNow;
    }

    private TaskItem()
    {
        Title = string.Empty;
    }

    public static TaskItem Create(string title, string? description = null, DateTime? dueDate = null)
    {
        ValidateTitle(title);
        return new TaskItem(title, description, dueDate);
    }

    public void Update(string title, string? description, bool isDone, DateTime? dueDate)
    {
        ValidateTitle(title);
        Title = title;
        Description = description;
        IsDone = isDone;
        DueDate = dueDate;
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title é obrigatório.", nameof(title));
        }
    }
}
