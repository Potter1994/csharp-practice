**練習 1:概念暖身(先寫筆記再動手)**
- **DTO** 是什麼?為什麼不該直接把 `Todo` 當成 API 的輸入與輸出?
  (提示:你現在的 `GET /todos` 會把 `CreatedAt` 吐給前端。如果哪天 `Todo` 多了 `OwnerId`、`InternalNote`,或 Day 12 加了密碼雜湊欄位,會發生什麼事?)
    <!-- 因為直接轉 `Todo` 可能會把不必要給的資訊或想保密的資訊給出去, 所以乾脆 DTO 轉成我想給的資料格式就好 -->
    DTO 三個理由按重要性排: 解耦(主要)、限制輸入面(安全)、隱藏欄位(我回答的)
    主要理由解除耦合: 讓 API 契約和資料庫 schema 各自獨立演進(假設我資料庫把 Title 改成了 Name 反而變成了資料庫破壞 API 變更)
    限制輸入面: 如果 `POST /todos` 直接綁定 Todo
    ```
    { "title": "買牛奶", "id": "任意值", "createdAt": "1999-01-01" }
    ```
    模型會乖乖把這些值丟進去(id 被客戶端控制了), 這叫 over-posting(或 mass assignment)


- 你其實已經有一個 DTO 了 —— `CreateTodoRequest`。想兩件事:為什麼它**不該**有 `Id`?為什麼它有 `IsDone` 其實有點可疑?
    <!-- 因為我是要新增 Todo 的標題跟是否已經完成, 而 Id 這種要產生唯一值的交給我 backend 去做就好 -->
    建立一個代辦的事項, 不應該控制他是否已經完成, 語意是矛盾的, 「未完成 -> 完成」應該只能透過 `PATCH /todos/{id}`,現在 POST 也能直接跳到完成狀態。
    所以 `CreateTodoRequest` 應該只有一個欄位 `Title`, `IsDone` 在 `TodoService.CreateAsync` 裡固定為 `false`
    DTO 該包含的是「這個操作真正需要的輸入」


- **驗證該放在哪一層?** 你現在的 `IsNullOrWhiteSpace` 檢查在 `TodoService`。下面三種規則是同一類東西嗎?
  - 「Title 不能空白」「Title 不能超過 200 字」
    <!-- 這個限制為寫入 SQL 的規則, 但是應該在 Service 就先檢查好, 而不是直接送入 SQL 再返回錯誤 -->
    我把它理解成「資料庫約束的前置檢查, 但他們是兩件獨立的事, 不是一前一後。
    這是格式驗證, 正確位置是最外層的 API 邊界(不是 Service), 理由:
    1. 他只看請求本身就能判斷 —— 不需查資料庫、不需要任何業務狀態
    2. 他是 API 契約的一部份, 應該出現在 OpenAPI 文件裡, 讓客戶端送出之前就知道規則
    3. 壞資料越早拒絕越好, 不該讓他穿過 Service 層

    而資料庫的 `NOT NULL`/ 長度限制是最後一道防線, 目的完全不同 —— 他保護的是資料完整性, 防止任何來源(包括直接跑 SQL 的人、資料修補腳本)寫入壞資料。
    我「不要靠資料庫報錯來驗證」的直覺是對的, 但理由不是效率 —— 而是資料庫的錯誤訊息無法變成好的 API 回應。 PostgreSQL 會回 `value too long for type character varying(200)`, 這對前端毫無用處, 而且會以 500 的形式出現

  - 「已完成的 Todo 不能再改標題」
    這個是業務邏輯, 寫在 Service 沒錯
    完全正確, 補充幾點
    1. 無法只看請求判斷 -> 不是格式驗證
    2. 需要先讀資料 -> 必須在能存取 Repository 的層

  - 「同一個使用者不能有兩筆同名的 Todo」
    <!-- 這個要將 Title 設為 primary key, 這樣就是屬於 SQL 的規則了, 這個邏輯不知道該怎麼寫比較好, 也不知是不是屬於 Service -->
    完全錯誤, 有四個問題
    1. 規則是「同一個使用者範圍內」唯一, 不是全局唯一 -> 所以至少要`(OwnerId, Title)`兩欄一起, 不可能是 `Title`單獨
    2. primary key ≠ unique constraint -> 我混淆了這兩個, 主鍵是「這一列的身份」, 唯一約束是「這幾欄的組合不能重複」。一張表只能有一個主鍵, 但可以有任意多個唯一的約束
    3. `Id` 已經是主鍵了, 不能再設一個
    4. 業務上也不該 —— `Title` 是使用者可以隨時修改的文字。拿他當主鍵意味者「改標題」等於「換成另一個實體, 所有指向他的外鍵都要跟著改

    正確做法是唯一索引, 這是 「Fluent API 真正不可或缺的場合」 的例子之一:
    ```csharp
    entity.HasIndex(t => new { t.OwnerId, t.Title }).IsUnique();
    ```

    「不知道該怎麼寫比較好」的部分,是這題最深的地方
    這叫資料完整性驗證, 他的特殊之處是: 無法只靠應用層保證他。
    兩個並行請求可以同時通過檢查,然後都寫入。這是 TOCTOU —— 你 Day 8 已經踩過兩次了:200 個並行 POST 掉 4 筆資料、以及 FindIndex 寫在 lock 外面炸出 14 次 NullReferenceException。完全同一個問題,只是這次換到了資料庫層面,而且不能用 lock 解決(多個應用程式實例各有自己的鎖)。

    所以正確答案是雙層分工:
    層	                做什麼	                    為什麼
