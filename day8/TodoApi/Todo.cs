public class Todo(Guid Id, string Title, bool IsDone)
{
    public Guid Id { get; } = Id;
    public string Title { get; set; } = Title;
    public bool IsDone { get; set; } = IsDone;
};

public record CreateTodoRequest(string Title, bool IsDone);
