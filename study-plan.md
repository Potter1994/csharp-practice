# 學習課表:C# 後端 + AI 應用 + AWS/Terraform + React

## 目標與方法
- **職涯需求導向**:C#/電腦原理、後端架構、AI/LLM 應用開發、AWS/Terraform、React 前端整合。
- **學習方式**:實作對照型,邊做邊學。**不排日期,依序完成,做完一個打一個勾。**
- **策略**:先跑過一輪完整全端專案(廣度優先),之後再回頭深化細節。這一輪結束後會排「深化階段」,不會學完就停。
- **貫穿專案**:一個 C# 後端 API(Todo/資料服務)→ 加 AI 功能 → 用 Terraform 部署到 AWS → React 前端串接。

進度標記方式:完成的項目把 `[ ]` 改成 `[x]`,並在後面補上完成日期;有卡關或想跳過的在後面加註記。

> 「Day N」是課程**單元編號**,不是日期,對應下方〈練習題詳解〉。

---

## 階段一:C# 基礎 + 電腦原理

- [x] **Day 1** C# 基本語法、型別系統 — 變數/型別、FizzBuzz 質數版、方法拆分與多載、`out` 參數、溫度轉換 CLI(2026-09-10 完成)
- [x] **Day 2** OOP:class/interface/繼承 — 圖書館借閱系統:封裝、抽象類別、多型、介面選擇性能力(2026-09-16 完成)
- [x] **Day 3** 電腦原理:記憶體、CPU、Stack vs Heap — value/reference type 實驗、`ref`、boxing、記憶體用量實測(2026-09-17 完成)
- [x] **Day 4** 電腦原理:作業系統、Process/Thread — race condition 與 `lock`、`Task`/`async`、`WhenAll` 並行查詢(2026-09-22 完成)
- [x] **Day 5** C#:LINQ、集合、Generics — 集合效能實測、LINQ 運算子、延遲執行、泛型約束、重構 Library(2026-09-30 完成)
- [x] **Day 6** 電腦原理:網路基礎(TCP/IP、HTTP) — 用 HttpClient 打公開 API(2026-10-05 完成)
- [x] **Day 7** 【複習/補進度】整理筆記,程式碼推上 GitHub(2026-10-05 完成)

## 階段二:後端架構設計(C# / ASP.NET Core)

- [x] **Day 8** ASP.NET Core 基礎、Minimal API — 建一個 Todo API(CRUD)(2026-10-08 完成)
- [ ] **Day 9** 分層架構(Controller/Service/Repository) — 重構 Todo API
- [ ] **Day 10** 資料庫:EF Core + PostgreSQL — 接資料庫取代記憶體儲存
- [ ] **Day 11** API 設計、RESTful、DTO/驗證 — 加輸入驗證、錯誤處理 middleware
- [ ] **Day 12** 身份驗證/授權(JWT) — 加登入與權限保護
- [ ] **Day 13** 單元測試(xUnit) — 幫 Service 層寫測試
- [ ] **Day 14** 【複習/補進度】用 Postman 完整測試 API

## 階段三:AI/LLM 應用開發(整合進 API)

- [ ] **Day 15** LLM 應用基礎、Prompt 設計 — 串接 Claude/OpenAI API,做摘要端點
- [ ] **Day 16** 結構化輸出、Function calling / Tool use — 讓 API 回傳結構化 JSON
- [ ] **Day 17** RAG 概念與簡單實作 — 向量搜尋 + LLM 做簡易問答
- [ ] **Day 18** Agent/多步驟工作流概念 — 設計會呼叫多個 API 的小型 agent 流程
- [ ] **Day 19** 成本、延遲、快取策略 — 加 caching,量測回應時間與 token 花費
- [ ] **Day 20** 整合測試 — AI 功能與 Todo API 整合測試
- [ ] **Day 21** 【複習/補進度】寫技術筆記記錄學到的 AI 應用模式

## 階段四:AWS + Terraform + DevOps + 前端串接

- [ ] **Day 22** AWS 基礎(EC2/RDS/S3/IAM 概念) — 建帳號、跑官方 hands-on 教學
- [ ] **Day 23** Docker 化 C# API — 寫 Dockerfile,本機跑容器
- [ ] **Day 24** Terraform 基礎(state、provider、resource) — 建簡單 AWS 資源(S3 bucket)
- [ ] **Day 25** Terraform 部署完整架構 — 部署 RDS + ECS/EC2 跑 API 容器
- [ ] **Day 26** CI/CD(GitHub Actions) — build → test → 部署 pipeline
- [ ] **Day 27** React 前端串接 — 呼叫部署好的 API(含 AI 功能)
- [ ] **Day 28** 【總複習 + Demo】前端 → API → AI → 資料庫,全部在 AWS 上跑一次

---

## 深化階段(第一輪結束後排入)
待第一輪完成後,依實際卡關/興趣點排下個月的深化主題,例如:
- AWS 更多服務、成本與安全最佳實踐
- Terraform 模組化、多環境管理
- AI Agent 進階設計模式
- C# 效能調校、更深的電腦系統原理

## 練習題詳解

