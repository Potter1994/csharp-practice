public class Todo(Guid id, string title, bool isDone)
{
    public Guid Id { get; set; } = id;
    public string Title { get; set; } = title;
    public bool IsDone { get; set; } = isDone;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public record CreateTodoRequest(string Title, bool IsDone);
