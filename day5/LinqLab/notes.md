**驗收標準**
- 能說出 `List` / `Dictionary` / `HashSet` 的差異與各自的適用場景,並用實測數字佐證查找效能的差別。
    `List` 適合用於儲存一般連續性的資料
    `Dirctionay` 適合一般儲存 key/value 的鍵值物件
    `HashSet` 去重 + 快速查找

    `List` 跟 `HashSet` 關鍵差距: 
        10 萬筆查 1 萬次:List 2676 ms vs HashSet 0.35 ms(約 7600 倍)
    關鍵不是倍數,而是 List 的 O(n) 會隨資料量惡化,HashSet 的 O(1) 不會

- 能解釋 **LINQ 的延遲執行**:查詢在什麼時候才真正跑、多次列舉會發生什麼、`ToList()` 的作用。
    LINQ 會在真正使用到他的時候才跑, 一般把它當成是一段敘述(不會在寫敘述的當下就馬上執行, 而是在使用到時才執行)
    多次列舉會再當下情況再次執行一次敘述
    `ToList` 的作用是會將在敘述當下執行且保存起來, 之後跟原陣列再無相關

- 能說出 `First` 和 `Single`、`First` 和 `FirstOrDefault` 的差別。
    `First` 尋找到第一個符合條件的
    `Single` 只能尋找只有一個符合, 否則拋出 `InvalidOperationException` 相關錯誤敘述
    `First` 跟 `FirstOrDefault` 差別在於如果找不到 `First` 會拋出 `InvalidOperationException` 錯誤, 而 `FirstOrDefault` 會回傳 null


- 用 LINQ 重構後的程式碼行數明顯減少,而且可讀性沒有下降。
    這跟我在 JS 學到 Function Programming 是一樣的道理, 不影響原本集合
    且是故意用與 MySQL 差不多的語法來去使用


**延伸**
- 查 `IEnumerable<T>` 和 `IQueryable<T>` 的差異(提示:一個在記憶體裡跑,一個會被翻譯成 SQL)——這直接銜接 Day 10 的 EF Core。
    我稍微看了一下, `IEnumerable<T>` 主要是在本地記憶體裡面跑
    而 `IQueryable<T>` 會將所有條件翻譯成高效的 SQL 語句, 送去資料庫執行, 只把結果傳回來

- 想一下 LINQ 方法鏈的效能:`Where().Where()` 和 `Where(a && b)` 哪個好?為什麼延遲執行讓這件事沒有想像中重要?
    `Where(a && b)` 好, 因為延遲執行能夠將 `Where().Where()` 整理成 `Where(a && b)` 嗎?