### Day 1 ✅:C# 基本語法、型別系統

**環境設置**
1. 安裝 .NET SDK(建議最新 LTS 版),終端機執行 `dotnet --version` 確認安裝成功。
2. 建一個資料夾 `csharp-practice/day1`,在裡面執行 `dotnet new console -o Basics`,`cd Basics`,確認 `dotnet run` 能印出 "Hello, World!"。

**練習 1:變數與型別**
在 `Program.cs` 寫一段程式,涵蓋以下要求:
- 宣告至少 5 種不同型別的變數:`int`、`double`、`string`、`bool`、`char`。
- 用 `var` 宣告一個變數並用 `.GetType()` 印出推斷出的型別,驗證型別推斷結果。
- 宣告一個 `const` 常數(例如 `const double Pi = 3.14159;`),嘗試修改它,觀察編譯器報錯訊息並記下來。
- 宣告一個 nullable 型別(例如 `int? age = null;`),印出它,再用 `??` 運算子給預設值。

**練習 2:迴圈與條件判斷**
寫一個 FizzBuzz 的變化版:
- 印出 1~50,3 的倍數印 "Fizz",5 的倍數印 "Buzz",都是印 "FizzBuzz",都不是印數字本身。
- 額外要求:如果數字是質數,前面加註記 "(質數)"。這會逼你另外寫一個判斷質數的小邏輯,順便練習 `for`、`if/else`、`%` 運算子。
- 分別用 `for` 迴圈和 `while` 迴圈各寫一次,比較兩者寫法差異。

**練習 3:方法(Methods)**
把練習 2 的邏輯拆成方法,要求:
- 寫一個 `bool IsPrime(int number)` 方法。
- 寫一個 `string FizzBuzzOne(int number)` 方法,回傳單一數字該印的字串。
- 練習方法多載(overload):寫兩個同名 `Describe` 方法,一個吃 `int`,一個吃 `string`,印出不同描述文字。
- 練習 `out` 參數:寫一個 `bool TryParseAge(string input, out int age)`,模仿 `int.TryParse` 的行為(輸入合法數字回傳 true 並帶出年齡,否則回傳 false)。

**練習 4(整合小專案):簡易溫度轉換 CLI**
用今天學到的東西做一個小工具:
- 程式啟動後用 `Console.ReadLine()` 讓使用者輸入溫度數值與單位(C 或 F)。
- 寫一個方法把攝氏轉華氏、華氏轉攝氏。
- 輸入非數字或非法單位時要處理(可以先用簡單的 `if` 判斷,不用到 exception,exception 處理留到後面章節)。
- 用 `while` 迴圈讓使用者可以連續輸入多次,輸入 "exit" 才結束。

**驗收標準**
- FizzBuzz 質數版能正確跑完 1~50 且結果肉眼可驗證(例如 7 應顯示 "7(質數)")。
- 溫度轉換工具能正確處理至少 3 組合法輸入 + 1 組不合法輸入而不崩潰。
- 能口頭解釋(講給自己聽或寫成筆記):`var` 跟明確型別的差異、`const` 跟一般變數差異、`out` 參數的用途。

**延伸(有餘力再做)**
- 把質數判斷改成更有效率的寫法(只檢查到 √n),並想一下為什麼這樣比較快——這會自然銜接到 Day 3 的電腦原理/效能主題。

### Day 2 ✅:OOP — class/interface/繼承(圖書館借閱系統)

**練習 1:定義基本類別**
- 建立 `Book` 類別:屬性含 `Title`、`Author`、`ISBN`、`IsBorrowed`(private set,只能透過方法改變)。
- 建立 `Member` 類別:屬性含 `Name`、`MemberId`、一個 `List<Book>` 存放目前借閱的書。
- 練習封裝:確保外部不能直接把 `IsBorrowed` 設成 true/false,只能透過 `Book` 自己的方法改變狀態。

**練習 2:繼承與多型**
- 建立抽象類別 `LibraryItem`(含 `Title`、`ItemId`、抽象方法 `GetLoanPeriodDays()`)。
- 讓 `Book : LibraryItem`(借期 30 天)、`Magazine : LibraryItem`(借期 7 天)分別 override `GetLoanPeriodDays()`。
- 寫一個方法 `PrintDueDate(LibraryItem item)`,傳入不同子類別實例,觀察多型如何讓同一段程式碼跑出不同借期。

**練習 3:Interface**
- 定義 `IBorrowable` 介面,含 `Borrow(Member member)` 和 `Return()` 兩個方法。
- 讓 `LibraryItem` 實作 `IBorrowable`。
- 額外練習:定義 `IRenewable` 介面(含 `Renew()` 續借方法),只讓 `Book` 實作(Magazine 不可續借),體會 interface 讓不同類別「選擇性」擁有能力的用途。

**練習 4(整合小專案):完整借閱流程**
- 寫一個 `Library` 類別管理一份 `List<LibraryItem>` 館藏和 `List<Member>` 會員。
- 實作:借書(檢查是否已被借走)、還書、查詢某會員目前借了哪些書、查詢某本書是否可借。
- 全部書目/會員資料先用程式碼手動建立幾筆(不用資料庫,留到 Day 10)。
- 在 `Main` 裡跑一輪完整劇本:某會員借兩本書 → 印出借閱狀態 → 還一本書 → 再印一次狀態。

