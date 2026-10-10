var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ITodoService, TodoService>();
builder.Services.AddSingleton<IGuidService, GuidService>();
// builder.Services.AddScoped<IGuidService, GuidService>();
// builder.Services.AddTransient<IGuidService, GuidService>();

// builder.Services.AddSingleton<ITodoRepository, InMemoryTodoRepository>(); // 可以隨時切換 Repository 因為依賴介面所以不會有問題
builder.Services.AddSingleton<ITodoRepository, FileTodoRepository>();
var app = builder.Build();

app.MapDemoEndpoints();
app.MapTodoEndpoints();

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