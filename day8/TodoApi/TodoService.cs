/*
**練習 4:業務邏輯該放哪一層**
- 把「標題不能空白」的驗證從端點移到 `TodoService`。端點從此只做一件事:**把 Service 的結果翻譯成 HTTP 狀態碼**。
- 問題來了:`Update` 現在回傳 `bool`,但失敗有兩種原因 —— 「找不到」要回 404、「標題空白」要回 400,`bool` 分不出來。想一個辦法讓 Service 能表達「為什麼失敗」。
  (提示:可以回傳 enum、自訂的 Result 型別、或 tuple。先自己想,再查 "Result pattern")
- 這題的重點不是哪個寫法最好,是體會「**Service 不該知道 HTTP 狀態碼,但要能表達失敗原因**」。
*/

public class TodoService(ITodoRepository repo) : ITodoService
{
    public Task<IReadOnlyList<Todo>> GetAllAsync()
    {
        return repo.GetAll();
    }

    // 只是轉手就不要 async
    public Task<Todo?> GetByIdAsync(Guid id)
    {
        return repo.GetById(id);
    }

    // await 之後還要做事情才去 async
    public async Task<Result<Todo>> CreateAsync(CreateTodoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result<Todo>.Invalid("Title is required.");
        }
        Todo newTodo = new(Guid.CreateVersion7(), request.Title, request.IsDone);
        await repo.Add(newTodo);
        return Result<Todo>.Ok(newTodo);
    }
    public async Task<Result<Todo>> UpdateAsync(Guid id, CreateTodoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result<Todo>.Invalid("Title is required.");
        }

        Todo UpdatedTodo = new(id, request.Title, request.IsDone);
        bool result = await repo.Update(UpdatedTodo);

        if (!result) return Result<Todo>.NotFound("Todo is not found.");

        return Result<Todo>.Ok(UpdatedTodo);
    }
    public async Task<Result> DeleteAsync(Guid id)
    {
        bool result = await repo.Delete(id);

        if (!result) return Result.NotFound("Todo is not found.");
        return Result.Ok();
    }
}