**驗收標準**
- 能借到已被借走的書時要能正確擋下並印出錯誤訊息,而不是讓狀態錯亂。
- 能解釋清楚:為什麼用抽象類別 `LibraryItem` 而不是每個類別各自獨立寫;`IBorrowable` 這種 interface 跟繼承的差異是什麼。

**延伸**
- 幫 `Book` 加上逾期罰金計算(用 `DateTime` 算超過借期幾天,一天罰多少錢),為 Day 4 的 DateTime/邏輯運算暖身。

---

> **背景知識:編譯型語言 vs 需要 Runtime 的語言**
> 語言分兩大類,差別在於程式碼怎麼變成 CPU 能執行的東西:
> - **編譯成原生機器碼(AOT)**:C、C++、Rust、Go、Swift。編譯器直接把程式碼轉成該顆 CPU(x86/ARM)看得懂的機器碼,作業系統直接載入執行,不需要另外安裝語言 runtime。代價是編出來的執行檔綁定特定作業系統 + CPU 架構,換到別的平台要重新編譯。
> - **編譯成中介語言,執行時再 JIT 轉換**:C#(.NET)、Java(JVM)。要有 runtime(CLR/JVM)幫忙把中介碼即時轉成機器碼。換到的好處是「一次編譯,裝了對應 runtime 的機器都能跑」。
> - **直譯型**:Python、JavaScript(Node)。每次執行都要有直譯器邊讀原始碼邊跑。
>
> 嚴謹一點說,即使是編譯成原生機器碼的執行檔,也還是依賴「作業系統」這個環境來載入、配置記憶體、處理系統呼叫——只是這一層本來就內建在電腦裡,不需要額外安裝。這跟今天(Day 3)、明天(Day 4)要學的 CPU 執行機器碼、OS 載入 process 直接相關。

### Day 3 ✅:電腦原理 — 記憶體、CPU、Stack vs Heap

**練習 1:Value Type vs Reference Type 實驗**
- 定義 `struct PointStruct { public int X, Y; }` 和 `class PointClass { public int X, Y; }`(結構相同)。
- 寫兩個方法 `ModifyStruct(PointStruct p)` 和 `ModifyClass(PointClass p)`,方法內都把 `X` 改成 999。
- 在 `Main` 分別呼叫,印出呼叫前後原始變數的 `X` 值,觀察並記錄:哪個變了、哪個沒變、為什麼(對照 stack 存的是值的複本、heap 上物件是共用參考)。

**練習 2:ref 關鍵字實驗**
- 把 `ModifyStruct` 改寫成 `ModifyStructByRef(ref PointStruct p)`,用 `ref` 呼叫,觀察這次原始變數的 `X` 有沒有變,理解 `ref` 如何強制以參考方式傳遞 value type。

**練習 3:Boxing/Unboxing 實驗**
- 寫 `int number = 42; object boxed = number;`,修改 `boxed` 內容後印出原本的 `number`,驗證兩者已經是獨立記憶體。
- 用 `Console.WriteLine` 觀察 boxing 前後,思考這個動作為什麼有額外的記憶體與效能成本(物件多了 heap 配置)。

**練習 4:記憶體用量觀察**
- 用 `GC.GetTotalMemory(true)` 在建立前後各印一次記憶體用量。
- 分別建立 100 萬個 `PointStruct` 陣列元素、100 萬個 `PointClass` 物件陣列,比較兩次記憶體差異,寫下你觀察到的數字與猜測的原因(class 物件在 heap 上每個都有額外的物件標頭開銷)。

**練習 5(整理筆記,不寫 code)**
用自己的話寫一小段筆記解釋:
- Stack 存什麼、Heap 存什麼、為什麼 Stack 存取比較快
- Value type 的變數什麼情況下其實也會被放到 heap(提示:當它是某個 class 的欄位,或被 box 的時候)

**驗收標準**
- 能準確預測(而不是跑了才知道)`ModifyStruct` 和 `ModifyClass` 各自會不會影響原始變數。
- 筆記能講清楚 boxing 為什麼有成本。

**延伸**
- 查一下 C# 的 `record struct` 和一般 `class` 的差異,跟今天學的概念做連結。

---

### Day 4 ✅:電腦原理 — 作業系統、Process/Thread(C# Task/async)

**練習 1:概念暖身(先寫筆記再寫 code)**
用自己的話寫一小段筆記區分:Process(行程)vs Thread(執行緒)的差異、為什麼多執行緒可以加速 I/O bound 的工作但不一定加速 CPU bound 的工作。

**練習 2:Thread 基礎與 Race Condition**
- 用 `System.Threading.Thread` 開兩條執行緒,各自把一個共享的 `int counter` 累加 100,000 次。
- 兩條執行緒都跑完後印出 `counter`,你會發現結果通常小於 200,000 — 記錄這個現象。
- 用 `lock` 保護 `counter++` 這段程式碼,重跑一次驗證結果變成正確的 200,000,理解 race condition 是怎麼發生、`lock` 怎麼解決。

