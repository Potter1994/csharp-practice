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
    - Transcations: 支援交易操作
    - LINQ: 用 C# 表達查詢條件

- `DbContext` 是什麼?為什麼它**必須**是 Scoped,不能是 Singleton?(提示:回去看 Day 8 那個 captive dependency 的錯誤訊息)
    <!-- 是因為他是短期的服務不能被 Singletone 長期綁架並留在記憶體中嗎? -->
    1. `DbContext` 不是執行緒安全的 <- 最關鍵
        官方文件明確說它不支援並行操作。Singleton 會被所有請求同時使用,兩個請求同時查詢就會丟:
        InvalidOperationException: A second operation was started on this context instance
        before a previous operation completed.
        
    2. 交界邊界會混在一起 <- 最可怕, 因為是靜默的資料錯誤
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
        A：直接用 DbSet（IQueryable）
        SELECT "t"."Title"                  ← 只要一個欄位
        FROM "Todos" AS "t"
        WHERE NOT ("t"."IsDone")            ← 過濾在資料庫做
        LIMIT @p                            ← 只傳 10 筆
        → 拿到 10 筆，耗時 24 ms

        B：先 GetAll 再在記憶體過濾（你的 Repository）
        SELECT "t"."Id", "t"."IsDone", "t"."Title"   ← 全部欄位
        FROM "Todos" AS "t"                           ← 沒有 WHERE、沒有 LIMIT
        → 從資料庫搬了 10000 筆進記憶體，才過濾出 10 筆，耗時 92 ms 
        ```

    - `DbSet` 不負責寫入
        `db.Todo.Add(todo)` 只是標記, 真正寫入要等 `db.SaveChangeAsync()`, 那是 Unit of Work 模式。 Day9 的 `repo.Add()` 是立刻寫入
    - `DbSet<T>` 是通用的, `ITodoRepository` 是為 Todo 客製的




EF Core 是什麼?
- Entity Framework Core 微軟的 ORM(Object-Relational Mapper), 讓你用 C# 物件操作資料庫, 不用自己寫 SQL。
