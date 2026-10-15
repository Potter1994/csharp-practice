**練習 1:概念暖身(先寫筆記再動手)**
Day 9 的 Update 是這樣寫的:
```csharp
_todos[index] = _todos[index] with { Title = todo.Title, IsDone = todo.IsDone };
```
with 產生的是一個全新物件。EF Core 追蹤的是從資料庫查出來的那個原物件,新物件它根本不認識 —— 所以 SaveChangesAsync() 什麼都不會做,而且不會報錯(回傳 0,靜默失敗)。

兩種解法:
```csharp
// A. 明確告訴 EF Core「拿這個去更新」
db.Update(updated);
await db.SaveChangesAsync();

// B. 把新值複製到被追蹤的那個物件上
var tracked = await db.Todos.FindAsync(id);
db.Entry(tracked).CurrentValues.SetValues(updated);
await db.SaveChangesAsync();
```
（也可以把 Todo 從 record 改成一般 class 配可寫的屬性,那樣直接改屬性就會被追蹤 —— 但你 Day 11 做 DTO 時會希望領域模型保持不可變,所以 A 或 B 比較好。）

- **ORM** 解決什麼問題?它的**代價**是什麼?(別只寫好處)
    ORM 解決的問題是能夠直接在 C# 使用物件操作資料庫
    代價是:
    1. 容易遇到 N+1 Query 問題
    2. 效能不透明 —— 你寫的 LINQ 可能產生爛 SQL,但你看不到(除非開 log)。這就是練習 5 要你做的事
    3. 抽象洩漏 —— 複雜查詢最後還是得寫 raw SQL,而且你得先懂 ORM 怎麼翻譯才知道哪裡出問題
    4. 學習成本加倍 —— 要懂 SQL 而且要懂 ORM 的行為(像上面那個 with 的陷阱)
    5. 難以用上資料庫的進階能力 —— window function、CTE、索引提示,ORM 通常做不好或做不到

    優點: 提供許多處理資料庫的常見工作
    - 型別安全(編譯器能幫你檢查 SQL 字串中的欄位名稱是否仍然存在)
    - Change Tracking: 追蹤實體的變更
    - Relationships: 設定一對多, 多對多等關聯
    - Migration: 管理資料庫 Schema 的版本變更
    - Transaction: 支援交易操作
    - LINQ: 用 C# 表達查詢條件

- `DbContext` 是什麼?為什麼它**必須**是 Scoped,不能是 Singleton?(提示:回去看 Day 8 那個 captive dependency 的錯誤訊息)
    <!-- 是因為他是短期的服務不能被 Singletone 長期綁架並留在記憶體中嗎? -->
    1. `DbContext` 不是執行緒安全的 <- 最關鍵
        官方文件明確說它不支援並行操作。Singleton 會被所有請求同時使用,兩個請求同時查詢就會丟:
        InvalidOperationException: A second operation was started on this context instance
        before a previous operation completed.
        
    2. 交易邊界會混在一起 <- 最可怕, 因為是靜默的資料錯誤
        SaveChangesAsync() 是一個交易。如果共用同一個 DbContext:
        請求 A：改了 Todo 1，還沒 SaveChanges
        請求 B：改了 Todo 2，呼叫 SaveChanges
            → 💥 A 還沒完成的變更也被一起提交了
        這會造成資料提交到一半的狀態,而且完全不會報錯。

    3. 變更追蹤無限累積
        每查一筆就保存一份快照。Singleton 永遠不釋放 -> 記憶體持續成長, 而且追蹤器越大, `SaveChange` 的比對越慢。
        為什麼 Scope 剛好: 一個 HTTP 請求 = 一個工作元 = 一個交易範圍。請求結束就釋放, 乾乾淨淨