**練習 3:Task 與 async/await**
- 寫一個模擬 I/O 的方法 `async Task<string> FetchDataAsync(string name, int delayMs)`,內部用 `await Task.Delay(delayMs)` 模擬等待,回傳一段字串。
- 依序(await 三次,一個接一個)呼叫三次不同 delay 的 `FetchDataAsync`,印出總花費時間。
- 改用 `Task.WhenAll` 同時發出三個呼叫,再印出總花費時間,比較跟依序執行的差異,理解 async 如何讓 I/O 等待時間重疊。

**練習 4(整合小專案):串接 Day 2 的圖書館系統**
- 把 Day 2 的 `Library` 加一個模擬「查詢多個分館庫存」的情境:寫 3 個 `async` 方法模擬向 3 個分館查詢某本書庫存(各自用不同的 `Task.Delay` 模擬網路延遲)。
- 用 `Task.WhenAll` 同時查詢 3 個分館,彙整結果印出「哪個分館有現貨」。

**驗收標準**
- 能清楚說出 race condition 發生的原因,以及 `lock` 如何解決(不是死背,是能解釋機制)。
- `Task.WhenAll` 版本的總花費時間應明顯少於依序 await 版本(觀察並記下實際測量的秒數差異)。

**延伸**
- 查一下 `async/await` 底層跟 Thread 的關係(提示:`await` 不等於「開一條新執行緒」,這是很多人常見的誤解),寫一小段筆記澄清這個觀念。

---

### Day 5 ✅:C# — LINQ、集合、Generics

**練習 1:認識集合型別**
- 分別用 `List<T>`、`Dictionary<K,V>`、`HashSet<T>`、`Queue<T>`、`Stack<T>` 各寫一小段,體會它們各自解決什麼問題。
- 效能實驗:建立 10 萬筆資料,分別用 `List.Contains()` 和 `HashSet.Contains()` 查找 1 萬次,用 `Stopwatch` 量測並記錄差距。想一下為什麼(提示:一個是逐筆比對,一個是雜湊表)。
- 寫下判斷表:什麼情況該用哪一個。

**練習 2:LINQ 基礎運算子**
用一組測試資料(例如 Day 2 的書籍清單)練習以下方法,每個都寫一次:
- 篩選:`Where`
- 投影:`Select`、`SelectMany`
- 排序:`OrderBy`、`OrderByDescending`、`ThenBy`
- 取單一元素:`First`、`FirstOrDefault`、`Single`、`SingleOrDefault`(注意四者差異)
- 判斷:`Any`、`All`、`Contains`
- 彙總:`Count`、`Sum`、`Average`、`Max`、`Min`
- 分組:`GroupBy`
- 轉換:`ToList`、`ToArray`、`ToDictionary`

每一個都跟你熟悉的 JavaScript 陣列方法對照(`filter`/`map`/`sort`/`find`/`some`/`every`/`reduce`),記下哪些有對應、哪些沒有。

**練習 3:延遲執行(這題最重要)**
- 寫一個 `Where` 查詢但**不要**呼叫 `ToList()`,在查詢後修改原始集合,再列舉查詢結果 —— 觀察印出來的是修改前還是修改後的資料。
- 在 `Where` 的條件裡放一行 `Console.WriteLine`,觀察它在哪一刻才被執行。
- 對同一個查詢變數 `foreach` 兩次,數一下條件被執行了幾次。
- 寫下結論:什麼時候查詢才真正執行?`ToList()` 改變了什麼?

**練習 4:Generics**
- 寫一個泛型方法 `T? FindMax<T>(IEnumerable<T> items) where T : IComparable<T>`,回傳最大值。
- 寫一個泛型類別 `SimpleRepository<T>`,內含 `Add`、`GetAll`、`FindById` —— 為第二週的 Repository 層暖身。
- 試著加上不同的泛型約束(`where T : class`、`where T : struct`、`where T : new()`),觀察各自允許什麼。

**練習 5(整合小專案):用 LINQ 重構 Day 2 的圖書館**
- 把 `Library.SearchBorrowedListByMember` 的 `foreach` + 字串拼接改成 LINQ + `string.Join`。
- 把 `CheckItem` 的 `Find` 改成 LINQ 寫法。
- 新增三個查詢:借最多書的會員、所有逾期未還的項目、按館藏類型(Book/Magazine)分組統計數量。
- 比較重構前後的行數與可讀性。

**驗收標準**
- 能說出 `List` / `Dictionary` / `HashSet` 的差異與各自的適用場景,並用實測數字佐證查找效能的差別。
- 能解釋 **LINQ 的延遲執行**:查詢在什麼時候才真正跑、多次列舉會發生什麼、`ToList()` 的作用。
- 能說出 `First` 和 `Single`、`First` 和 `FirstOrDefault` 的差別。
- 用 LINQ 重構後的程式碼行數明顯減少,而且可讀性沒有下降。

