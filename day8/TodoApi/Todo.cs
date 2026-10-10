public record Todo(Guid Id, string Title, bool IsDone);

public record CreateTodoRequest(string Title, bool IsDone);
