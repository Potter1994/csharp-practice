/*
Day 8
**練習 3:Todo CRUD(本日主線)**
先定義 `record Todo(int Id, string Title, bool IsDone);`,資料先存在記憶體的 `List<Todo>`(資料庫留到 Day 10)。
實作五個端點,**狀態碼要正確**:

| 方法 | 路徑 | 成功回傳 | 失敗回傳 |
|---|---|---|---|
| `GET` | `/todos` | 200 + 陣列 | — |
| `GET` | `/todos/{id}` | 200 + 單筆 | 404 |
| `POST` | `/todos` | **201** + `Location` header | 400(標題空白) |
| `PUT` | `/todos/{id}` | 204 No Content | 404 |
| `DELETE` | `/todos/{id}` | 204 No Content | 404 |

- 用 `Results.Ok()` / `Results.Created()` / `Results.NotFound()` / `Results.NoContent()` / `Results.BadRequest()`。
- 對照 Day 6 寫的冪等性筆記:驗證 `PUT` 同一筆打兩次結果相同、`POST` 打兩

搭配 Day 9 
**練習 3:端點搬出 `Program.cs`**
- 寫一個擴充方法 `public static void MapTodoEndpoints(this WebApplication app)`,把五個端點搬進去(放在 `Endpoints/TodoEndpoints.cs`)。
- 用 `app.MapGroup("/todos")` 收斂共同前綴,端點路徑只剩 `""` 和 `"/{id}"`。
- 目標:`Program.cs` 回到 10 行以內。

*/

public static class TodoEndpoints
{
    // 我沒改, 讓自己下次能看懂在說什麼
    // 這邊的 WebApplication 如果改成介面當參數: IEndpointRouteBuilder
    // 這樣底下的 MapGroup 之後的也能直接使用這邊舉例的 MapTodoEndpoints
    // 因為 IEndpointRouteBuilder 跟 WebApplication 都有實作介面 IEndpointRouteBuilder
    internal static void MapTodoEndpoints(this WebApplication /*IEndpointRouteBuilder*/ app)
    {
        // - 用 `app.MapGroup("/todos")` 收斂共同前綴,端點路徑只剩 `""` 和 `"/{id}"`。
        RouteGroupBuilder group = app.MapGroup("/todos");

        group.MapGet("", (ITodoService service) => Results.Ok(service.GetAll()));

        group.MapGet("/{id}", (Guid id, ITodoService service) =>
        {
            var result = service.GetById(id);
            if (result == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(result);

        });

        group.MapPost("", (CreateTodoRequest request, ITodoService service) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.BadRequest(new { message = "Title is required." });
            }

            Todo newTodo = service.Create(request);
            return Results.Created($"/todos/{newTodo.Id}", newTodo);
        });

        group.MapPut("/{id}", (Guid id, CreateTodoRequest request, ITodoService service) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.BadRequest(new { message = "Title is required." });
            }

            var result = service.Update(id, request);

            if (!result)
            {
                return Results.NotFound(new { message = "Todo is not found." });
            }

            return Results.NoContent();
        });

        group.MapDelete("/{id}", (Guid id, ITodoService service) =>
        {
            var result = service.Delete(id);

            if (!result)
            {
                return Results.NotFound(new { message = "Todo id is not found." });
            }

            return Results.NoContent();
        });
    }

}