**練習 1:概念暖身(先寫筆記再寫 code)**
用自己的話回答:
- `Program.cs` 裡 `WebApplication.CreateBuilder(args)` → `builder.Build()` → `app.Run()` 這三步各做了什麼?
    `WebApplication.CreateBuilder(args)`: 使用基本預設的設置建立起一個建立者
    `CreateBuilder` 做的事比想像多:讀 appsettings.json + 環境變數 + 命令列、建立 DI 容器(實測 111 個預設服務)、設定 logging 和 Kestrel
    
    `builder.Build()`: 是分界線 —— 之後不能再註冊服務(跟 Day 6 的 HttpClient.Timeout 同一種「凍結」設計)
    `app.Run()`: 會阻塞 —— 這就是 dotnet run 不會自己結束的原因

- 什麼是 **middleware pipeline**?為什麼 `app.UseXxx()` 的**順序**會影響行為?(提示:想像成一層層包起來的洋蔥,請求進去、回應出來)
    `middleware pipeline`: 就是要一關接一關持續流下去, 照著順序執行所有的中間件
    為什麼順序會影響行為: 例如有些中間件會去解析 CORS, 然後後面使用到的才能正常使用, 所以順序很重要

    [A] 請求進來 → [B] 請求進來 → [端點] → [B] 回應出去 → [A] 回應出去
    計時、日誌、例外處理這類 middleware 全靠 await next() 之後的那段才做得到。你的 CORS 例子方向對,但更好懂的是:驗證身分必須排在授權檢查之前,寫反就變成「還不知道你是誰就檢查你有沒有權限」。

- 什麼是 **DI(依賴注入)容器**?為什麼不自己 `new` 就好?
    DI(依賴注入)容器: DI 容器就是註冊一次, 到處注入, 端點完全不知道 `TodoService` 怎麼來的
    `builder.Services.AddSingleton<ITodoService, TodoService>()` + 端點用 `(ITodoService service)` 取得

    為什麼不自己 new:端點不該知道資料存在哪,換成 EF Core 時端點一行都不用改


- `AddSingleton` / `AddScoped` / `AddTransient` 三種生命週期差在哪?各自適合什麼?
    `AddSingleton`: 生命週期為整個應用結束才會關閉
    `AddScoped`: 生命週期為同一個 request 結束後才會關閉
    `AddTransient`: 每次被要求注入時都給一個新的實例

    Singleton 適合無狀態的服務;有可變狀態就得自己處理執行緒安全
    Scoped 適合 DbContext;Singleton 不能注入 Scoped

    不是你 new,是容器在「有人要求注入」時決定要不要給一個新實例。

- Minimal API 跟 Controller 寫法差在哪?各自適合什麼場景?
    Minimal API 寫法跟 Nodejs + express 的寫法很像
    Minimal API 適合寫在小專案, Controller 適合寫在大型且複雜的專案
    但我沒寫過 Controller 的寫法所以還不太清楚


- 對照 Day 6:你那時候是 HTTP **客戶端**,現在是**伺服器端**。同一個請求在兩邊分別經過什麼?
    客戶端一樣
    DNS -> TCP -> TLS -> HTTP request -> response -> render

    server 端
    Kestrel 收連線 → 解析 HTTP → 建立 HttpContext
    → middleware pipeline（去程）
        → Routing 比對出端點 → Model Binding → 執行你的 lambda
    → return IResult → ExecuteAsync 寫入 StatusCode + Header + Body
    → middleware pipeline（回程）
    → 寫回 socket


    - 對照 Day 6 寫的冪等性筆記:驗證 `PUT` 同一筆打兩次結果相同、`POST` 打兩次會建出兩筆。
        冪等: 不管執行幾次, 結果一樣(可以安心重試)
        GET / PUT / DELETE: 冪等, 可以安心重試
        POST: 不冪等, 可能建立出兩筆以上重複資料

        POST 打兩次(相同內容)
        第一次 id: 01a1170c-2dcf-77f0-...
        第二次 id: 01a1170c-2dd9-734d-...     ← 不同 id
        「重複測試」有 2 筆  → 不冪等 ✓

        PUT 同一筆打三次
        第 1 次後: {"title":"最終標題","isDone":true}
        第 2 次後: {"title":"最終標題","isDone":true}     ← 完全相同
        第 3 次後: {"title":"最終標題","isDone":true}
        → 冪等 ✓