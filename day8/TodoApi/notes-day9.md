
**練習 1:概念暖身(先寫筆記再動手)**
- Day 8 的 `TodoService` 其實同時扮演了**兩個角色**,是哪兩個?(提示:它既決定「規則」也負責「資料怎麼存」)
    `TodoService` 扮演了, 資料存取, 以及處理資料的規則兩個角色, 他應該只要負責處理資料的規則就好

- Endpoint / Service / Repository 三層各自的職責是什麼?什麼東西**不該**出現在每一層?
    Endpoint: <!--處理路由, 根據 url 及 method 來去處理相對應的事情-->
        職責: 解析 HTTP 請求、把結果翻譯成狀態碼
        不該出現: 業務規則、直接操作 SQL/List
    Service: <!-- 對資料的規則處理 -->
        職責: 業務規則、驗證、流程協調
        不該出現: `Restuls.Ok()`、`HttpContent`、`List.Add()`、SQL
    Repository: 資料怎麼儲存/取得, 把資料存取方式抽象掉 (只管 CRUD, 完全不知道業務規則)
        職責: 資料存取
        不該出現: 業務規則、HTTP 相關的任何東西

   * 為什麼 Service 不能有`Results.Ok()`?因為 Service 可能被 CLI 工具、背景排程、單元測試呼叫, 那些場合根本沒有 HTTP。
   把狀態碼寫進 Service, 等於假設「永遠只有 Web 會用我」

- 為什麼依賴方向是 `Endpoint → Service → Repository`,反過來會怎樣?
    <!-- 這個依賴方向我不是太明白, 但就我猜測應該是因為第一個遇到的一定會是 `Endpoint` 才能知道要做什麼處理, 而 Service 跟 Repository 的順序方向我就不知道了 -->
    依賴方向看的是: 誰的程式碼出現了誰的型別。
    ```csharp
    // Endpoint 的程式碼提到了 ITodoService
    app.MapGet("/todos", (ITodoService service) => ...);

    // Service 的程式碼提到了 ITodoRepository
    class TodoService(ITodoRepository repo) : ITodoService {}

    // Repository 的程式碼不提任何上層, 他根本不知道 Service 的存在
    class InMemoryTodoRepository() : ITodoRepository 
    {
        private readonly List<Todo> _list = [];
    }
    ```
    資料雙向流動, 但依賴單向。

    如果反過來讓 Repository 依賴 Service
    ```csharp
    class TodoRepository(ITodoService service) : ITodoRepository;   // ⚠️
    class TodoService(ITodoRepository repo) : ITodoService;
    // A circular dependency was detected for the service of type 'ITodoRepository'.
    // A circular dependency was detected for the service of type 'ITodoService'.
    ```
    DI 容器直接拒絕, 程式起不來。因為要建 Service 得先有 Repository, 要建 Repository 得先由 Service, 無解。
    除了這個硬性限制,單向依賴還換到三件事:
    低層可以獨立測試 —— 測 Repository 不用準備 Service
    低層可以獨立替換 —— 這就是練習 5 要驗證的:換 Repository,Service 一個字都不用改
    改動的影響範圍可預測 —— 改 Repository 內部實作,上層不受影響;反過來依賴的話,改哪裡都可能牽動全部

    ⭐ 真正的關鍵:依賴的是「介面」,不是「實作」
    ```csharp
            TodoService
             │ 依賴
             ↓
      ITodoRepository  ←─────┐ 實作
       （抽象/契約）          │
                    InMemoryTodoRepository
                    FileTodoRepository
                    EfTodoRepository
    ```
    注意箭頭方向:Service 和具體實作「都指向介面」,它們彼此不認識。
    這叫依賴反轉原則(SOLID 的 D)—— 「高層模組不應該依賴低層模組,兩者都應該依賴抽象」。
    這也是為什麼練習 5 換 Repository 只要改一行註冊:TodoService 從頭到尾只認得 ITodoRepository 這個介面,它根本不知道背後是 List、檔案還是 PostgreSQL。

    而這正是 Day 8 的 DI 容器在做的事 —— 容器是那個「負責把介面對應到實作」的角色。沒有 DI,依賴反轉就只能自己手動接線。


- 分層的**代價**是什麼?(別只寫好處 —— 想想檔案數量、追一個 bug 要跳幾層)
    - 分層的代價肯定是檔案會變多, 可能追蹤一個 bug 要從 Endpoint 再跳到 Service 再跳到 Repository 才能找到
    - 過度抽象 —— 只有一個實作卻硬包一層介面,是純粹的成本。ITodoRepository 之所以值得,是因為你真的會有第二個實作(練習 5 的 File 版、Day 10 的 EF 版)
    - 一個小改動要碰多個檔案 —— 新增一個欄位可能要改 record、Repository、Service、Endpoint 四個地方
    - 間接層讓追蹤變難 —— 「這個介面的實作到底是哪一個?」要回去看 DI 註冊才知道,IDE 的 Go to Definition 會停在介面

判斷原則: 分層是在「改動成本」和「理解成本」之間取捨。三個端點的小工具不需要方層;會長期維護、會換資料來源的專案才值得

- 對照 Day 8 練習 5 討論過的 Contracts 專案:那個「抽出中立的第三方」跟這裡的分層,是同一個原理嗎?
    我覺得是同一個原理, 就是為了分離出他們的職責, 才不會改壞一個就全部壞掉, 也能夠針對知道的部分去改什麼檔案(例如是資料處理規則有問題, 就去解決 Service)

    更精確的說法是:都在實踐「依賴抽象,而非依賴實作」。
    Contracts 那個圖是 Client → Contracts ← Api(兩邊都指向中間);分層是 Endpoint → Service → Repository(一條單向的鏈)。