- **Migration** 是什麼?為什麼不直接手寫 SQL 建表就好?
    Migration 我也不知道該怎麼解釋, 再麻煩你解釋給我理解
    不手寫 SQL 有幾個優點
        1. 同步問題: 你改了 C# 的 `Todo` 類別, 要記得去改 SQL。兩邊遲早不一致, 而且不一致時沒人會發現
        2. 多環境要套用的變更: 你的機器、同事的機器、測試環境、正式環境,每個都要照同樣順序跑過同樣的變更
        3. 要漸進升級, 不能砍掉重建: 正式資料庫裡有真實資料,只能 `ALTER`,不能 `DROP` 再 `CREATE`
        4. EF Core 自動算差異: 你不用自己想「從 v3 到 v4 要改什麼」,它比對快照就知道
        5. 可回滾: `Down()`

    元件	        |        在哪	                     |       記的是什麼
    migration 檔案	    Migrations/*_InitialCreate.cs	    一次變更怎麼做(Up)、怎麼復原(Down)
    snapshot	       AppDbContextModelSnapshot.cs	       「模型目前長什麼樣」—— 算差異的基準
    history 表	       資料庫裡的 __EFMigrationsHistory	     「這台資料庫跑過哪些 migration」


    dotnet ef migrations script                       # 從零到最新
    dotnet ef migrations script InitialCreate         # 從 InitialCreate 之後到最新
    dotnet ef migrations script AddCreatedAt InitialCreate   # 反向 → 產生 Down() 的 SQL

    dotnet ef migrations script --idempotent -o migrate.sql
    你會看到每段 SQL 被包進 PostgreSQL 的 DO $$ ... IF NOT EXISTS ... END $$ 區塊裡,先檢查 history 表有沒有這筆才執行。
    這就是「同一份腳本對任何進度的資料庫都安全」的實作方式。

- **變更追蹤(change tracking)** 是什麼?為什麼改了物件的屬性之後,只要呼叫 `SaveChangesAsync()` 就會寫回資料庫?
    Change tracking 預設會透過追蹤 DbContext 查詢出來的 Entity, 在 EF Core 知道是被新增, 刪除, 修改後在 `SaveChangeAsync()` 產生對應的 SQL

    查詢時 EF Core 會保存一份原始值的快照,SaveChanges() 時拿當前值跟快照比對,算出哪些欄位變了,只更新那些欄位。
    每個被追蹤的物件有五種狀態:
    Added → INSERT       Modified → UPDATE      Deleted → DELETE
    Unchanged → 什麼都不做          Detached → 不在追蹤範圍內

    成本:快照佔記憶體、比對要時間。所以唯讀查詢應該關掉:
    await db.Todos.AsNoTracking().ToListAsync();   // 不保存快照，快很多
    這就是延伸題要你查的 AsNoTracking()。你的 GetAllAsync() 是純讀取,正是該用的地方。

- 對照 Day 9:`DbSet<Todo>` 跟你自己寫的 `ITodoRepository`,**像在哪、不像在哪**?
    DbSet 跟 ITodoRepository 他們都透過介面方法提供使用
    - 都是「語意集合」的抽象: `Add`、`Remove`、查詢
    - 都隱藏了實際資料怎麼存取

    不像:
    - `DbSet<T>` 是 `IQueryable<T>`, 你的介面回傳已執行的結果
        db.Todos.Where(...).Select(...).Take(10)   // 還沒執行，可以繼續組合，最後翻譯成一句 SQL (拿到的是在資料庫處理完之後的資料)
        repo.GetAllAsync()                          // 已經執行完了，拿到的是記憶體裡的清單
        ```
        db.Todos.Where(...).Select(...).Take(10)：直接用 DbSet（IQueryable）
        SELECT "t"."Title"                  ← 只要一個欄位
        FROM "Todos" AS "t"
        WHERE NOT ("t"."IsDone")            ← 過濾在資料庫做
        LIMIT @p                            ← 只傳 10 筆
        → 拿到 10 筆，耗時 24 ms

        repo.GetAllAsync()：先 GetAll 再在記憶體過濾（你的 Repository）
        SELECT "t"."Id", "t"."IsDone", "t"."Title", "t"."CreatedAt"   ← 全部欄位
        FROM "Todos" AS "t"                           ← 沒有 WHERE、沒有 LIMIT
        → 從資料庫搬了 10000 筆進記憶體，才過濾出 10 筆，耗時 92 ms 
        ```

    - `DbSet` 不負責寫入
        `db.Todo.Add(todo)` 只是標記, 真正寫入要等 `db.SaveChangeAsync()`, 那是 Unit of Work 模式。 Day9 的 `repo.Add()` 是立刻寫入
    - `DbSet<T>` 是通用的, `ITodoRepository` 是為 Todo 客製的


寫下你對「EF Core 上面要不要再包 Repository」的看法(Day 9 延伸題欠的那題)。
    我覺得還是需要包 Repository, 這樣能無痛切換, 會遇到 SQL 相關效能問題可以在 IRepository 只給寫入相關的, 而複雜查詢的時候再額外使用 db 去寫
    但這個代價就是會有某一層直接依賴 AppDbContext, 也就是直接依賴 EF Core。

1. 你今天把 Repository 從 InMemory 換到 File 再換到 EfCore,一共改了幾行?TodoClient 那套測試改了幾個字?
    只改一行註冊,Service / Endpoint / Controller / TodoClient 測試全部沒動

2. 那句 SELECT t."Id", t."CreatedAt", t."IsDone", t."Title" FROM "Todos" ORDER BY t."Id",在表裡有十萬筆資料的時候會發生什麼事?你的 92 ms 是 10000 筆的數字。
    會取回十萬筆資料, 然後在本地記憶體再去做處理

3. 如果 ITodoRepository 有 40 個方法,你還能在一個下午寫出 FileTodoRepository 嗎?
    這樣每個 Repository 都要寫 40 個方法太不切實際了

上面三個題目連接起來:
    「可替換性」和「查詢能力」是同一個取捨的兩面, 不是兩個獨立的優缺點。
    介面小才換得動, 但介面小就意味者查詢只能在記憶體做; 要讓查詢下推到 SQL, 介面就得膨脹, 而膨脹到一定程度就換不動了。


EF Core 是什麼?
- Entity Framework Core 微軟的 ORM(Object-Relational Mapper), 讓你用 C# 物件操作資料庫, 不用自己寫 SQL。


所以使用 AsNoTracking 查兩次回傳的不會是同一個物件實例嗎
對,**不是同一個實例**。每次查詢都會建立全新的物件。

## 追蹤模式在做什麼

`DbContext` 內部有一張 **identity map** —— 以「實體型別 + 主鍵」為 key 的字典。查詢 materialize 時,EF Core 會先問這張表:「這個 Id 我手上有了嗎?」

```csharp
var a1 = await db.Todos.FirstOrDefaultAsync(t => t.Id == id);
var a2 = await db.Todos.FirstOrDefaultAsync(t => t.Id == id);

ReferenceEquals(a1, a2);    // true —— 同一個物件
```

這裡有個關鍵細節:**第二次查詢仍然發了 SQL、資料仍然從資料庫傳回來了**,但 EF Core 發現 identity map 裡已經有這個 Id,於是**把讀到的新資料丟掉,回傳原本那個物件**。

換成 `AsNoTracking`:

```csharp
var b1 = await db.Todos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
var b2 = await db.Todos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

ReferenceEquals(b1, b2);    // false —— 兩個獨立的物件
```

沒有 identity map 要查、沒有比對,直接 new 一個新的。這就是省下來的那部分成本。

## 一個反直覺的後果

上面那句「把讀到的新資料丟掉」有個實際影響:

**追蹤模式下,同一個請求裡第二次查詢,可能拿不到資料庫的最新值。**

```csharp
var a1 = await db.Todos.FirstOrDefaultAsync(t => t.Id == id);   // Title = "原本"

// 此時別的請求（或你自己開 psql）把這筆的 Title 改成 "被改了"

var a2 = await db.Todos.FirstOrDefaultAsync(t => t.Id == id);
// a2.Title 還是 "原本" —— 因為 a2 就是 a1
```

SQL 確實跑了、資料庫確實回傳了 `"被改了"`,但 EF Core 選擇保留 identity map 裡那個物件。這是刻意的設計,不是 bug。

（真的需要最新值時要用 `db.Entry(a1).ReloadAsync()`,或換一個 DbContext。這也是 `AsNoTracking` 的另一個好處 —— 它總是給你查詢當時的資料庫狀態。）

## 為什麼要這樣設計

因為 change tracker 必須保證「一個實體在一個 `DbContext` 裡只有一份」。否則:

```csharp
a1.Title = "甲";
a2.Title = "乙";
await db.SaveChangesAsync();    // 該寫哪一個?
```

如果 `a1` 和 `a2` 是兩個不同物件,`SaveChanges` 就面對一個無解的歧義。identity map 從根本上消滅這個問題 —— 這是 Unit of Work 模式的必要條件,也是你筆記裡寫的「`DbSet` 不負責寫入,真正寫入要等 `SaveChangesAsync`」那句話背後的機制。

## `AsNoTracking` 的隱藏成本

反過來說,沒有身分解析也有代價。等你做延伸題加 `Category` 關聯之後會遇到:

```csharp
db.Todos.Include(t => t.Category).AsNoTracking().ToList()
```

假設 100 筆 Todo 都屬於同一個 Category:

- **追蹤模式** → 100 個 `todo.Category` 全部指向**同一個** Category 物件
- **`AsNoTracking`** → 建立 **100 個**內容相同的 Category 物件

省了快照,卻多配置了 99 個重複物件。所以 `AsNoTracking` 不是無條件更快 —— 在有大量重複關聯資料的查詢上可能更慢、更吃記憶體。

EF Core 5.0 為此加了折衷方案:

```csharp
db.Todos.Include(t => t.Category).AsNoTrackingWithIdentityResolution()
```

不追蹤(不建快照、不進 change tracker),但**仍然做身分解析**(重複的實體共用一個物件)。

你現在沒有關聯資料,所以 `AsNoTracking()` 是純賺的。但知道這個分野,延伸題加 `Category` 的時候就不會踩到。

## 自己驗一次

貼這個臨時端點進去打一次,比讀文字清楚:

```csharp
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
```

（表要先有資料,`FirstAsync` 空表會丟例外。）

最後那行 `db.ChangeTracker.Entries<Todo>().Count()` 值得注意 —— 它會告訴你 change tracker 裡現在躺著幾個實體。你會看到 `AsNoTracking` 查出來的那兩個**完全不在裡面**。這是「追蹤」這件事第一次變成你能直接觀察的東西,而不只是概念。

---

# 延伸題

## 延伸 1:`Include()` 與 N+1 查詢問題

> **狀態:概念整理,沒有實際實作。** 沒有真的幫 `Todo` 加 `Category` 關聯,下面是討論後的理解。要驗證的話建議開新分支做,不要動現在這份乾淨的成果。

### N+1 是什麼

「1 次查主表 + N 次查關聯」。查 100 筆 Todo 要顯示分類名稱,結果發了 101 句 SQL。

### 關鍵:EF Core 預設不會發生 N+1

EF Core **預設關閉 lazy loading**,所以沒有 `Include` 的話導覽屬性就是 `null`,不會偷偷發查詢:

```csharp
var todos = await db.Todos.ToListAsync();
todos[0].Category;    // null —— 不會去查,就是 null
```

要開啟 lazy loading 必須**主動做三件事**:

1. 裝 `Microsoft.EntityFrameworkCore.Proxies` 套件
2. 呼叫 `UseLazyLoadingProxies()`
3. 導覽屬性宣告成 `virtual`

這跟舊的 Entity Framework 6 不同(EF6 預設開啟 lazy loading,所以 N+1 滿地都是)。網路上大多數 N+1 文章講的是那個世界或其他語言的 ORM。

### 那 EF Core 的 N+1 從哪來 → 自己手寫的迴圈

最容易踩到的時機是 Day 11 做 DTO 轉換:

```csharp
var todos = await db.Todos.ToListAsync();                    // 1 次

foreach (var t in todos)
{
    var cat = await db.Categories.FindAsync(t.CategoryId);   // N 次 ← 這裡
    dtos.Add(new TodoDto(t.Title, cat.Name));
}
```

這段程式碼讀起來毫無問題、測試會過、小資料量下很快。5000 筆時就是 5001 次資料庫往返。

**而且不會有任何警告** —— 只會在 log 裡看到 5001 句幾乎一樣的 `SELECT`。這就是練習 5 開 SQL log 的真正價值:**N+1 在程式碼裡看不出來,在 log 裡一眼認得出來**。

### `Include()` 解決它,但自己帶了另一個問題

```csharp
await db.Todos.Include(t => t.Category).ToListAsync();
```

產生一句 `LEFT JOIN`,一次拿齊。`Todo → Category` 是 many-to-one(多個 Todo 指向一個 Category),這種 `Include` 很安全。

但**反向**的 one-to-many 且 `Include` 兩個以上集合時:

```csharp
db.Categories.Include(c => c.Todos).Include(c => c.Tags)
```

SQL 變成 `Todos × Tags` 的笛卡爾積 —— 一個有 100 個 Todo、10 個 Tag 的 Category 回傳 1000 列,Category 的欄位重複 1000 次。這叫**笛卡爾爆炸**,症狀是「只有幾百筆資料,查詢卻回傳幾十 MB」。

EF Core 5.0 的解法是 `AsSplitQuery()`,拆成三句查詢(Categories / Todos / Tags)在記憶體組裝。

### 三種策略是一個光譜,沒有絕對正確

| 策略 | 查詢次數 | 傳輸量 |
|---|---|---|
| N+1 | **N+1 次** | 最小 |
| `Include` 單一 JOIN | **1 次** | 可能爆炸 |
| `AsSplitQuery` | 關聯數 + 1 次 | 無重複 |

多次往返 vs 重複資料,選哪個取決於網路延遲和資料形狀。這是「ORM 效能不透明」那個代價最具體的樣子 —— 寫起來都是一兩行 LINQ,送出去的 SQL 天差地遠。

### 這題對練習 5 結論的補強

如果真的加了 `Category` 關聯,`InMemoryTodoRepository` 和 `FileTodoRepository` 要怎麼實作?JSON 檔裡怎麼表達關聯?`Include` 這個概念在它們身上根本不存在。

**關聯資料會讓「可替換的 Repository」這個承諾大幅貶值。** 今天能輕鬆換三個實作,有很大一部分是因為 `Todo` 是個沒有關聯的扁平物件。真實系統的實體有關聯,而關聯正是 `DbSet` / `Include` 擅長、手寫 Repository 很痛的地方。

---

## 延伸 3:`dotnet ef migrations script`

### 本質差別只有一句

```
dotnet ef database update      → 連資料庫 → 讀 history 表 → 算出缺哪些 → 直接執行
dotnet ef migrations script    → 不連資料庫 → 把 SQL 印出來給你
```

### 不同環境用不同做法(重要:開發環境就是用 update)

| 環境 | 做法 | 為什麼 |
|---|---|---|
| 自己的機器 | `database update` | 自己就是 DBA,資料隨時可丟 |
| 同事的機器 | `database update` | pull 到 migration 檔案後自己跑 |
| CI / 測試環境 | `database update` 或腳本 | 拋棄式環境 |
| **正式環境** | `script --idempotent` → 審核 → 執行 | 見下面四個理由 |

協同開發的日常循環:

```
我:改模型 → migrations add AddCategory → database update(更新自己的資料庫)
  → git commit(migration 檔案 + snapshot 一起進版控)→ push

同事:git pull → 看到 Migrations/ 多了檔案 → database update(更新他自己的資料庫)
```

**migration 檔案和 snapshot 必須 commit 進 git** —— 它們是原始碼,不是建置產物。

### 正式環境不用 `database update` 的四個理由

1. **權限分離** —— `ALTER TABLE` 這種 DDL 權限在正式環境通常只有 DBA 有;應用程式帳號應該只能 CRUD
2. **可審核** —— 有人能在執行前讀過那段 SQL。這次親身體會過為什麼必要:那個空的 `Up()`、還有 `defaultValue` 給出 `-infinity`,直接套進生產環境一個靜默無效、一個污染全表
3. **可存檔** —— 腳本是靜態檔案,能進版控、附在變更單上、出事時比對「當時到底執行了什麼」
4. **生產環境根本沒有原始碼** ← 最致命。`database update` 需要**載入專案的 assembly** 才能讀到 migration 類別,也就是執行的機器上必須有 .NET SDK、原始碼、能還原的 NuGet 套件。容器化部署裡這些前提全都不成立

### `--idempotent` 是最重要的旗標

```bash
dotnet ef migrations script --idempotent -o migrate.sql
```

產生的 SQL 會把每個 migration 包在「history 表裡還沒有這筆才執行」的條件裡,所以**同一份腳本對任何進度的資料庫都安全**,重複跑也不會壞。

沒有這個旗標的腳本重跑會直接失敗(重複建表)。而現實是往往不確定目標環境跑到哪了 —— 測試、預備、生產進度各不相同。

其他形式:

```bash
dotnet ef migrations script                               # 從零到最新
dotnet ef migrations script InitialCreate                 # 從 InitialCreate 之後到最新
dotnet ef migrations script AddCreatedAt InitialCreate    # from/to 反過來 → 產生回滾腳本
```

最後那個很實用:**把 from/to 反過來就會產生 `Down()` 的 SQL**。生產環境的變更計畫通常要求附上回滾腳本,這就是產生它的方法。

### 實際跑過的輸出,三個觀察

**觀察 1:每個 migration 是一個獨立交易,schema 變更和 history 記錄在同一個交易裡**

```sql
START TRANSACTION;
  CREATE TABLE "Todos" (...);
  INSERT INTO "__EFMigrationsHistory" VALUES ('20261014075757_InitialCreate', '10.0.12');
COMMIT;
```

這是整個 migration 系統可靠性的基礎 —— 不可能出現「表建好了但 history 沒記」或反過來。要嘛兩個都成功,要嘛一起回滾。

而且是**每個 migration 各自一個交易**,不是全部包一大包。所以第二個失敗時第一個仍然 commit 並記錄完成,下次重跑只補跑第二個。

> PostgreSQL 支援**交易式 DDL** 所以這招才行得通。MySQL 的 DDL 是隱式 commit,做不到這件事 —— 這是 PostgreSQL 在 schema 遷移上的實質優勢。

腳本第一句 `CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory"` 也是同個道理:不假設記帳表存在,自己負責建立,所以對全新的空資料庫就能直接跑。

**觀察 2:`-infinity` 的因果鏈在這裡閉合**

```sql
ALTER TABLE "Todos" ADD "CreatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
```

```
EF Core 產生 defaultValue: new DateTime(1,1,1,...)     ← C# 的 DateTime.MinValue
  → Npgsql 翻譯成 DEFAULT TIMESTAMPTZ '-infinity'     ← 就是這句 SQL
  → psql 查出來顯示 -infinity                          ← 回滾實驗看到的
```

**觀察 3:idempotent 版本的 `CREATE TABLE` 沒有加 `IF NOT EXISTS`**

```sql
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '...') THEN
    CREATE TABLE "Todos" (        ← 這裡沒有 IF NOT EXISTS
    ...
```

判斷完全依賴 history 表。這是刻意的:腳本保護的是「**migration 的進度一致性**」,不是盲目容錯。如果有人手動建了同名表而 history 沒紀錄,腳本還是會嘗試建表並失敗 —— 這是正確行為,因為那個資料庫處於 EF Core 無法理解的狀態,硬跑下去只會讓問題更難查。

另外每個 migration 被拆成兩個 `DO $EF$` 區塊(一個做變更、一個記 history),但檢查的是**完全相同的條件**,又包在同一個交易裡,所以原子性保證依然成立。

### 還有第三種做法,以及它的問題

很多專案在 `Program.cs` 裡啟動時自動套用:

```csharp
using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
```

方便,小專案或單一實例可接受。但生產環境有三個問題:應用程式帳號被迫擁有 DDL 權限、多個實例同時啟動會競爭(EF Core 有加鎖但失敗處理很醜)、schema 變更變成部署的副作用而不是可審核的獨立步驟。

Day 26 做 CI/CD 時的標準答案大概是:pipeline 裡一個獨立 job 跑 `migrations script --idempotent`,產物存檔,再由另一個有 DDL 權限的步驟執行 —— 跟部署應用程式的 job 分開。

### 協同開發真正會痛的地方

不是 `update` 還是 `script`,而是**兩個人同時加 migration**:

```
main:  InitialCreate → AddCreatedAt
         ├── 我的分支:  → AddCategory    (改了 snapshot)
         └── 同事分支:  → AddPriority    (也改了 snapshot)
```

兩人都改了 `AppDbContextModelSnapshot.cs`,merge 時**一定衝突**。而這個檔案是自動產生的、幾百行,手動合併幾乎不可能正確。

標準解法是**後 merge 的人放棄自己的 migration 重做**:

```bash
dotnet ef migrations remove     # 移除自己還沒推出去的 migration
git pull                        # 拿到對方的 migration
dotnet ef migrations add AddPriority   # 基於新的 snapshot 重新產生
```

因為 migration 必須是**線性的一條鏈** —— 每個都假設前一個已經套用。這也是「migration 一旦推上共用分支就不能再改、只能往前補」這條原則的根源。

> 所以有些團隊會約定「同一個 sprint 內只有一個人負責加 migration」或「加 migration 前先在群組喊一聲」。聽起來很土,但確實有效。
