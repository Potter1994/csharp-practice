public class TodoService : ITodoService
{
    private readonly List<Todo> _todoList = [
        new Todo(Guid.CreateVersion7(), "Test 1", false),
        new Todo(Guid.CreateVersion7(), "Test 2", false),
        new Todo(Guid.CreateVersion7(), "Test 3", true),
        new Todo(Guid.CreateVersion7(), "Test 4", false),
    ];
    private readonly object _lock = new();

    public IReadOnlyList<Todo> GetAll()
    {
        // [.. _todoList] 相當於 _todoList.ToList();
        lock (_lock) return [.. _todoList];
    }

    public Todo? GetById(Guid id)
    {
        lock (_lock) return _todoList.FirstOrDefault(todo => todo.Id == id);
    }
    public Todo Create(CreateTodoRequest request)
    {
        Todo newTodo = new Todo(Guid.CreateVersion7(), request.Title, request.IsDone);
        lock (_lock)
        {
            _todoList.Add(newTodo);
        }
        return newTodo;
    }
    public bool Update(Guid id, CreateTodoRequest request)
    {
        lock (_lock)
        {
            var index = _todoList.FindIndex(todo => todo.Id == id);
            if (index == -1) return false;
            _todoList[index] = _todoList[index] with { Title = request.Title, IsDone = request.IsDone };
            return true;
        }
    }
    public bool Delete(Guid id)
    {
        lock (_lock)
        {
            var index = _todoList.FindIndex(todo => todo.Id == id);
            if (index == -1) return false;
            _todoList.RemoveAt(index);
            return true;
        }
    }
}