**延伸**
- 查 `IEnumerable<T>` 和 `IQueryable<T>` 的差異(提示:一個在記憶體裡跑,一個會被翻譯成 SQL)——這直接銜接 Day 10 的 EF Core。
- 想一下 LINQ 方法鏈的效能:`Where().Where()` 和 `Where(a && b)` 哪個好?為什麼延遲執行讓這件事沒有想像中重要?

---

### Day 6 ✅:電腦原理 — 網路基礎(TCP/IP、HTTP)+ HttpClient

**練習 1:概念暖身(先寫筆記再寫 code)**
用自己的話回答:
- 在瀏覽器輸入一個網址到畫面出現,中間經過哪些步驟?(DNS → TCP 連線 → TLS → HTTP 請求 → 回應 → 渲染)
- TCP 和 UDP 差在哪?各自適合什麼場景?
- 什麼是「三向交握」?為什麼需要它?
- HTTP 狀態碼的五個級距(1xx~5xx)各代表什麼?分別舉兩個常見的例子。
- HTTP/1.1、HTTP/2、HTTP/3 的主要差異是什麼?(不用深入,知道各自解決什麼問題即可)

**練習 2:用 HttpClient 打公開 API**
用 `https://jsonplaceholder.typicode.com` 這個免費測試 API(不需金鑰):
- `GET /posts/1` 取得單筆資料,把 JSON 反序列化成一個 `record`。
- `GET /posts` 取得全部,反序列化成 `List<T>`。
- `POST /posts` 送出一筆新資料,觀察回應。
- 印出回應的 **狀態碼**、**部分 header**(例如 `Content-Type`)、**body**。
- 用 `System.Text.Json` 的 `JsonSerializer`,並設定 `PropertyNameCaseInsensitive = true` 觀察差異。

**練習 3:HttpClient 的生命週期(這題是實務重點)**
- 查一下為什麼 `using var client = new HttpClient()` 是**錯誤**用法,以及 socket exhaustion 是什麼。
- 查一下為什麼把 `HttpClient` 當成 `static` 單例長期重用,又會遇到 DNS 變更的問題。
- 寫下結論:實務上該怎麼用?(提示:`IHttpClientFactory`,Day 8 會正式用到)
- 實驗:用同一個 `HttpClient` 實例連續打 5 次請求,跟每次都 new 一個比較耗時。

**練習 4:錯誤處理與逾時**
- 故意打一個不存在的路徑(例如 `/posts/99999`),觀察回傳什麼狀態碼、`EnsureSuccessStatusCode()` 會發生什麼。
- 設定 `HttpClient.Timeout`,打一個會慢的端點,觀察逾時的例外類型。
- 用 `CancellationTokenSource` 實作「3 秒後取消請求」。
- 寫下:`HttpRequestException`、`TaskCanceledException`、`TimeoutException` 各在什麼情況出現?

**練習 5(整合小專案):並行抓取 + LINQ 處理**
把 Day 4 和 Day 5 學的東西串起來:
- 用 `Task.WhenAll` **同時**抓取多筆資料(例如 `/posts/1` ~ `/posts/10`)。
- 跟「依序抓 10 次」比較總耗時,記下數字。
- 用 LINQ 處理回傳的資料:依 `userId` 分組、找出標題最長的一篇、統計每個使用者的文章數。
- 把結果用 `Dump`(JSON 序列化)印出來。

**驗收標準**
- 能講清楚一個 HTTP 請求從發出到收到回應,中間發生了什麼(對照 Day 4 學的 I/O 機制)。
- 能說出 `HttpClient` 的正確用法與兩種錯誤用法各自的後果。
- 並行版本的總耗時應明顯少於依序版本(記下實際數字)。
- 能正確處理「請求失敗」與「逾時」兩種情況,程式不會崩潰。

**延伸**
- 用瀏覽器 DevTools 的 Network 面板打開同一個 API,對照 C# 收到的 header 和狀態碼。
- 查一下 REST 的幾個原則,以及 `GET`/`POST`/`PUT`/`PATCH`/`DELETE` 的語意差異(冪等性)——這直接銜接 Day 11 的 API 設計。

### Day 7 ✅:複習/補進度 — 整理筆記,程式碼推上 GitHub

- 補上 day1、day2 的 `notes.md`(前六天只有 day3~day6 有筆記)。
- 修正 day5、day6 筆記裡不精確的段落。
- 建立 `.gitignore`(排除 `obj`/`bin`,2.6M)、`README.md`,推上 GitHub。

### Day 8 ✅:ASP.NET Core 基礎、Minimal API — Todo API(CRUD)

**環境設置**
1. `cd day8`,執行 `dotnet new web -o TodoApi`(`web` 範本就是 Minimal API,不要用 `mvc` 或 `webapi`)。
2. `dotnet run` 確認能啟動,記下它印出的 port。用瀏覽器或 `curl` 打一次根路徑確認有回應。