資料庫唯一索引	        唯一真正的保證	       資料庫在寫入時原子地檢查,沒有競態空隙
應用層預先檢查	        只為了錯誤訊息品質	    讓一般情況回 409 Conflict 而不是 500
捕捉唯一約束違反例外	 補漏網的競態	        預檢通過但實際寫入衝突時,翻譯成 409

- 查 **ProblemDetails**(RFC 9457,舊編號 RFC 7807)。它解決什麼問題?為什麼需要一個「標準的錯誤回應格式」?
    他解決了 API 的一致性, 因為這樣方便 client 端統一取得錯誤的回應格式訊息
    對。補三件你該知道的具體內容:

    欄位結構(RFC 9457 定義的)
    ```csharp
    {
    "type": "https://example.com/probs/out-of-credit",
    "title": "You do not have enough credit.",
    "status": 403,
    "detail": "Your current balance is 30, but that costs 50.",
    "instance": "/account/12345/msgs/abc"
    }
    ```
    一個常見誤用:type 應該是 URI(通常指向說明文件),不是錯誤代碼字串。

    1. 獨立的 media type:回應的 Content-Type 是 application/problem+json,不是 application/json。客戶端可以據此判斷「這是個標準錯誤物件」而不用先猜。
    2. 為什麼需要標準而不是自己定:關鍵在於你的 API 不是客戶端唯一要對接的 API。如果每個服務都發明自己的錯誤格式,客戶端就得為每個服務寫一套錯誤處理。標準格式讓通用的錯誤處理中介層、SDK、監控工具成為可能。
    3. 另外一個你練習 3 會馬上看到的:ASP.NET Core 有個擴充版 ValidationProblemDetails,多了一個 errors 字典(欄位名 → 錯誤訊息陣列)。那是驗證失敗時的標準回應。


- 回頭看 Day 6 的冪等性筆記:`PUT` 跟 `PATCH` 的語意差在哪?你現在的 `PUT /todos/{id}` 如果只想改 `IsDone` 一個欄位,用 `PUT` 恰當嗎?
    `PUT`: 完整替換
    `PATCH`: 只修改需要的

    PUT 必然冪等 —— 它是「設定絕對值」
    PATCH 不保證冪等 —— 取決於 patch 的內容

    如果只想改 `IsDone` 使用 `PATCH` 是更好的選擇



**練習 2:拆出 Response DTO**
- 新增 `TodoResponse`,只包含前端需要的欄位(排除了 `CreatedAt` —— 這是刻意的決定,不是漏掉)。

- **轉換放在哪一層?** 一開始選了 Service,改成放在 Endpoint。
    最初的推理是消去法:「Endpoint 負責 HTTP、Repository 負責資料存取,所以剩下 Service」。
    那個推理的漏洞是 —— **轉換 DTO 本身就是 HTTP 層的工作**。`TodoResponse` 存在的唯一理由是「HTTP 客戶端要看到什麼」,跟業務邏輯無關。
    Endpoint 的職責不只是「翻譯狀態碼」,它負責整個 HTTP 表述層:狀態碼、header、以及 **body 的形狀**。

- 如果 `ITodoService` 回傳 `Result<TodoResponse>`,就跟 Day 9 修掉的問題是同一個,只是方向相反:
    ```
    Day 9 修掉:    API 輸入格式(CreateTodoRequest)  →  滲進 Repository 介面
    Day 11 要避免: API 輸出格式(TodoResponse)       →  滲進 Service 介面
    ```
    三個具體代價:
    1. `ITodoService` 再也不能服務別的傳輸方式(gRPC、CLI、背景作業拿到的都是 HTTP 回應格式)
    2. Service 內部會資訊損失 —— 如果 `CreateAsync` 在 `await repo.AddAsync()` 之後還要做事(寄通知、寫 audit log 需要 `CreatedAt`),手上已經只有 `TodoResponse`
    3. Day 13 寫單元測試時,斷言的會是 API 回應格式而不是領域行為

- **怎麼避免兩套端點重複寫轉換** —— 用 Day 1 學的擴充方法,而不是把轉換推進 Service:
    ```csharp
    public static class TodoMappingExtensions
    {
        public static TodoResponse ToResponse(this Todo todo) => new(todo.Id, todo.Title, todo.IsDone);
    }
    ```
    四件事同時成立:`ITodoService` 繼續回傳 `Result<Todo>`、轉換只有一份、兩套端點都能用、`TodoResponse` 和 mapping 都住在 API 層。

