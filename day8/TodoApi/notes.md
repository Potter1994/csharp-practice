**練習 1:概念暖身(先寫筆記再寫 code)**
用自己的話回答:
- `Program.cs` 裡 `WebApplication.CreateBuilder(args)` → `builder.Build()` → `app.Run()` 這三步各做了什麼?
    `WebApplication.CreateBuilder(args)`: 使用基本預設的設置建立起一個建立者
    `builder.Build()`: 使用該建立者建立出實例 web application
    `app.Run()`: 實例 web application 實際跑起來

- 什麼是 **middleware pipeline**?為什麼 `app.UseXxx()` 的**順序**會影響行為?(提示:想像成一層層包起來的洋蔥,請求進去、回應出來)
    `middleware pipeline`: 就是要一關接一關持續流下去, 照著順序執行所有的中間件
    為什麼順序會影響行為: 例如有些中間件會去解析 CORS, 然後後面使用到的才能正常使用, 所以順序很重要

- 什麼是 **DI(依賴注入)容器**?為什麼不自己 `new` 就好?
    DI(依賴注入)容器: 就是將容器建立時在將實例加進去, 不會把容器綁死, 能夠他容器靈活的應用他使要到的實例(做好正確的 interface or abstract 繼承確保有統一的介面能使用即可)


- `AddSingleton` / `AddScoped` / `AddTransient` 三種生命週期差在哪?各自適合什麼?
    我是去 Google 了才知道在講什麼, 我是不是需要先看一下相關的文章才來回答這個問題比較好?
    `AddSingleton`: 生命週期為整個應用結束才會關閉
    `AddScoped`: 生命週期為同一個 request 結束後才會關閉
    `AddTransient`: 每次 new 一個新的 service 就會關閉

    說實話我不懂他這邊 new 一個 service 意思是什麼?
    各自適合什麼我也不知道

- Minimal API 跟 Controller 寫法差在哪?各自適合什麼場景?
    這個我也不知道, 我是不是有什麼文章先看比較好? 如果像你這樣一邊問我一邊查是好的嗎? 我感覺這樣會沒辦法完整的學習到


- 對照 Day 6:你那時候是 HTTP **客戶端**,現在是**伺服器端**。同一個請求在兩邊分別經過什麼?
    客戶端一樣
    DNS -> TCP -> TLS -> HTTP request -> response -> render

    server 端
    收到 request -> 跑 server 端的程式碼看是否要去跟 database 拿取資料 -> 回送給 client 端



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