**練習 1:概念暖身(先寫筆記再寫 code)**
用自己的話回答:
- `Program.cs` 裡 `WebApplication.CreateBuilder(args)` → `builder.Build()` → `app.Run()` 這三步各做了什麼?
- 什麼是 **middleware pipeline**?為什麼 `app.UseXxx()` 的**順序**會影響行為?(提示:想像成一層層包起來的洋蔥,請求進去、回應出來)
- 什麼是 **DI(依賴注入)容器**?為什麼不自己 `new` 就好?
- `AddSingleton` / `AddScoped` / `AddTransient` 三種生命週期差在哪?各自適合什麼?
- Minimal API 跟 Controller 寫法差在哪?各自適合什麼場景?
- 對照 Day 6:你那時候是 HTTP **客戶端**,現在是**伺服器端**。同一個請求在兩邊分別經過什麼?

**練習 2:第一個端點**
- 把範本預設的 `app.MapGet("/", () => "Hello World!")` 讀懂,然後自己加:
  - `GET /hello?name=xxx` — 從 query string 取值並回傳問候語
  - `GET /hello/{name}` — 改從路由參數取值,比較兩者寫法
- 用 `curl` 測試(不要只用瀏覽器,瀏覽器只能發 GET)。

**練習 3:Todo CRUD(本日主線)**
先定義 `record Todo(int Id, string Title, bool IsDone);`,資料先存在記憶體的 `List<Todo>`(資料庫留到 Day 10)。
實作五個端點,**狀態碼要正確**:

| 方法 | 路徑 | 成功回傳 | 失敗回傳 |
|---|---|---|---|
| `GET` | `/todos` | 200 + 陣列 | — |
| `GET` | `/todos/{id}` | 200 + 單筆 | 404 |
| `POST` | `/todos` | **201** + `Location` header | 400(標題空白) |
| `PUT` | `/todos/{id}` | 204 No Content | 404 |
| `DELETE` | `/todos/{id}` | 204 No Content | 404 |

- 用 `Results.Ok()` / `Results.Created()` / `Results.NotFound()` / `Results.NoContent()` / `Results.BadRequest()`。
- 對照 Day 6 寫的冪等性筆記:驗證 `PUT` 同一筆打兩次結果相同、`POST` 打兩次會建出兩筆。

**練習 4:DI 與服務分離**
- 把 `List<Todo>` 的操作抽成 `ITodoService` + `TodoService`,端點只負責接請求、回傳結果。
- 用 `builder.Services.AddSingleton<ITodoService, TodoService>()` 註冊,端點用參數注入取得。
- **實測三種生命週期的差別**:做一個 `IGuidService` 只回傳一個建構時產生的 `Guid`,分別註冊成 Singleton / Scoped / Transient,在同一個端點注入兩次並印出來,觀察:
  - 同一個請求內兩個 Guid 是否相同?
  - 不同請求之間是否相同?
  - 記下結果,解釋為什麼。

**練習 5(整合小專案):用 Day 6 的 HttpClient 測自己的 API**
- 另開一個 console 專案(或沿用 `day6/HttpLab`),用 `HttpClient` 對自己的 Todo API 跑一輪完整流程:
  `POST` 建立 → `GET` 確認 → `PUT` 更新 → `GET` 確認 → `DELETE` → `GET` 應得到 404。
- 每一步印出狀態碼,驗證跟練習 3 的表格一致。
- 這題同時複習 Day 6 的 `GetFromJsonAsync` / `PostAsJsonAsync` 與例外處理。

**驗收標準**
- 五個端點都能正確運作,狀態碼符合上表(用 curl 或練習 5 的 client 驗證)。
- 能解釋 middleware 順序為什麼重要,舉一個順序寫反會出錯的例子。
- 能說出 Singleton / Scoped / Transient 的差別,並用練習 4 的實測數字佐證。
- 能解釋為什麼 `POST` 回傳 201 而不是 200,以及 `Location` header 的作用。

**延伸**
- 打開 OpenAPI(.NET 9+ 內建 `builder.Services.AddOpenApi()`),看看自動產生的 API 文件。
- 查 `app.MapGroup("/todos")` 路由群組怎麼用,把五個端點收斂起來。
- 想一下:現在所有邏輯都塞在 `Program.cs` 裡,如果有 50 個端點會怎樣?(這就是 Day 9 分層架構要解決的問題)

### Day 9:分層架構(Endpoint / Service / Repository)— 重構 Todo API

**環境設置**
- 直接在 `day8/TodoApi` 上重構,不要開新專案(重構的重點就是「同樣的功能、不同的結構」)。
- 重構前先確認五個端點都正常,重構後用同一份 `.http` 或 `TodoClient` 再跑一次,行為必須完全一樣。

**練習 1:概念暖身(先寫筆記再動手)**
- Day 8 的 `TodoService` 其實同時扮演了**兩個角色**,是哪兩個?(提示:它既決定「規則」也負責「資料怎麼存」)
- Endpoint / Service / Repository 三層各自的職責是什麼?什麼東西**不該**出現在每一層?
- 為什麼依賴方向是 `Endpoint → Service → Repository`,反過來會怎樣?
- 分層的**代價**是什麼?(別只寫好處 —— 想想檔案數量、追一個 bug 要跳幾層)
- 對照 Day 8 練習 5 討論過的 Contracts 專案:那個「抽出中立的第三方」跟這裡的分層,是同一個原理嗎?

