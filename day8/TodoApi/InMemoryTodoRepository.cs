public class InMemoryTodoRepository : ITodoRepository
{
    private readonly List<Todo> _todos = [];

    // 為什麼這邊要 lock 是為了 CPU 有可能在同時處離更新順序時拿到寫到一半的 _todos 導致取得 null 的可能
    // 之後使用 database 就不會有這個問題了(database 自己處理好這些情況了)

    // lock 為什麼該在 Repository 而不是 Service?
    // 因為在 Repository 才是對資料做改動的時候
    // 鎖要跟「他保護的那份資料」放在一起
    private readonly object _lock = new();

    public Task AddAsync(Todo todo)
    {
        lock (_lock)
        {
            _todos.Add(todo);
            return Task.CompletedTask;
        }
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        lock (_lock)
        {
            int index = _todos.FindIndex(todo => todo.Id == id);
            if (index == -1) return Task.FromResult(false);
            _todos.RemoveAt(index);
            return Task.FromResult(true);
        }
    }

    public Task<Todo?> GetByIdAsync(Guid id)
    {
        lock (_lock)
        {
            return Task.FromResult(_todos.FirstOrDefault(todo => todo.Id == id));
        }
    }

    public Task<IReadOnlyList<Todo>> GetAllAsync()
    {
        lock (_lock)
        {
            return Task.FromResult<IReadOnlyList<Todo>>([.. _todos]);
        }
    }

    public Task<bool> UpdateAsync(Todo todo)
    {
        lock (_lock)
        {
            int index = _todos.FindIndex(t => t.Id == todo.Id);
            if (index == -1) return Task.FromResult(false);
            _todos[index] = _todos[index] with { Title = todo.Title, IsDone = todo.IsDone };
            return Task.FromResult(true);
        }
    }
}