- `TodoResponse` 用 `record` 而不是 `class`:
    `Todo` 之所以從 `record` 改成 class 是為了 EF Core 的 change tracking(Day 10 踩過 `with` 的坑)。DTO 沒有這個需求 —— 建立之後只會被序列化,**不可變才是正確語意**。

- **`PUT`/`DELETE` 不需要 `.ToResponse()`** —— 它們回 204 No Content,根本沒有 body 要轉換。

- **加一個 `InternalNote` 欄位會洩漏嗎?** 不會,因為 `TodoResponse` 裡沒有這個欄位。
    關鍵是**白名單 vs 黑名單**的差別:
    ```
    靠 private / [JsonIgnore] 隱藏  →  黑名單:預設公開,靠記得標注來藏
    用 TodoResponse 挑選欄位        →  白名單:沒寫進 DTO 就結構上不可能出去
    ```
    對照 Day 10 加 `CreatedAt` 的經驗:那次它**自動**出現在所有 API 回應裡,因為當時沒有白名單。
    (附帶修正一個誤解:「private 欄位不會洩漏」碰巧成立,但理由是錯的。`private` 控制的是 C# 存取權限,不是 API 可見性;而 `InternalNote` 一定會是 `public`,因為 Service 要讀寫它。)


**練習 3:輸入驗證**
- DTO 依操作拆開,並把手寫的 `IsNullOrWhiteSpace` 換成宣告式驗證:
    ```csharp
    public record CreateTodoRequest([Required][StringLength(20)] string Title);
    public record UpdateTodoRequest([Required][StringLength(20)] string Title, [Required] bool? IsDone);
    public record SetDoneRequest([Required] bool? IsDone);
    ```
    `CreateTodoRequest` 沒有 `IsDone` —— 新建的待辦必然未完成,讓客戶端指定在語意上矛盾,而且會多開一條狀態轉換的路徑。

- **兩套端點對驗證的處理歷來不同**(這是本題最大的發現):
    ```
    MVC:          路由 → 模型綁定 → 模型驗證 → [ApiController] 檢查 ModelState → action
    Minimal API:  路由 → 參數綁定 → handler
    ```
    Minimal API **沒有「模型驗證」這個階段**,所以 Data Annotations 完全被忽略。
    實測:同一個空 Title,`/api/todos` 回 `ValidationProblemDetails`,`/todos` 卻一路跑到 Service 才被 `IsNullOrWhiteSpace` 擋下。
    而且 `request.Title` 在 Minimal API 裡**真的是 `null`**,儘管宣告是 non-nullable `string` —— 因為 `System.Text.Json` 完全不看 nullable 註記。
    解法是 .NET 10 的 `builder.Services.AddValidation()`,它用 **endpoint filter** 實作(所以不需要配 `app.UseXxx()`)。

- **沒寫 `[Required]` 也會擋空值** —— 原因是 nullable reference types:
    `<Nullable>enable</Nullable>` 之下,ASP.NET Core 把 non-nullable 的 `string` **隱式**當成必填。
    也就是說 Controller 從 Day 9 寫完那天就已經有驗證了,只是一直沒送空值去測所以沒發現。
    另外 `RequiredAttribute` 內部會先 `Trim()` 再檢查,所以 `""` 和 `"   "` 都會被擋 —— 行為跟 `IsNullOrWhiteSpace` 完全一樣。
    但它**不管長度**,所以 `[StringLength(20)]` 還是得自己加。

- **`[Required]` 加在 `bool` 上完全無效**(實測確認):
    `RequiredAttribute` 檢查的是「值是不是 null」,而 `bool` 永遠不是 null → 永遠通過,寫了跟沒寫一樣。
    ```
    bool IsDone              →  [Required] 無效,沒傳就填 false
    bool? IsDone             →  沒有 required,null 通過
    [Required] bool? IsDone  →  ✅ 有效,沒傳 → null → 400
    ```
    要讓布林值真正必填,必須先讓它**能夠**是 null。

- **實值型別無法分辨「沒傳」和「傳了預設值」**(這點對 PATCH 致命):
    ```
    {"title":"買牛奶"}                   →  IsDone = false
    {"title":"買牛奶","isDone":false}    →  IsDone = false     ← 綁定結果完全相同
    ```
    精確的機制:**不是綁定器主動填 default,而是它根本沒碰那個屬性** —— 物件誕生時 CLR 保證記憶體歸零,所以 `IsDone` 在任何人碰它之前就已經是 `false`。兩種情況下記憶體狀態相同,事後看不出差別。
    `bool?` 之所以能分辨,是因為它是 `Nullable<bool>` —— 一個 struct,**多帶一個 `hasValue` 欄位**(所以 `bool` 1 byte、`bool?` 2 bytes)。它本身也是 value type,自己完全不是 null。
    同樣的坑在 `int` 上更危險:`PagedQuery(int Page, int PageSize)` 漏傳 pageSize → `0` → `.Take(0)` → 回傳空陣列,而且沒有任何錯誤。

- **同一個 `bool?`,PUT 和 PATCH 用它的理由完全相反**:
    |  | `UpdateTodoRequest`(PUT) | `SetDoneRequest`(PATCH) |
    |---|---|---|
    | 語意 | 完整替換 | 只改提供的欄位 |
    | 寫法 | `[Required] bool?` | `bool?` 不加 required(本專案選了專用端點,所以加了) |
    | 漏傳時 | **400**,拒絕不完整的請求 | `null` = 不要動這個欄位 |
    | 用 `bool?` 的理由 | 為了**偵測漏傳** | 為了**表達不修改** |

    而 `bool` 兩邊都不能用 —— 它無法區分「沒傳」和「傳了 false」。
    PUT 不加 `[Required]` 會造成靜默的資料遺失:客戶端只想改標題卻漏傳 `isDone`,完成狀態就被重設成 `false`,回 204 成功。

- **怎麼分辨是驗證層擋的還是 Service 擋的** —— 看回應格式:
    ```
    驗證層   →  {"type":...,"title":"One or more validation errors occurred.","errors":{"IsDone":[...]}}
    Service  →  {"message":"IsDone is required"}
    ```
    實測確認 `[Required] bool?` 是**驗證層**擋的,所以 Service 那兩個 null 檢查在走 HTTP 時是死碼。

- **那 Service 的保底檢查還要留嗎?** 要。
    `TodoService` 不是「HTTP API 的實作」,它是一個**可被任何人呼叫的服務**。在加 `AddValidation()` 之前,Minimal API 路徑上**它是唯一的門**;而 Day 13 寫單元測試時會直接 `new TodoService(fakeRepo).CreateAsync(...)`,那條路徑完全繞過端點。
    代價是「Title 不能空白」寫在兩個地方,而且**它們會不同步**(改了邊界的長度上限很容易忘記 Service 那邊)。

- **三層驗證的失敗表達方式各不相同**(這是整件事的核心):
    ```
    格式驗證(邊界)    →  400 ProblemDetails    「你送錯了,這是哪個欄位錯」
    不變量(領域模型)  →  丟例外                「到這裡還不合法,那是程式 bug」
    業務規則(Service) →  Result<T>             「請求合法,但現在不能這麼做」
    ```
    第二行解釋了為什麼不能「只在領域模型驗一次」:如果建構子對空白 Title 丟 `ArgumentException`,使用者打錯字就變成**例外** —— 而例外是用來表達「程式邏輯錯了」的。兩者混在一起,錯誤監控會被使用者的打字錯誤淹沒。
    這也是 Day 9 做 Result pattern 的正當理由:**預期內的失敗用回傳值,預期外的失敗才用例外。**


**練習 4:錯誤處理 middleware**
- **故意改錯連線字串** —— 改密碼或資料庫名(立刻失敗),不要改 Port(要等 TCP timeout,會以為程式當掉)。

- **兩種環境的實際輸出**:
    Development:
    ```
    An unhandled exception occurred while processing the request.
    PostgresException: 28P01: password authentication failed for user "postgres"
    Npgsql.Internal.NpgsqlConnector.ReadMessageLong(...)
    ```
    一行就交出三件事:**資料庫是 PostgreSQL**(攻擊者知道用哪套 SQL injection 語法)、**使用者名稱是 `postgres`**(可以開始試密碼)、**而那是預設的超級使用者**(代表沒做權限分離)。
    往下還有完整堆疊和**請求的 headers** —— 使用者帶的認證 cookie/token 會原封不動顯示在上面。所以這個頁面出現在生產環境是事故等級的問題,不是「比較詳細」而已。

    Production:
    ```
    HTTP ERROR 500   (完全空白,沒有 body)
    ```
    客戶端無法分辨「伺服器掛了/資料庫掛了/我的請求有問題」,也**沒有任何編號可以回報**。

- **正確答案是同一個例外、兩個去處**:
    ```
    客戶端    →  最少資訊 + 一個 traceId
    server log →  完整堆疊 + 同一個 traceId
    ```
    `traceId` 是把兩者綁在一起的鑰匙 —— 使用者回報編號,就能在 log 裡定位到那一筆。它由 `AddProblemDetails()` 自動補上(來自 `Activity.Current` 或 `HttpContext.TraceIdentifier`)。

- **`launchSettings.json` 會覆蓋 shell 的環境變數**:
    `ASPNETCORE_ENVIRONMENT=Production dotnet run` 沒有效果,因為 `http` profile 裡硬寫了 `Development`。
    ```bash
    ASPNETCORE_ENVIRONMENT=Production ASPNETCORE_URLS=http://localhost:5020 dotnet run --no-launch-profile
    ```
    兩個重要性質:它**不會進到 `dotnet publish` 的產出**,所以生產環境完全不受它影響;而 `ASPNETCORE_ENVIRONMENT` **沒被設定時預設就是 Production**。
    也就是說 Development 是本機靠這個檔案「刻意打開」的 —— 忘記設定會得到最保守的行為。跟 `ResultStatus.Invalid = 0` 是同一個失敗安全思路。

- **`IExceptionHandler` 的 `bool` 回傳值是個契約**:
    ```
    return false  →  觀察者:我只做副作用(log/metrics),回應交給預設行為
                     有多個 handler 時會繼續問下一個 —— 這是責任鏈,不是缺點
    return true   →  接管者:我已經完整寫好回應了,middleware 不再插手
                     所以**必須自己寫 body**,否則客戶端拿到 500 + 空 body
    ```
    實測確認:`return true` 但不寫 body → 又變回那個空白 500。
    而 `TryWriteAsync` 用的就是 `AddProblemDetails()` 註冊的那個 `IProblemDetailsService` —— 兩條路用**同一個服務**,差別只在誰發起。

- **要分清兩個名字很像的東西**:
    | | 是什麼 | 在哪 |
    |---|---|---|
    | `ExceptionHandlerMiddleware` | 真正的 middleware | pipeline 裡,由 `app.UseExceptionHandler()` 加入 |
    | `IExceptionHandler`(我的 `GlobalExceptionHandler`) | **一個服務** | 不在 pipeline 裡,被上面那個呼叫 |

    `AddExceptionHandler<T>()` 只是登記在容器裡等著被用。實測:把 `app.UseExceptionHandler()` 註解掉之後,handler **完全不會被呼叫,連 log 都沒有**。

- **middleware 的預設行為已經做了很多** —— 自己 log 一筆完整堆疊、設 500、用 `IProblemDetailsService` 產生 ProblemDetails。
    所以自訂 handler 如果也傳 `exception` 物件去 log,會得到**兩筆幾乎相同的堆疊**。
    它真正的價值在**補上下文**:middleware 只知道「發生了什麼」(堆疊),不知道「在什麼情境下發生」(路徑、使用者、traceId)。
    一個原則:**先確認框架的預設不夠,再加自己的層。** 這一天遇到兩次 —— `HasKey`/`IsRequired` 是 no-op、這個 handler 大部分是重複。

- **`app.Map...` 不是 middleware**(這點推翻了直覺):
    `MapControllers()` / `MapGet()` 只是把路由登記到一個集合裡,**不在寫它的位置插入任何東西**。真正執行端點的是 `EndpointMiddleware`,由 `WebApplication` 自動加在 pipeline 的**最後**。
    ```
    [自動] UseRouting()
      ↓
    UseExceptionHandler()     ← 不管寫在 Map 前面還是後面
      ↓
    [自動] UseEndpoints()     ← 端點在這裡執行,永遠是最內層
    ```
    **所以 `Use` 和 `Map` 的相對順序完全不影響行為,只有 `Use` 彼此之間的順序才重要。**
    實測驗證(用一個會丟例外的 `/boom` middleware):
    ```
    UseExceptionHandler() 在前  →  乾淨的 ProblemDetails 500 ✅
    UseExceptionHandler() 在後  →  例外穿過它,沒人攔,又變空白 500 ❌
    ```
    這就是為什麼標準順序是 `UseExceptionHandler` 擺最外層 —— 放到 `UseAuthentication()` 後面的話,認證過程丟的例外就漏出去了。

- **`UseExceptionHandler` 和 `UseStatusCodePages` 處理的是完全不同的東西**:
    ```
    UseExceptionHandler   →  攔「丟出來的例外」
    UseStatusCodePages    →  攔「狀態碼是 4xx/5xx 但 body 是空的」回應
    ```
    **五種錯誤,只有一種是例外**:
    | 回應 | 怎麼產生的 | `UseExceptionHandler` 攔得到? |
    |---|---|---|
    | 400 參數綁定失敗(`/todos/asdf`) | 綁定階段,handler 沒執行 | ❌ |
    | 400 驗證失敗(空 Title) | 驗證 filter,handler 沒執行 | ❌ |
    | 404 路由不存在(`/foo`) | routing 階段 | ❌ |
    | 404 資料找不到 | **自己的程式碼主動回傳** | ❌ |
    | 500 資料庫連不上 | **真的丟了例外** | ✅ |

    前四種都是「請求被正常處理,結論是個錯誤狀態碼」。這解釋了為什麼 `Results.NotFound()` 加了 `AddProblemDetails()` 還是空 body —— 它沒丟例外,而且沒有人去呼叫那個服務。

- **`AddProblemDetails()` 不是 middleware,它什麼都不攔** —— 只是註冊 `IProblemDetailsService` 給別人用。
    | | 有變成 ProblemDetails? |
    |---|---|
    | 驗證錯誤(400) | ✅ 驗證層會去用那個服務 |
    | `Results.NotFound()` | ❌ 要 `UseStatusCodePages()` 才會 |
    | 未處理例外 | ❌ 要 `UseExceptionHandler()` 才會 |

- **`UseStatusCodePages` 能補格式,補不出內容**:
    ```
    Results.NotFound()                             →  沒有 detail(middleware 不知道我想說什麼)
    Results.Problem(detail: "Todo is not found.")  →  有 detail
    ```
    所以「要不要依賴 middleware 補」決定了客戶端能不能拿到有意義的錯誤說明。
    最後的分配:**`Invalid` 保留具體原因(`detail: result.Error`)、`NotFound` 交給 `CustomizeProblemDetails` 的通用訊息**。
    `CustomizeProblemDetails` 用 `??=` 是關鍵 —— 只在「我沒提供」時才填,所以它是**後備而不是取代**。它真正負責的是我碰不到的那些角落:參數綁定失敗、路由不存在。

- **結構化日誌的佔位符靠位置綁定,不是靠名字**:
    ```csharp
    logger.LogError(exception, "... {Method} {Path}", context.Request.Method, context.Request.Path);
    //                               第1個  第2個      ↑第1個參數            ↑第2個參數
    ```
    把 template 裡的名字調換 → 值會錯位,**而編譯器不會警告**。名字只是「給那個位置取個名」,跟 `string.Format` 的 `{0}` `{1}` 是同一個機制。
    **不能用字串插值**,因為 logger 收到的東西不同:
    ```
    插值    →  "Unhandled exception for GET /todos"        (已經壓平的字串)
    結構化  →  template + ["GET", "/todos"]                 (分開保存)
    ```
    結構化之後 `Method` / `Path` 會變成**可查詢的欄位**(`Path = "/todos" and Method = "POST"`),也能按 template 分群統計;插值的話只剩全文搜尋。
    另外第一個參數要傳 `exception` 物件,不然堆疊不會進 log。佔位符用 PascalCase(它們會變成欄位名)。
    .NET 有個 analyzer 規則 `CA2254: Template should be a static expression` 專門抓這件事。

- **一個相關陷阱**:如果回應**已經開始傳送**才丟例外,錯誤處理就來不及了。
    ```
    warn: The response has already started, the error handler will not be executed.
    ```
    header 和部分 body 已經在線路上,改不回來,客戶端會收到截斷的 JSON。所以串流大量資料的端點要在開始寫回應**之前**就把可能失敗的事做完。


**練習 5(整合小專案):把 API 契約收乾淨**
- 讓五個端點的**所有**錯誤回應都是 `ProblemDetails` 格式 —— 400(驗證失敗)、404(找不到)、500(未預期例外)全部統一。
    我在 Program.cs 設置 AddProblemDetail(); 有使用 ??= 給他 400,404,500 會回傳的 detail 訊息, 如果各自有設置 detail 就不會被預設干擾

- 用 `TodoClient` 或 `.http` 跑一輪完整驗收。這次不只檢查狀態碼,還要檢查**回應 body 的格式一致性**。
    這個回傳沒問題

- 加一個 `PATCH /todos/{id}` 端點(只改 `IsDone`),對照它跟 `PUT /todos/{id}` 的語意差異,並驗證兩者都冪等。
    PATCH 跟 PUT 我都是放完整 todo 去更新, 確定為冪等

- 寫下你對「DTO 要不要為每個操作各拆一個」的看法:你會有 `CreateTodoRequest`、`TodoResponse`,那 `Update` 要不要自己的 `UpdateTodoRequest`?`PATCH` 要不要第三個?
    <!-- 我覺得要, 這樣語意、型別都清楚 -->
    規則驗證互不相容, 這才是真正的論據:
                            Title	                        IsDone
CreateTodoRequest	[Required][StringLength(20)]	欄位不存在 —— 新建必然 false
UpdateTodoRequest	[Required][StringLength(20)]	[Required] bool? —— 必須明確提供
SetDoneRequest	    欄位不存在	                      [Required] bool?
TodoResponse	    無驗證(輸出用)	                   無驗證(輸出用)

最關鍵的推論:共用 DTO 會逼你把驗證退回命令式
假設共用一個 TodoRequest(string? Title, bool? IsDone),為了讓三個操作都能用,所有欄位都得是 nullable 且不能加 [Required]。然後你必須在 Service 裡寫:
```csharp
// Create 時
if (request.Title is null) return Invalid("Title is required.");
if (request.IsDone is not null) return Invalid("不能在建立時指定完成狀態");

// Update 時
if (request.Title is null) return Invalid("Title is required.");
if (request.IsDone is null) return Invalid("IsDone is required.");
```

**驗收標準**
- 領域模型 `Todo` 不再出現在任何 API 的輸入或輸出上。
    我在有回傳 `Todo` 的地方都有使用 DTO `ToResponse();` 來做轉換
    輸入部分 CreateTodoRequest / UpdateTodoRequest / SetDoneRequest 都是 DTO,Todo 從來沒出現在任何端點的參數上

- 送不合法的輸入,兩套端點(Minimal API / Controller)回傳的狀態碼與 body 格式一致。
    有在 Program.cs 中把 `AddProblemDetails()` 使用 ??= 預設 400, 404, 500 相關的錯誤訊息回傳
    ```csharp
    GET    /todos/{不存在的guid}   vs  /api/todos/{同一個}
    GET    /todos/asdf             vs  /api/todos/asdf
    POST   /todos {}               vs  /api/todos {}
    DELETE /todos/{不存在的guid}   vs  /api/todos/{同一個}
    ```
    每一對除了 traceId 應該完全相同。有了這個,驗收項才算有證據。

- 連線字串改錯之後,客戶端拿到的是乾淨的 `ProblemDetails` 500,看不到 stack trace,也看不到連線字串。
    有加入 `builder.Services.AddExceptionHandler<GlobalExceptionHandler>();` 跟 `UseExceptionHandler()` 來去處理

- 能解釋「格式驗證」「業務規則驗證」「資料完整性驗證」分別該在哪一層,以及為什麼。
    格式驗證: 在 API 邊界(Endpoint/Controller), 因為是輸入的驗證所以在接觸到的第一層就解決
    業務規則驗證: Service 層, <!--因為業務規則是屬於 Service 處理的--> 
        1. 他需要知道當前狀態 —— 「已完成的 Todo 不能改標題」必須先讀出那筆資料,所以必須在能存取 Repository 的層
        2. 換掉 API 它依然成立 —— 把 HTTP 換成 CLI、gRPC、背景作業,這條規則還是要生效。而格式驗證會跟著 API 一起消失

    資料完整性驗證: 資料庫操作的時候, <!--無法只靠應用層保護, 不同資料庫有各自的鎖可以去保護競態問題-->
    「不同資料庫有各自的鎖可以去保護競態問題」 —— 這句把兩個不同的機制混在一起了,而這個區分很重要:

是什麼	             例子
約束(constraint)	宣告式的規則,資料庫保證它永遠成立	UNIQUE、FOREIGN KEY、CHECK、NOT NULL
鎖(lock)	        並行控制機制,管理同時存取	行鎖、表鎖、SELECT FOR UPDATE

資料完整性靠的是約束,不是鎖。
而約束之所以能解決你前面那個「同名 Todo」的競態,不是因為它用了鎖,是因為檢查和寫入發生在同一個原子操作裡:
```sql
INSERT INTO "Todos" (...) VALUES (...);
-- 資料庫在寫入的瞬間檢查唯一索引,沒有「先查再寫」的空隙
```

對照應用層的做法:
```csharp
var exists = await repo.ExistsByTitleAsync(title);   // 查
if (exists) return Conflict();
await repo.AddAsync(todo);                            // 寫  ← 中間有空隙
```
TOCTOU 的根源是「檢查」和「動作」分成兩步。 約束把它們合成一步,所以空隙消失了 —— 這才是它無可取代的原因,而且跟鎖無關。

- 能說明為什麼 `PUT` 和 `PATCH` 都冪等而 `POST` 不冪等 —— 用你 Day 6 的結論(關鍵在「設定絕對值 vs 在現值上做變化」)檢查你的 `PATCH` 
    `PUT` 跟 `PATCH` 我都是使用絕對值設置, 所以不管執行幾次都會是一樣的結果
    `POST` 如果同樣操作執行多次, 則會一直建立
    原因:CreateAsync 裡的 Guid.CreateVersion7() 每次都產生新的 id,所以同樣的 body 送兩次會建出兩筆獨立的資源。它不是「在現值上做變化」,而是每次都產生一個新的現值 —— 這是第三種情況,比「設定絕對值 vs 在現值上變化」的二分法多出來的那個。

    順帶可以補一句:這就是為什麼 POST 需要 idempotency key 才能安全重試(你 Day 6 筆記裡寫過)—— 客戶端給一個唯一鍵,伺服器用它去識別「這是同一個請求的重試」而不是新的建立。


**延伸**
- 查 FluentValidation,比較它跟 Data Annotations 的取捨(表達力、跨欄位規則 vs 多一個依賴)。
    Data Annotations: 適用於簡單、直接的配置，例如標示主鍵、必填欄位、字串長度。他們直接放在模型類別上，方便查看。
    Fluent API: 適用於複雜的配置，例如多對多關係、自定義欄位名稱、組合鍵(composite keys)、索引(indexs)、更精細的映射。他將所有配置集中在 `DbContext` 中，保持模型類別的整齊。

    	EF Core Fluent API	         FluentValidation
是什麼	EF Core 內建的模型配置	        第三方 NuGet 套件
寫在哪	DbContext.OnModelCreating	  獨立的 validator 類別
管什麼	資料庫 schema 映射	            輸入驗證

FluentValidation 長這樣:
```csharp
public class CreateTodoRequestValidator : AbstractValidator<CreateTodoRequest>
{
    public CreateTodoRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("標題不能空白")
            .MaximumLength(20);
    }
}
```

它取代的是你 DTO 上那些 [Required] [StringLength(20)]。真正的取捨:
            Data Annotations	        FluentValidation
規則位置	DTO 屬性上,一眼看到	            獨立類別,DTO 保持乾淨
跨欄位規則	很難(要 IValidatableObject)	   RuleFor(x => x.End).GreaterThan(x => x.Start)
條件式規則	幾乎做不到	                    When() / Unless()
可注入服務	❌ attribute 無法注入 DI	       ✅ validator 是普通類別
單獨測試	要跑整個驗證流程	             直接 new 出來測
依賴	    內建	                      多一個套件

最關鍵的是「可注入服務」那一列。 attribute 是編譯期的 metadata,所以你不可能寫一個 [UniqueTitle] 去查資料庫。FluentValidation 的 validator 可以注入 ITodoRepository:
```csharp
RuleFor(x => x.Title)
    .MustAsync(async (title, ct) => !await repo.ExistsByTitleAsync(title));
```
但這個能力有陷阱 —— 把「標題不能重複」放進驗證層的話,你就把資料完整性驗證塞進了格式驗證那一層,而且它有你學過的 TOCTOU 問題。能做不代表該做,這正好又是那個三層分工的考題。


- 查 `[ApiController]` 的自動模型驗證是怎麼運作的 —— 為什麼 Controller 版不需要你手動檢查 `ModelState`?回去看 Day 9 那個「探索規則不是靠 `[ApiController]`」的結論,這次看的是它**真正**負責的事。
    加上 `[ApiController]` 屬性時, 自動模型驗證(Automatic Model Validation) 會預設開啟
    1. 機制是一個 action filter(ModelStateInvalidFilter),由 [ApiController] 自動加上
    2. 它在 action 執行之前跑 —— 所以驗證失敗時 action 方法根本不執行(你的 ResultStatus.Invalid 分支因此變成死碼)
    3. 失敗時產生 ValidationProblemDetails(帶 errors 字典)直接回 400
    4. 規則來源有兩個:Data Annotations + non-nullable reference type 的隱式 required(你 CreateTodoRequest 沒寫 [Required] 也會擋空值,就是這個)
    5. 可以關掉:options.SuppressModelStateInvalidFilter = true,關掉之後就得自己寫 if (!ModelState.IsValid) return ValidationProblem();

    第 4 點特別值得記 —— 你前面困惑過「我沒加 attribute 為什麼會擋」,答案就在那裡(non-nullable reference type 的隱式 required)。

- 查 OpenAPI:`dotnet new webapi` 預設附的那個東西是什麼,DTO 跟它的關係是什麼。
    模板可能會配置 OpenAPI 文件產生功能。實際包含哪些套件與程式碼，會受到 .NET SDK 版本和模板選項影響。
    例如，較新的 ASP.NET Core 專案可能出現：
    ```csharp
    builder.Services.AddOpenApi();
    var app = builder.Build();
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }
    ```
    - AddOpenApi()：註冊 OpenAPI 文件產生服務。
    - MapOpenApi()：建立提供 OpenAPI JSON 文件的 HTTP Endpoint。

    DTO 是 Data Transfer Object (資料傳輸物件)，負責定義 API 要接收或傳送哪些資料。
    真正的關係是:OpenAPI 的 schema 直接從你的 DTO 型別產生,而且 Data Annotations 會被寫進 schema。

    你的 `CreateTodoRequest([Required][StringLength(20)] string Title)` 會變成:
    ```csharp
    {
        "type": "object",
        "required": ["title"],
        "properties": {
            "title": { "type": "string", "maxLength": 20 }
        }
    }
    ```
    maxLength: 20 出現在文件裡了 —— 客戶端讀文件就知道上限,不用靠試錯。

- 想一下 API 版本控制:哪天 `TodoResponse` 要改欄位名,已經上線的前端怎麼辦?
    新增一個欄位, 讓前端能夠不會壞掉, 但是這樣不知道好不好

    我的答案(新增欄位、保留舊的)有個名字:additive change,而且它通常是最好的選擇。

    完整的選項:
    做法	                                    代價
    新增欄位、保留舊欄位	                  回應變大、兩欄要同步、需要棄用計畫
    API 版本控制(/v1/todos、/v2/todos)	    要維護兩套程式碼和測試
    就不改	                               很多時候「欄位名不夠好」不足以構成改動的理由

而這題最重要的洞察:
你這一整天做的事 —— DTO 把 API 契約跟資料庫 schema 解耦 —— 正是為了讓這個問題有解。
```csharp
public record TodoResponse(Guid Id, string Name, bool IsDone);   // API 改名成 Name

public static TodoResponse ToResponse(this Todo todo)
    => new(todo.Id, todo.Title, todo.IsDone);
//                      ↑ 資料庫欄位還是 Title,完全沒動
```
資料庫改名不影響 API,API 改名不影響資料庫。 這就是你練習 1 答的那個「解耦」,現在有了具體場景:

沒有 DTO → Todo.Title 改名 = 所有前端立刻壞掉,而且你被資料庫 schema 綁死,不敢重構
有 DTO → 兩邊各自演進,中間那行 mapping 吸收所有差異

那行 ToResponse() 看起來只是三行無聊的轉換,但它是整個解耦的支點。