**練習 2:拆出 Repository 層**
- 定義 `ITodoRepository`,把「資料怎麼存取」從 `TodoService` 搬過去。
- 寫 `InMemoryTodoRepository` 實作它,`List<Todo>` 和 `lock` 都搬到這一層。
- `TodoService` 改成建構函式注入 `ITodoRepository`,它從此**不知道資料存在哪**。
- 兩個都要註冊到 DI 容器。
- 想一下:`lock` 為什麼該在 Repository 而不是 Service?

**練習 3:端點搬出 `Program.cs`**
- 寫一個擴充方法 `public static void MapTodoEndpoints(this WebApplication app)`,把五個端點搬進去(放在 `Endpoints/TodoEndpoints.cs`)。
- 用 `app.MapGroup("/todos")` 收斂共同前綴,端點路徑只剩 `""` 和 `"/{id}"`。
- 目標:`Program.cs` 回到 10 行以內。

**練習 4:業務邏輯該放哪一層**
- 把「標題不能空白」的驗證從端點移到 `TodoService`。端點從此只做一件事:**把 Service 的結果翻譯成 HTTP 狀態碼**。
- 問題來了:`Update` 現在回傳 `bool`,但失敗有兩種原因 —— 「找不到」要回 404、「標題空白」要回 400,`bool` 分不出來。想一個辦法讓 Service 能表達「為什麼失敗」。
  (提示:可以回傳 enum、自訂的 Result 型別、或 tuple。先自己想,再查 "Result pattern")
- 這題的重點不是哪個寫法最好,是體會「**Service 不該知道 HTTP 狀態碼,但要能表達失敗原因**」。

**練習 5(整合小專案):換一個 Repository 實作**
- 寫 `FileTodoRepository`,把資料存成 JSON 檔(用你 Day 6 的 `System.Text.Json`)。
- **只改 `Program.cs` 註冊的那一行**切換實作,其他程式碼一個字都不能動。
- 驗證:重啟程式後資料還在;再切回 `InMemory`,行為一樣正確。
- 這就是 Day 10 換成 EF Core 時會做的事 —— 介面不變,換掉實作。

**驗收標準**
- `Program.cs` 在 10 行以內,五個端點行為跟重構前完全相同。
- 能說出三層各自的職責,以及「這段程式碼該放哪一層」的判斷依據。
- 切換 `InMemory` ↔ `File` 兩種 Repository 只需要改一行註冊。
- 能解釋為什麼 `TodoService` 不該出現 `Results.Ok()`、`ITodoRepository` 不該出現 `HttpContext`。

**延伸**
- 把同一套功能用 Controller 寫一次(`dotnet new webapi --use-controllers`),對照 Minimal API 的分層差在哪。
- 查一下 Repository 模式的爭議:有人認為 EF Core 的 `DbSet` 本身就是 Repository,再包一層是多餘的。看完之後寫下你的看法。

## 調整紀錄
> 之後每次討論學習狀況時,在這裡加一筆日期 + 調整內容,方便回顧課表怎麼演變。

- 2026-08-11:初版課表建立。
- 2026-09-10:Day 1 完成,練習 1~4 全數通過驗收(FizzBuzz 質數版、方法拆分/多載、`out` 參數、溫度轉換 CLI)。實際開工日比原訂晚約一個月,課表日期全部重排。學習節奏改為每天完成 2~3 天份內容,結束日由 09/07 調整為 09/22。
- 2026-09-16:Day 2 完成(練習 1~4)。圖書館借閱系統涵蓋:屬性 vs 欄位的封裝、`IReadOnlyList` 包裝集合、抽象類別 `LibraryItem` 與多型、`IBorrowable`/`IRenewable` 介面的選擇性能力、`Library` 協調層。
  實際節奏與預期不符:Day 1(09/10)到 Day 2(09/16)花了 6 天完成 1 個單元,而非原訂的每天 2~3 個單元。Day 3 之後的日期尚未重排,下次討論時再一起調整。
