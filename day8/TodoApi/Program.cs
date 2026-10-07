

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ITodoService, TodoService>();
builder.Services.AddSingleton<IGuidService, GuidService>();
// builder.Services.AddScoped<IGuidService, GuidService>();
// builder.Services.AddTransient<IGuidService, GuidService>();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/hello", async (HttpContext context, /*[FromQuery] 可以寫在前面更清楚*/ string name) =>
{
    Console.WriteLine($"QueryString 也能直接寫在參數取得 name: {name}");
    foreach (var key in context.Request.Query.Keys)
    {
        await context.Response.WriteAsync($"{key}: {context.Request.Query[key]}\r\n");
    }
});

app.MapGet("/hello/{name}", (string? name) => $"Hello {name}");

/*
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
*/

app.MapGet("/todos", (ITodoService service) =>
{
    return Results.Ok(service.GetAll());
});

app.MapGet("/todos/{id}", (Guid id, ITodoService service) =>
{
    var result = service.GetById(id);
    if (result == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(result);

});

app.MapPost("/todos", (CreateTodoRequest request, ITodoService service) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest(new { message = "Title is required." });
    }

    Todo newTodo = service.Create(request);
    return Results.Created($"/todos/{newTodo.Id}", newTodo);
});

app.MapPut("/todos/{id}", (Guid id, CreateTodoRequest request, ITodoService service) =>
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

app.MapDelete("/todos/{id}", (Guid id, ITodoService service) =>
{
    var result = service.Delete(id);

    if (!result)
    {
        return Results.NotFound(new { message = "Todo id is not found." });
    }

    return Results.NoContent();
});


app.MapGet("/guid", (IGuidService service1, IGuidService service2) =>
{
    return new
    {
        guid1 = service1.Id,
        guid2 = service2.Id,
        same = service1.Id == service2.Id,
    };
});

app.Run();

/*
**練習 4:DI 與服務分離**
- 把 `List<Todo>` 的操作抽成 `ITodoService` + `TodoService`,端點只負責接請求、回傳結果。
- 用 `builder.Services.AddSingleton<ITodoService, TodoService>()` 註冊,端點用參數注入取得。
- **實測三種生命週期的差別**:做一個 `IGuidService` 只回傳一個建構時產生的 `Guid`,分別註冊成 Singleton / Scoped / Transient,在同一個端點注入兩次並印出來,觀察:
  - 同一個請求內兩個 Guid 是否相同?
    Singleton 跟 Scoped 的 Guid 在同一個端點注入想次會是一樣的(取得同一個 service)
    Transient 則是不一樣的

  - 不同請求之間是否相同?
    不同請求之間只有 Singleton 還是相同的

  - 記下結果,解釋為什麼。
    Singleton 是只要程式沒有關閉就都會是同一個
    Scoped 則是只要是同一次請求都會是同一個
    Transient 每次建立都是新的

            同一請求內兩次         跨請求
Singleton	1ab448 = 1ab448	    1ab448 → 1ab448 相同
Scoped	    3186d5 = 3186d5	    3186d5 → c06db5 不同
Transient	25a35f ≠ ffc81b	    全都不同
*/


public record CreateTodoRequest(string Title, bool IsDone);