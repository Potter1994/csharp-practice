public class InMemoryTodoRepository : ITodoRepository
{
    private readonly List<Todo> _todos = [];

    // 為什麼這邊要 lock 是為了 CPU 有可能在同時處離更新順序時拿到寫到一半的 _todos 導致取得 null 的可能
    // 之後使用 database 就不會有這個問題了(database 自己處理好這些情況了)

    // lock 為什麼該在 Repository 而不是 Service?
    // 因為在 Repository 才是對資料做改動的時候
    // 鎖要跟「他保護的那份資料」放在一起
    private readonly object _lock = new();
    public void Add(Todo todo)
    {
        lock (_lock)
        {
            _todos.Add(todo);
        }
    }

    public bool Delete(Guid id)
    {
        lock (_lock)
        {
            int index = _todos.FindIndex(todo => todo.Id == id);
            if (index == -1) return false;
            _todos.RemoveAt(index);
            return true;
        }
    }

    public Todo? GetById(Guid id)
    {
        lock (_lock)
        {
            return _todos.FirstOrDefault(todo => todo.Id == id);
        }
    }

    public IReadOnlyList<Todo> GetAll()
    {
        lock (_lock)
        {
            return [.. _todos];
        }
    }

    public bool Update(Todo todo)
    {
        lock (_lock)
        {
            int index = _todos.FindIndex(t => t.Id == todo.Id);
            if (index == -1) return false;
            _todos[index] = _todos[index] with { Title = todo.Title, IsDone = todo.IsDone };
            return true;
        }
    }
}