- 2026-09-16:改為**不排日期**制。原本的日期排程(每天 2~3 單元)與實際節奏不符,且每次落後都要重排一次,徒增雜訊。現在只保留單元順序,完成時打勾並記錄完成日期。先前合併的 Day 2-3、Day 4-5 等區塊也拆回單一單元,方便一次打一個勾。
- 2026-09-17:Day 3 完成(練習 1~5)。釐清「C# 與 JS 預設都是傳值,差別在值是地址還是資料」、`ref`/`out`/`in` 的別名語意、boxing 成本。練習 4 實測 100 萬個物件:struct 陣列 7 MB vs class 陣列 53 MB,其中 21 MB 是 GC 堆管理開銷(理論資料量僅 32 MB)。筆記在 `day3/MemoryLab/notes.md`。
- 2026-09-22:Day 4 完成(練習 1~4 + 延伸)。Process/Thread 的記憶體分工、I/O bound vs CPU bound、race condition 與 `lock` 的原子性、`await` 等待期間零執行緒、`Task.Run` 與執行緒集區。實測:依序 await 3012 ms vs `Task.WhenAll` 1501 ms;100 個 `Task.Run` 只用 11 條執行緒。day4 專案透過 `<ProjectReference>` 引用 day2 的 `Library`。筆記在 `day4/AsyncLab/notes.md`。
- 2026-09-30:Day 5 完成(練習 1~5 + 延伸)。集合實測 List O(n) vs HashSet O(1) 差約 7600 倍;LINQ 延遲執行(查詢 3 次 = 條件執行 21 次)、`ToList()` 的快照語意;泛型約束 `IComparable<T>`/`IEntity`/`new()`;`SimpleRepository<T>` 為第二週 Repository 層暖身;用 LINQ 重構 day2 的 `Library`。延伸釐清 `Expression<Func<>>` 是 `IQueryable` 能翻譯成 SQL 的關鍵。筆記在 `day5/LinqLab/notes.md`。
- 2026-10-05:Day 6 完成(練習 1~5 + 延伸)。網路基礎:TCP/UDP 的取捨(UDP 不是「比較快」,而是延遲低 —— 不等重傳也不等按序交付)、三向交握真正的目的是交換初始序號 ISN、HTTP/1.1→2→3 的演進(持久連線 → 多工 + HPACK → QUIC 建在 UDP 上,繞過 TCP 寫死在作業系統裡的按序交付)。`HttpClient` 生命週期:連線池其實在底層的 `SocketsHttpHandler` 上而不是 `HttpClient` 本身,所以每次 `new` 都是一個新的空池子;而 static 單例又因為連線一直活著、從來沒有重新 DNS 解析的時機,解法是設 `PooledConnectionLifetime`(等同 `IHttpClientFactory` 的核心行為)。逾時丟的是 `TaskCanceledException` 而非 `TimeoutException`,要靠 `InnerException is TimeoutException` 才分得出「逾時」與「主動取消」。實測:依序抓 10 筆 1216 ms vs `Task.WhenAll` 363 ms;冷連線 WhenAll 641 ms,但暖身後去抓「從沒抓過的網址」只要 91 ms —— 證明差異來自 TCP/TLS 連線重用,不是內容快取。延伸對照 DevTools 發現 `HttpClient` 預設連一個 request header 都不送(瀏覽器會送十幾個),以及回應裡的 `Alt-Svc: h3`(伺服器支援 HTTP/3 但 .NET 預設不升級)、`ETag`、`cf-cache-status`。補上冪等性與 REST 原則:冪等性決定「請求失敗時能不能重試」,`GET`/`PUT`/`DELETE` 可以安心重試,`POST` 要靠 idempotency key。筆記在 `day6/HttpLab/notes.md`。
- 2026-10-05:Day 7 完成(複習/整理筆記 + 推上 GitHub)。補上 day1、day2 的 `notes.md`(原本只有 day3~day6 有)。修正前幾天筆記的精確度:`FirstOrDefault` 對實值型別回傳 `default(T)` 而非 null、`ToList()` 是淺層複製(元素仍共用)、`Where().Where()` 不會被合併成 `Where(a && b)`(但延遲執行讓資料只走一遍,不像 JS 會產生中間陣列);day6 的「`HttpClient` 沒有連線池」更正為「連線池在底層的 `SocketsHttpHandler` 上,每次 `new` 都是新的空池子」,並補上冪等性與「逾時能不能重試」的關聯。釐清 `=>` 是 expression-bodied member 不是 lambda(Release 下 IL 與 `{ return ...; }` 完全相同)、local function 捕捉外部變數用 struct 閉包而 lambda 用 class。建立 `.gitignore`(排除 2.6M 的 obj/bin)與 `README.md`,推上 `Potter1994/csharp-practice`。
- 2026-10-08:Day 8 完成(練習 1~5)。Minimal API 基礎:參數綁定的判斷順序(簡單型別看型別有沒有 `TryParse`、複雜型別落到 body、DI 服務從容器取)、`IResult` 的 `ExecuteAsync` 才是真正寫入回應的地方(自己實作一個 20 行的 `IResult` 行為與 `Results.Ok` 完全相同)、middleware pipeline 的洋蔥進出實測、`CreateBuilder` 預設註冊 111 個服務。Todo CRUD 五個端點狀態碼驗收全對(201 + `Location`、204、404),並實測冪等性:`POST` 兩次建出兩筆不同 id、`PUT` 三次結果完全相同;另確認「有 id」不是冪等的原因,關鍵在操作是「設定絕對值」還是「在現值上做變化」。DI 三種生命週期實測:同一請求內 Singleton/Scoped 相同、Transient 不同,跨請求只有 Singleton 相同。踩到 Singleton + `List<T>` 的執行緒安全問題:200 個並行 POST 掉 4 筆資料而且**沒有任何例外**;`Update`/`Delete` 把 `FindIndex` 寫在 lock 外造成 14 次 `NullReferenceException`(TOCTOU),改成查詢與寫入同在一個 lock、`GetAll` 回傳鎖內複製的快照後解決。確認 captive dependency:Singleton 注入 Scoped 會在 `Build()` 階段直接拋例外 —— Day 10 把 `TodoService` 改成 Scoped 之後,這些 lock 就不再需要。筆記在 `day8/TodoApi/notes.md`。
