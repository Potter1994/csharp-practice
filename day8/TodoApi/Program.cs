using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers(); // 試試用 Controller

builder.Services.AddScoped<ITodoService, TodoService>(); // 要改成 Scoped 這樣 service 在引用 EF Core 的時候才不會壞掉
// builder.Services.AddScoped<IGuidService, GuidService>();
// builder.Services.AddTransient<IGuidService, GuidService>();

// builder.Services.AddSingleton<ITodoRepository, InMemoryTodoRepository>(); // 可以隨時切換 Repository 因為依賴介面所以不會有問題
// builder.Services.AddSingleton<ITodoRepository, FileTodoRepository>();
builder.Services.AddScoped<ITodoRepository, EfTodoRepository>();

// Day11 練習三新增 AddValidation() 讓 Minimal API 可以像 MVC 一樣進行 Data Annotations 
builder.Services.AddValidation();

// Day11 練習四新增 AddProblemDetails 讓 Minimal API 可以像 MVC 一樣回傳 ProblemDetails 格式的錯誤
builder.Services.AddProblemDetails(options =>
{
  // 可以在這邊預設 Detail, 使用 ??= 則是如果沒有設置 Detail 的情況下才去, 不會影響到我有設置的時候
  options.CustomizeProblemDetails = ctx =>
  {
    ctx.ProblemDetails.Detail ??= ctx.ProblemDetails.Status switch
    {
      400 => "The request could not be understand. Check the URL and request body.",
      404 => "The requested resource was not found.",
      500 => "An unexcepted error occurred.",
      _ => null,
    };
  };
});

// Day11 練習四新增例外處理的 500 error
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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

app.UseExceptionHandler(); // 處理「丟出來的例外」 -> 500
app.UseStatusCodePages(); // 處理「狀態碼是 4xx/5xx 但 body 空的」回應 (只有 body 是空的才會用喔)
/*
五種錯誤,只有一種是例外

回應	 怎麼產生的	                 UseExceptionHandler 攔得到嗎
400   參數綁定失敗(/todos/asdf)	   綁定階段,handler 沒執行	❌
400   驗證失敗(空 Title)	        驗證 filter,handler 沒執行	❌
404   路由不存在(/foo)	          routing 階段	❌
404   資料找不到	                你的程式碼主動回傳	❌
500   資料庫連不上	              真的丟了例外	✅
*/

app.Use(async (context, next) =>
{
  if (context.Request.Path == "/boom")
  {
    throw new InvalidOperationException("測試用的爆炸");
  }
  await next();
});

/* 如果 middleware 是這個順序: 打 /boom → 例外往上穿過 UseExceptionHandler,沒人攔(Production 下又變成空白 500)❌
app.Use(...那個會爆的...);
app.UseExceptionHandler();

這才是「洋蔥包覆」的真正含意:UseExceptionHandler 只能攔到比它內層的東西,而你自己加的 middleware 是有先後的。
實務上這就是為什麼標準的註冊順序長這樣:
app.UseExceptionHandler();   // 最外層 —— 要能攔到下面全部
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();        // 位置無關,端點永遠最內層
*/

app.MapOpenApi();
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
    1. 接受介面膨脹 —— 方法數還在十幾個以內時完全可行,別過度設計
    2. Specification pattern —— 把查詢條件物件化,GetAsync(ISpecification<Todo> spec)。介面不膨脹了,但你等於自己重新發明一套查詢語言 —— 而 LINQ 本來就是
    3. 讓 Repository 回傳 IQueryable<T> —— 效能問題消失,但抽象完全漏光:上層現在直接依賴 EF Core 的查詢語意,InMemoryTodoRepository 再也不可能正確實作它。這等於承認「我只是想要一個命名空間」
    4. 分讀寫 —— 寫入走 Repository(需要封裝業務規則、Result pattern、交易邊界),查詢直接用 DbContext 或 Dapper(需要效能和彈性)。這是現在最主流的務實答案,也是 CQRS 的輕量版
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