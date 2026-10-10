// /*
// **練習 5(整合小專案):換一個 Repository 實作**
// - 寫 `FileTodoRepository`,把資料存成 JSON 檔(用你 Day 6 的 `System.Text.Json`)。
// - **只改 `Program.cs` 註冊的那一行**切換實作,其他程式碼一個字都不能動。
// - 驗證:重啟程式後資料還在;再切回 `InMemory`,行為一樣正確。
// - 這就是 Day 10 換成 EF Core 時會做的事 —— 介面不變,換掉實作。
// */

using System.Text.Encodings.Web;
using System.Text.Json;

public class FileTodoRepository : ITodoRepository
{
    static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };
    static readonly string TodoPath = "./todos.json";
    private readonly SemaphoreSlim _gate = new(1, 1);

    public FileTodoRepository()
    {
        if (!File.Exists(TodoPath))
        {
            File.WriteAllText(TodoPath, "[]");
        }
    }

    public async Task AddAsync(Todo todo)
    {
        await _gate.WaitAsync();
        try
        {
            List<Todo> todoList = await GetTodosFromJson();
            todoList.Add(todo);
            await WriteToLocalJsonFile(todoList);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await _gate.WaitAsync();
        try
        {
            List<Todo> todoList = await GetTodosFromJson();
            int index = todoList.FindIndex(todo => todo.Id == id);
            if (index == -1) return false;
            todoList.RemoveAt(index);
            await WriteToLocalJsonFile(todoList);
            return true;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IReadOnlyList<Todo>> GetAllAsync()
    {
        await _gate.WaitAsync();
        try
        {
            return [.. await GetTodosFromJson()];
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<Todo?> GetByIdAsync(Guid id)
    {
        await _gate.WaitAsync();
        try
        {
            List<Todo> todoList = await GetTodosFromJson();
            return todoList.FirstOrDefault(todo => todo.Id == id);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<bool> UpdateAsync(Todo todo)
    {
        await _gate.WaitAsync();
        try
        {
            List<Todo> todoList = await GetTodosFromJson();
            int index = todoList.FindIndex(t => t.Id == todo.Id);
            if (index == -1) return false;
            todoList[index] = todoList[index] with { Title = todo.Title, IsDone = todo.IsDone };
            await WriteToLocalJsonFile(todoList);
            return true;
        }
        finally
        {
            _gate.Release();
        }
    }

    // 輔助方法用 private 不是用 internal
    private async Task<List<Todo>> GetTodosFromJson()
    {
        string content = await File.ReadAllTextAsync(TodoPath);
        return JsonSerializer.Deserialize<List<Todo>>(content, Options) ?? [];
    }

    private async Task WriteToLocalJsonFile(List<Todo> todoList)
    {
        string content = JsonSerializer.Serialize(todoList, Options);
        await File.WriteAllTextAsync(TodoPath, content);
    }
}