using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(); // 試試用 Controller

builder.Services.AddScoped<ITodoService, TodoService>(); // 要改成 Scoped 這樣 service 在引用 EF Core 的時候才不會壞掉
// builder.Services.AddScoped<IGuidService, GuidService>();
// builder.Services.AddTransient<IGuidService, GuidService>();

// builder.Services.AddSingleton<ITodoRepository, InMemoryTodoRepository>(); // 可以隨時切換 Repository 因為依賴介面所以不會有問題
// builder.Services.AddSingleton<ITodoRepository, FileTodoRepository>();
builder.Services.AddScoped<ITodoRepository, EfTodoRepository>();

/*
=== AddDbContext 一行，實際註冊了 5 個服務 ===
   Singleton ServiceProviderAccessor
   Scoped    IDbContextOptionsConfiguration`1
   Scoped    DbContextOptions`1
   Scoped    DbContextOptions
   Scoped    AppDbContext              ← 你問的那個
*/
var conn = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("application.json 缺少 ConnectionString:Default");
builder.Services.AddDbContext<AppDbContext>(opt =>
{
  opt.UseNpgsql(conn);

  if (builder.Environment.IsDevelopment())
  {
    opt.EnableSensitiveDataLogging();
  }
});
var app = builder.Build();

app.MapControllers();
// app.MapDemoEndpoints();
app.MapTodoEndpoints();

/*
**練習 5(整合小專案):看見 EF Core 真正送出的 SQL**
- 在 `appsettings.Development.json` 把 EF Core 的 log 等級開到 `Information`,讓它印出每一句 SQL。
- 打一次 `GET /todos`,看它送出什麼 SQL。
    Executed DbCommand (1ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
    SELECT t."Id", t."CreatedAt", t."IsDone", t."Title"
    FROM "Todos" AS t
    ORDER BY t."Id"

- 然後做這個對照實驗,**這題是 Day 9 延伸題的答案**:
  - A:走你的 Repository —— `GetAllAsync()` 之後在記憶體裡 `.Where(t => !t.IsDone).Select(t => t.Title).Take(10)`
      Executed DbCommand (12ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT t."Id", t."CreatedAt", t."IsDone", t."Title"
      FROM "Todos" AS t
      ORDER BY t."Id"

  - B:直接用 `DbSet` —— `db.Todos.Where(t => !t.IsDone).Select(t => t.Title).Take(10).ToListAsync()`
      Executed DbCommand (7ms) [Parameters=[@p='10'], CommandType='Text', CommandTimeout='30']
      SELECT t."Title"
      FROM "Todos" AS t
      WHERE NOT (t."IsDone")
      LIMIT @p

  - 比較兩者送出的 SQL 有什麼不同,資料量大時差在哪。
    A 則是拿回了全部, 在程式端執行剩下的 LINQ 
    而 B 所有的查詢、篩選、Select 都直接在 SQL 那邊就做完了

- 寫下你對「EF Core 上面要不要再包 Repository」的看法(Day 9 延伸題欠的那題)。
    我終於明白你的意思了, 我覺得可能還是要包 Repository 會更好一點,
    如果真的有很多不一樣的資料操作可能就是一直新增方法到 ITodoRepository 來讓 Repository 實作是不錯的解決方法
*/
app.MapGet("/repository-test", async (ITodoRepository repo) =>
{
  var todos = await repo.GetAllAsync();
  var result = todos.Where(t => !t.IsDone).Select(t => t.Title).Take(10);
  return Results.Ok(result);
});

app.MapGet("/db-test", async (AppDbContext db) =>
{
  // 這裡需要 ToListAsync() 是因為 DbContext 在序列化前就隨請求結束被釋放,你會拿到 ObjectDisposedException
  // 這是 ASP.NET Core + EF Core 的經典 bug 補一個 .ToList() or .ToListAsync() 就能永遠避開。
  var result = await db.Todos.Where(t => !t.IsDone).Select(t => t.Title).Take(10).ToListAsync();
  return Results.Ok(result);
});

app.MapGet("/identity-test", async (AppDbContext db) =>
{
  Guid id = (await db.Todos.AsNoTracking().FirstAsync()).Id;

  var a1 = await db.Todos.FirstOrDefaultAsync(t => t.Id == id);
  var a2 = await db.Todos.FirstOrDefaultAsync(t => t.Id == id);

  var b1 = await db.Todos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
  var b2 = await db.Todos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

  return Results.Ok(new
  {
    追蹤模式_同一個物件 = ReferenceEquals(a1, a2),
    不追蹤_同一個物件 = ReferenceEquals(b1, b2),
    追蹤模式查到幾個實體 = db.ChangeTracker.Entries<Todo>().Count(),
  });
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