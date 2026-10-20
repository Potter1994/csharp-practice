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
- [x] **Day 9** 分層架構(Controller/Service/Repository) — 重構 Todo API(2026-10-11 完成)
- [x] **Day 10** 資料庫:EF Core + PostgreSQL — 接資料庫取代記憶體儲存(2026-10-16 完成)
- [x] **Day 11** API 設計、RESTful、DTO/驗證 — 加輸入驗證、錯誤處理 middleware(2026-10-20 完成)
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

### Day 9 ✅:分層架構(Endpoint / Service / Repository)— 重構 Todo API

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

### Day 10 ✅:資料庫 — EF Core + PostgreSQL

**環境設置**
1. 用 Docker 跑一個 PostgreSQL(不需要懂 Docker,照抄即可;Day 23 才會正式學):
   `docker run --name todo-postgres -e POSTGRES_PASSWORD=dev123 -e POSTGRES_DB=tododb -p 5432:5432 -d postgres:17`
   - `docker ps` 確認容器在跑;`docker stop todo-postgres` / `docker start todo-postgres` 停跑;`docker rm -f todo-postgres` 砍掉重來。
2. 裝 EF Core 的命令列工具(全域,只要一次):`dotnet tool install --global dotnet-ef`
3. 在 `day8/TodoApi` 裝兩個套件:
   - `dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL`(PostgreSQL 的 provider)
   - `dotnet add package Microsoft.EntityFrameworkCore.Design`(產生 migration 用)

**練習 1:概念暖身(先寫筆記再動手)**
- **ORM** 解決什麼問題?它的**代價**是什麼?(別只寫好處)
- `DbContext` 是什麼?為什麼它**必須**是 Scoped,不能是 Singleton?(提示:回去看 Day 8 那個 captive dependency 的錯誤訊息)
- **Migration** 是什麼?為什麼不直接手寫 SQL 建表就好?
- **變更追蹤(change tracking)** 是什麼?為什麼改了物件的屬性之後,只要呼叫 `SaveChangesAsync()` 就會寫回資料庫?
- 對照 Day 9:`DbSet<Todo>` 跟你自己寫的 `ITodoRepository`,**像在哪、不像在哪**?

**練習 2:接上資料庫**
- 建立 `AppDbContext : DbContext`,裡面放一個 `public DbSet<Todo> Todos { get; set; }`。
- 連線字串寫在 `appsettings.json`,**不要寫死在程式碼裡**:
  `"ConnectionStrings": { "Default": "Host=localhost;Port=5432;Database=tododb;Username=postgres;Password=dev123" }`
- 在 `Program.cs` 註冊:`builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));`
- 查一下 `AddDbContext` 預設註冊成哪種生命週期,跟你 Day 8 的實測對照。

**練習 3:Migration**
- `dotnet ef migrations add InitialCreate` —— 打開產生的 `Migrations/` 資料夾,**讀懂 `Up()` 和 `Down()` 在做什麼**。
- `dotnet ef database update` —— 把表實際建到資料庫。
- 進去資料庫確認表建好了:`docker exec -it todo-postgres psql -U postgres -d tododb -c "\d \"Todos\""`
- **再做一次**:幫 `Todo` 加一個 `DateTime CreatedAt` 欄位,重新 `migrations add AddCreatedAt` + `database update`,比較第二份 migration 跟第一份差在哪。
- 想一下:`Down()` 存在的意義是什麼?

**練習 4(本日主線):`EfTodoRepository`** —— 收 Day 9 留下的伏筆
- 寫 `EfTodoRepository : ITodoRepository`,**介面一個字都不要改**。
- `Program.cs` **只改註冊那一行**切換過去。
- ⚠️ 這時候會踩到一個啟動就炸的錯誤 —— `TodoService` 現在是 `Singleton`,但它要注入的 Repository 依賴 Scoped 的 `DbContext`。想清楚為什麼,然後修好它。
- 修好之後:**把 `lock` / `SemaphoreSlim` 全部拿掉**,並在筆記裡解釋為什麼現在不需要了。
- 跑 `TodoClient` 驗證五個端點行為完全不變。

**練習 5(整合小專案):看見 EF Core 真正送出的 SQL**
- 在 `appsettings.Development.json` 把 EF Core 的 log 等級開到 `Information`,讓它印出每一句 SQL。
- 打一次 `GET /todos`,看它送出什麼 SQL。
- 然後做這個對照實驗,**這題是 Day 9 延伸題的答案**:
  - A:走你的 Repository —— `GetAllAsync()` 之後在記憶體裡 `.Where(t => !t.IsDone).Select(t => t.Title).Take(10)`
  - B:直接用 `DbSet` —— `db.Todos.Where(t => !t.IsDone).Select(t => t.Title).Take(10).ToListAsync()`
  - 比較兩者送出的 SQL 有什麼不同,資料量大時差在哪。
- 寫下你對「EF Core 上面要不要再包 Repository」的看法(Day 9 延伸題欠的那題)。

**驗收標準**
- 資料存在 PostgreSQL —— **停掉程式、甚至停掉容器再啟動,資料都還在**。
- 能解釋 `DbContext` 為什麼必須是 Scoped,以及把 `TodoService` 改成 Scoped 之後為什麼 `lock` 就不需要了。
- 能讀懂 migration 的 `Up()`/`Down()`,以及 EF Core 為一次 `GET /todos` 產生的 SQL。
- 五個端點行為跟 Day 9 完全一致(用 `TodoClient` 或 `.http` 驗證)。
- 能用自己跑出來的 SQL 說明「包 Repository」的代價。

**延伸**
- 幫 `Todo` 加一個關聯(例如 `Category`),體會 `Include()` 和 **N+1 查詢問題**。
- 查 `AsNoTracking()` 是什麼、什麼時候該用 —— 這跟變更追蹤的成本直接相關。
- 查一下 `dotnet ef migrations script` 能做什麼(提示:正式環境通常不跑 `database update`)。

### Day 11 ✅:API 設計、RESTful、DTO/驗證 — 加輸入驗證、錯誤處理 middleware

**練習 1:概念暖身(先寫筆記再動手)**
- **DTO** 是什麼?為什麼不該直接把 `Todo` 當成 API 的輸入與輸出?
  (提示:你現在的 `GET /todos` 會把 `CreatedAt` 吐給前端。如果哪天 `Todo` 多了 `OwnerId`、`InternalNote`,或 Day 12 加了密碼雜湊欄位,會發生什麼事?)
- 你其實已經有一個 DTO 了 —— `CreateTodoRequest`。想兩件事:為什麼它**不該**有 `Id`?為什麼它有 `IsDone` 其實有點可疑?
- **驗證該放在哪一層?** 你現在的 `IsNullOrWhiteSpace` 檢查在 `TodoService`。下面三種規則是同一類東西嗎?
  - 「Title 不能空白」「Title 不能超過 200 字」
  - 「已完成的 Todo 不能再改標題」
  - 「同一個使用者不能有兩筆同名的 Todo」
- 查 **ProblemDetails**(RFC 9457,舊編號 RFC 7807)。它解決什麼問題?為什麼需要一個「標準的錯誤回應格式」?
- 回頭看 Day 6 的冪等性筆記:`PUT` 跟 `PATCH` 的語意差在哪?你現在的 `PUT /todos/{id}` 如果只想改 `IsDone` 一個欄位,用 `PUT` 恰當嗎?

**練習 2:拆出 Response DTO**
- 現在五個端點直接回傳領域模型 `Todo`。新增一個 `TodoResponse`,只包含前端真的需要的欄位。
- 問題來了:**轉換放在哪一層?** Endpoint / Service / Repository?先自己決定再往下看。
  (提示:`ITodoService` 現在回傳 `Result<Todo>`。如果改成 `Result<TodoResponse>`,Service 就知道了 API 的輸出格式 —— 那是好事還是壞事?回去看 Day 9 把 `CreateTodoRequest` 從 `ITodoRepository` 移走的理由。)
- 做完回答:如果 `Todo` 加一個 `InternalNote` 欄位,要改幾個檔案?會不會意外洩漏給前端?

**練習 3:輸入驗證**
- 把 `TodoService` 裡手寫的 `string.IsNullOrWhiteSpace(request.Title)` 換成宣告式驗證。先查清楚:**.NET 10 的 Minimal API 有沒有內建的驗證支援?**
  (提示:查 `AddValidation`,以及 `[Required]`/`[StringLength]` 這些 Data Annotations 在 Minimal API 上會不會自動生效 —— 這跟 Controller 的行為歷來不同,別假設一樣)
- 規則:`Title` 必填、長度 1~200。
- **然後比較兩套端點**:同樣送一個空的 Title,Minimal API 版和 Controller 版回傳的狀態碼一樣嗎?body 的 JSON 結構一樣嗎?
- 關鍵問題:驗證移到端點之後,`TodoService` 裡那個 `Result<Todo>.Invalid("Title is required.")` **還需要嗎?** 如果拿掉,哪天有背景作業直接呼叫 `ITodoService` 時,誰來擋?

**練習 4:錯誤處理 middleware**
- 先**故意製造一次未預期的例外**:把 `appsettings.json` 的連線字串改錯,打 `GET /todos`,看客戶端拿到什麼。
- 同一件事在兩種環境各試一次,比較差異:`dotnet run` 跟 `ASPNETCORE_ENVIRONMENT=Production dotnet run`。想一下為什麼預設行為要這樣設計。
- 用 `IExceptionHandler`(.NET 8+)或 `UseExceptionHandler` 做統一的錯誤處理:所有未預期的例外都回 `ProblemDetails` 格式的 500,而且**不洩漏 stack trace 或連線字串**。
- 加上 `builder.Services.AddProblemDetails()`,觀察它對原本的 404、400 回應有什麼影響。
- 想一下:middleware 的註冊順序重要嗎?錯誤處理該放在 pipeline 的哪個位置?(回去看 Day 8 那個洋蔥進出的實測)

**練習 5(整合小專案):把 API 契約收乾淨**
- 讓五個端點的**所有**錯誤回應都是 `ProblemDetails` 格式 —— 400(驗證失敗)、404(找不到)、500(未預期例外)全部統一。
- 用 `TodoClient` 或 `.http` 跑一輪完整驗收。這次不只檢查狀態碼,還要檢查**回應 body 的格式一致性**。
- 加一個 `PATCH /todos/{id}` 端點(只改 `IsDone`),對照它跟 `PUT /todos/{id}` 的語意差異,並驗證兩者都冪等。
- 寫下你對「DTO 要不要為每個操作各拆一個」的看法:你會有 `CreateTodoRequest`、`TodoResponse`,那 `Update` 要不要自己的 `UpdateTodoRequest`?`PATCH` 要不要第三個?

**驗收標準**
- 領域模型 `Todo` 不再出現在任何 API 的輸入或輸出上。
- 送不合法的輸入,兩套端點(Minimal API / Controller)回傳的狀態碼與 body 格式一致。
- 連線字串改錯之後,客戶端拿到的是乾淨的 `ProblemDetails` 500,看不到 stack trace,也看不到連線字串。
- 能解釋「格式驗證」「業務規則驗證」「資料完整性驗證」分別該在哪一層,以及為什麼。
- 能說明為什麼 `PUT` 和 `PATCH` 都冪等而 `POST` 不冪等 —— 用你 Day 6 的結論(關鍵在「設定絕對值 vs 在現值上做變化」)檢查你的 `PATCH` 設計。

**延伸**
- 查 FluentValidation,比較它跟 Data Annotations 的取捨(表達力、跨欄位規則 vs 多一個依賴)。
- 查 `[ApiController]` 的自動模型驗證是怎麼運作的 —— 為什麼 Controller 版不需要你手動檢查 `ModelState`?回去看 Day 9 那個「探索規則不是靠 `[ApiController]`」的結論,這次看的是它**真正**負責的事。
- 查 OpenAPI:`dotnet new webapi` 預設附的那個東西是什麼,DTO 跟它的關係是什麼。
- 想一下 API 版本控制:哪天 `TodoResponse` 要改欄位名,已經上線的前端怎麼辦?

### Day 12:身份驗證/授權(JWT)— 加登入與權限保護

**環境設置**
1. 在 `day8/TodoApi` 裝兩個套件:
   - `dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer`(驗證 JWT)
   - `dotnet add package Microsoft.AspNetCore.Identity`(只為了借它的 `PasswordHasher<T>`,不是要用整套 Identity)
2. 簽章密鑰**絕對不能寫進 `appsettings.json` 推上 GitHub**。用 user secrets:
   - `dotnet user-secrets init`
   - `dotnet user-secrets set "Jwt:Key" "<一串至少 32 字元的隨機字串>"`
   - 想一下:user secrets 存在哪裡?為什麼它不會進版控?

**練習 1:概念暖身(先寫筆記再動手)**
- **Authentication 跟 Authorization 差在哪?** 對應的 HTTP 狀態碼分別是什麼?(提示:401 和 403 的語意完全不同,而 401 的名字其實取錯了)
- **JWT 的三個部分**是什麼?為什麼用 `.` 分隔?去 jwt.io 貼一個你等下會產生的 token 進去看。
- **JWT 不是加密,是簽章。** 把 payload 那段單獨拿出來 base64url 解碼,確認你能讀到裡面的內容。然後回答:**什麼東西絕對不能放進 JWT?**
- 有了 JWT 為什麼就不需要伺服器端的 session?**代價是什麼?**(提示:想一下「使用者被停權」這件事多快會生效)
- **為什麼 JWT 很難「登出」?** 業界怎麼處理這個問題?
- 密碼為什麼不能明文存?**雜湊跟加密差在哪**(哪一個是單向的)?為什麼不能用 MD5 或 SHA256 來雜湊密碼?(提示:查 "password hashing" 跟 "salt"、"work factor")

**練習 2:使用者與密碼雜湊**
- 建 `User` 實體(`Id`、`Email`、`PasswordHash`、`CreatedAt`),加進 `AppDbContext`,跑 migration。
- **Email 必須唯一** —— 這題直接用 Day 11 練習 1 題 3 的結論:唯一索引 + 應用層預檢 + 捕捉唯一約束違反例外,三層各自負責什麼?
- 實作 `POST /auth/register`。**密碼雜湊不要自己寫**,用 `PasswordHasher<User>`。
- DTO 設計:`RegisterRequest` 該有哪些欄位?**絕對不能有什麼?**(回去看 Day 11 的 over-posting)
- 註冊成功該回什麼?要不要直接回 token?(兩種做法都有人用,想清楚差別)

**練習 3:登入與發 JWT**
- 實作 `POST /auth/login`:驗證密碼(`PasswordHasher.VerifyHashedPassword`)→ 產生 JWT。
- **claims 要放什麼?** 至少需要能識別使用者的那個。查 `ClaimTypes.NameIdentifier` 跟 JWT 標準的 `sub` 有什麼關係(這裡有個 .NET 特有的坑)。
- 在 `Program.cs` 設定 `AddAuthentication().AddJwtBearer(...)`,把 `Issuer`、`Audience`、`IssuerSigningKey`、`ClockSkew` 都設明確。想一下 `ValidateIssuer` / `ValidateAudience` 各自在防什麼。
- **登入失敗要回什麼?** 401 還是 400?而且訊息**不能說「密碼錯誤」或「帳號不存在」** —— 為什麼?
- 把產生的 token 貼到 jwt.io,對照你設定的 claims。

**練習 4:保護端點**
- 加 `app.UseAuthentication()` 和 `app.UseAuthorization()`。**順序很重要** —— 用 Day 11 學的那套(`Use` 之間有先後、`Map` 位置無關)推一下它們該放哪,以及為什麼 `UseAuthentication` 必須在 `UseAuthorization` 前面。
- 保護五個 Todo 端點:Minimal API 用 `.RequireAuthorization()`(可以掛在 `MapGroup` 上),Controller 用 `[Authorize]`。
- 四種失敗情況各測一次,記下**狀態碼和回應格式**:
  - 完全沒帶 token
  - token 格式錯(隨便一串字)
  - 簽章對不上(把 key 改掉再驗)
  - token 過期(把 `exp` 設成 1 分鐘,等它過期)
- **401 跟 403 什麼時候各自出現?** 自己製造一次 403 看看。
- 這些 401 有進到你 Day 11 做的 ProblemDetails 格式嗎?**為什麼?**(提示:回去看「五種錯誤只有一種是例外」那張表)

**練習 5(整合小專案):資料隔離**
- `Todo` 加 `OwnerId`(指向 `User`),跑 migration。
- **既有資料怎麼辦?** 資料庫裡已經有沒有 owner 的 Todo —— 這跟 Day 10 加 `CreatedAt` 時的 `defaultValue` 是同一類問題,但這次更嚴重(外鍵約束會拒絕無效的值)。想清楚再動手。
- 從 JWT 取出使用者 id:`HttpContext.User.FindFirst(...)`。這段邏輯要放哪一層?
- **所有**查詢都要過濾 `OwnerId` —— 五個端點一個都不能漏。
- 關鍵設計題:**這會改變 `ITodoRepository` 的介面嗎?** 三個選項(每個方法多一個 `ownerId` 參數 / 介面不變而在 Service 過濾 / 用 EF Core 的 global query filter),各自的代價是什麼?回去看 Day 10 練習 5 關於介面膨脹的結論。
- 另一個設計題:`GET /todos/{id}` 查到**別人的** Todo 該回 **404 還是 403**?兩種都說得通,選一個並寫下理由。
- 用兩個帳號交叉測試:A 建立的 Todo,B 拿自己的 token 完全看不到也改不動。

**驗收標準**
- 密碼在資料庫裡是雜湊值,而且同一個密碼註冊兩次會得到**不同**的雜湊(說明為什麼)。
- 沒帶 token 打任何 Todo 端點都回 401,兩套端點(Minimal API / Controller)行為一致。
- 能解釋 JWT 的三個部分、為什麼它不需要 session、以及「登出」為什麼是個難題。
- 能說明 401 和 403 的差別,並各自製造一次。
- A 帳號的 Todo 對 B 帳號完全不可見、不可改 —— 五個端點都驗證過。
- 簽章密鑰不在版控裡(`git log -p` 搜不到)。

**延伸**
- 查 **refresh token**:為什麼需要它?access token 為什麼要設短效期?
- 查 **role-based** 跟 **policy-based** 授權的差別,試著做一個 `[Authorize(Policy = "...")]`。
- 想一下:**為什麼 JWT 必須搭配 HTTPS?** 不用的話會發生什麼?
- 查 token 該存在前端哪裡(localStorage / sessionStorage / httpOnly cookie),各自對 XSS 和 CSRF 的暴露程度 —— 這題沒有完美答案,重點是理解取捨。

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
- 2026-10-11:Day 9 完成(練習 1~5)。**在 `day8/TodoApi` 上原地重構,沒有另開專案**,筆記在 `day8/TodoApi/notes-day9.md`。把 Day 8 那個同時扮演「業務規則」與「資料存取」的 `TodoService` 拆成 Endpoint / Service / Repository 三層,`Program.cs` 從 132 行縮到 8 行執行程式碼。最重要的觀念修正是「**依賴方向 ≠ 呼叫順序**」:呼叫是雙向的(請求往下、資料往上回),依賴是單向的,看的是「誰的程式碼裡提到誰的型別」;反過來讓 Repository 依賴 Service 會被 DI 容器直接拒絕(`A circular dependency was detected`)。驗收時掃過 Repository 的程式碼,裡面找不到任何 `ITodoService` 字樣 —— 低層完全不知道高層存在。介面設計修掉兩個洩漏:`ITodoRepository` 原本收 `CreateTodoRequest`(那是 API 層的輸入格式)改成只收領域模型 `Todo`,`Guid.CreateVersion7()` 從 Repository 搬回 Service(「Id 怎麼來」是業務規則,不是儲存細節)。練習 4 用 `readonly record struct` 實作 Result pattern,解決「`bool` 分不出該回 404 還是 400」;同時確認**不是每個方法都該包 Result** —— `GetAll` 不會失敗、`GetById` 用 `Todo?` 就表達完整,只有失敗原因超過一種時才需要。練習 5 寫 `FileTodoRepository` 時把整條鏈改成 async(async 會傳染),踩到兩件事:`lock` 區塊內不能 `await`(error CS1996,因為 `Monitor` 必須由同一條執行緒進出,而 `await` 後可能換執行緒),async 世界要用 `SemaphoreSlim(1,1)` 配 `try`/`finally`,而且它**不可重入**,私有輔助方法一律不能再取鎖;另外釐清「只是轉手就不要 async」的例外 —— 有 `try`/`catch` 或 `using` 時必須 `await`,否則例外被包在 Task 裡攔不到、或資源在 Task 跑完前就被釋放。最終驗證:切換 `InMemoryTodoRepository` ↔ `FileTodoRepository` **只改一行註冊**,上面兩層一個字都沒動,重啟後資料還在。過程中踩到的坑:搬運時 `UpdateTodo` 漏掉 lock、`ITodoRepository` 忘了註冊(DI 的接線錯誤編譯不會擋,要到執行期第一次請求才爆)。
- 2026-10-16:Day 10 完成(練習 1~5 + 延伸)。**同樣在 `day8/TodoApi` 上原地接資料庫**,筆記在 `day8/TodoApi/notes-day10.md`。Docker 跑 PostgreSQL,`AddDbContext` 一行實測註冊了 5 個服務(`AppDbContext` 是 Scoped);釐清 `where TContext : DbContext` 是泛型約束、`DbSet<Todo> Todos => Set<Todo>()` 是 expression-bodied member(所以沒有未初始化警告 —— 計算屬性沒有欄位),而編譯器對 DbContext 的 `DbSet` 自動屬性另有特例(不報 CS8618)。**Migration 的三個組成部分**:migration 檔案(`Up`/`Down`)、snapshot(「模型目前長什麼樣」)、資料庫裡的 `__EFMigrationsHistory`(「這台資料庫跑到哪」);`migrations add` 只比對模型與 snapshot、完全不碰資料庫,`database update` 只比對檔案清單與 history 表、完全不看 C# 模型 —— 這一天踩的每個坑都是這兩條軸不同步的後果。踩到的坑依序是:`CreatedAt { get; }` 沒有 setter 又不在建構子參數裡,EF Core 直接不納入模型,於是 `migrations add` 產生**空的 `Up()`**(而且 `database update` 會成功、會記進 history 表、資料庫什麼都沒變);跳過 `database update` 就打端點拿到 `42703 column does not exist`(模型推進了但 schema 沒);`DateTime.Now` 寫進 `timestamptz` 被 Npgsql 擋下(`Kind` 必須是 `Utc`),而且這個錯誤只在**執行期第一次寫入**才爆,`migrations add` / `database update` 都不會提前發現。回滾實驗驗證了 `Down()` 的真正代價:`DropColumn` 之後再 `Up()` 回來,同一筆資料的 `Id`/`Title` 活著、`CreatedAt` 變成 `-infinity`(EF Core 的 `defaultValue: DateTime.MinValue` 經 Npgsql 映射成 PostgreSQL 的特殊值),**schema 可逆、資料不可逆**,而且事後完全看不出那一欄曾經有值。把 `Todo` 從 `record` 改成 class 以配合 change tracking 時,順手把 `Id` 移出建構子,造成 `UpdateAsync(id, request)` 的 `id` 參數完全沒被使用、`Guid.CreateVersion7()` 每次產生新 Id、PUT 永遠回 404 —— 編譯器對未使用的參數沒有警告,屬於全靜默的 bug;修法是把必填的 identity 放回建構子參數,**讓編譯器當守門員**。同一個錯誤模式也出現在 Day 8 自己寫的 `IGuidService`:`{ get; } = Guid.CreateVersion7()` 配 Singleton 註冊 → 整個程式生命週期只算一次 → 永遠同一個 Guid(實測 `new Todo()` 三次得到三個不同 Guid、Singleton 的 `svc.Id` 讀三次完全相同,證明兩者差別在「new 幾次」而不是 initializer 行為)。`EfTodoRepository` 寫完後抓到 `FirstAsync` 的語意:找不到時**丟 `InvalidOperationException` 而不是回傳 null**,導致 `if (todo != null)` 恆真、`return false` 成為死碼、三個方法的 404 路徑全部變成 500,而 `dotnet build` 零警告;換成 `FirstOrDefaultAsync` 後修復,並記下 `First`/`FirstOrDefault`/`Single`/`SingleOrDefault` 四者的對稱語意。另一個行為差異是 `GetAllAsync` 沒有 `ORDER BY` —— PostgreSQL 不保證回傳順序,而 `InMemory`/`File` 版是穩定的插入順序,換實作後行為悄悄分岔;因為 Id 是 Guid v7(開頭即時間戳),加 `OrderBy(t => t.Id)` 就取回插入順序。`UpdateAsync` 刻意不用 `db.Update(todo)` 而是「先查出被追蹤的實體再改欄位」,讓 change tracker 只把變更的欄位寫進 `UPDATE` —— 否則整列覆蓋會把 `CreatedAt` 重設成當下時間(又一個靜默失敗)。`lock`/`SemaphoreSlim` 全部拿掉:`DbContext` 是 Scoped、每個請求一個獨立實例,沒有共享狀態可競爭,併發控制下移到資料庫的交易與 MVCC;但要記住 `DbContext` **本身不是執行緒安全**,它不需要鎖是因為 Scoped 保證它不被多執行緒同時碰。練習 5 開 SQL log 做對照實驗(10000 筆):走 Repository 送出 `SELECT` 四個欄位、全表、無 `WHERE` 無 `LIMIT`,92 ms;直接用 `DbSet` 送出 `SELECT "Title"` + `WHERE NOT "IsDone"` + `LIMIT @p`,24 ms —— `ToListAsync()` 就是 `IQueryable`(組 SQL)與 `IEnumerable`(記憶體)的分界線。`AsNoTracking()` 加在 `GetAllAsync`(不加在 `GetByIdAsync`,因為方法名無法表達「結果不可改」,而不被追蹤的實體改了之後 `SaveChanges` 不會有任何反應);實測 `ReferenceEquals` 確認追蹤模式下同一個 Id 查兩次回傳**同一個物件實例**(identity map),`AsNoTracking` 則是兩個獨立物件 —— 也因此追蹤模式下第二次查詢拿不到資料庫的最新值。**Repository 模式的結論(Day 9 延伸題欠的那題)**:寫入走 Repository(封裝業務規則、Result pattern、交易邊界),複雜查詢直接用 `DbContext` —— 也就是分讀寫;而最重要的體會是「**可替換性與查詢能力是同一個取捨的兩面**」:這一天把 Repository 從 `InMemory` → `File` → `EfCore` 換過三次、只改 `Program.cs` 一行註冊、Day 8 寫的 `TodoClient` 測試一字未改全綠,之所以能這麼輕鬆正是因為介面只有五個方法;而介面只有 `GetAllAsync()` 也正是上面那句爛 SQL 的原因 —— 要讓查詢下推到 SQL 介面就得膨脹,膨脹到四十個方法就再也寫不出第二個實作了。延伸題:釐清 **EF Core 預設不會發生 N+1**(預設關閉 lazy loading,要主動裝 `Proxies` 套件 + `UseLazyLoadingProxies()` + `virtual` 導覽屬性才會),真正的來源是自己手寫的迴圈(Day 11 做 DTO 轉換時最容易踩),而 `Include` 自己帶來笛卡爾爆炸的風險、`AsSplitQuery()` 是第三種取捨;`dotnet ef migrations script --idempotent` 實際跑過兩次,看到每個 migration 被包在獨立交易裡、schema 變更與 history 記錄同進同退(PostgreSQL 支援交易式 DDL 才辦得到),以及正式環境不跑 `database update` 的關鍵理由 —— 它需要載入專案 assembly,而生產環境沒有原始碼和 SDK。最後釐清 Fluent API:`HasKey(t => t.Id)` 和 `Property(t => t.Title).IsRequired()` 都是 no-op(convention 已經做到,證據是 `InitialCreate` 在 `OnModelCreating` 寫出來之前就已經產生了正確的主鍵與 `NOT NULL`),`HasDefaultValue()` 無參數版本等於 CLR 預設值也就是 `-infinity`,而 `HasDefaultValueSql("now()")` 在 C# 端仍有 `= DateTime.UtcNow` initializer 的情況下**永遠不會生效**(SQL 的 DEFAULT 只在 INSERT 省略該欄時才套用)—— 真正需要資料庫預設值的場景是「寫入來源不只應用程式」「需要單一時間來源」「migration 回填既有資料」。
- 2026-10-20:Day 11 完成(練習 1~5 + 延伸)。**繼續在 `day8/TodoApi` 上做**,筆記在 `day8/TodoApi/notes-day11.md`(527 行,是目前最長的一份)。主線是把對外的契約邊界關起來 —— Day 9 關的是內部的層次邊界(API 輸入格式不該滲進 Repository),Day 11 關的是反方向(領域模型不該滲出去給前端),是同一個原則的兩個方向。拆出 `TodoResponse` 之後最重要的判斷是「**轉換該放哪一層**」:一開始用消去法選了 Service(端點管 HTTP、Repository 管存取、所以剩 Service),但那個推理的漏洞是**轉換 DTO 本身就是 HTTP 層的工作** —— `TodoResponse` 存在的唯一理由是「HTTP 客戶端要看到什麼」。若讓 `ITodoService` 回傳 `Result<TodoResponse>`,代價有三:Service 再也不能服務 gRPC/CLI/背景作業、`CreateAsync` 在 `await` 之後想做事(寄通知、audit log 需要 `CreatedAt`)時已經資訊損失、Day 13 寫測試時斷言的會是 API 格式而非領域行為。最後用 Day 1 學的擴充方法 `ToResponse()` 同時滿足四件事:Service 不碰 API 格式、轉換只有一份、兩套端點共用、`TodoResponse` 與 mapping 都住在 API 層。DTO 的另一個價值是**白名單**:加 `InternalNote` 不會洩漏,因為 `TodoResponse` 裡沒有那個欄位 —— 對照 Day 10 加 `CreatedAt` 時它**自動**出現在所有回應裡(當時沒有白名單),而靠 `private` 或 `[JsonIgnore]` 隱藏是黑名單思維(預設公開、靠記得標注)。驗證這一題挖出最多反直覺的機制:**Minimal API 沒有「模型驗證」這個 pipeline 階段**(MVC 是 綁定→驗證→`[ApiController]` 檢查 ModelState→action,Minimal API 只有 綁定→handler),所以 Data Annotations 被完全忽略,實測同一個空 Title 在 `/api/todos` 被 `ValidationProblemDetails` 擋下、在 `/todos` 卻一路跑到 Service 才被 `IsNullOrWhiteSpace` 攔住 —— 而且 `request.Title` 在那裡**真的是 null**,儘管宣告是 non-nullable `string`(`System.Text.Json` 完全不看 nullable 註記,這也正是「為什麼需要驗證層」的根本答案:型別系統的承諾在反序列化邊界守不住)。解法是 .NET 10 的 `AddValidation()`,它用 **endpoint filter** 實作所以不需要配 `app.UseXxx()`。另一個發現是**沒寫 `[Required]` 也會擋空值** —— nullable reference types 啟用後,ASP.NET Core 把 non-nullable `string` 隱式當成必填(所以 Controller 從 Day 9 寫完那天就有驗證了,只是一直沒送空值去測);而 `RequiredAttribute` 內部會先 `Trim()` 再檢查,行為跟 `IsNullOrWhiteSpace` 完全一樣,但它不管長度。踩到的最大認知坑是 **`[Required]` 加在 `bool` 上完全無效**(`RequiredAttribute` 檢查的是「是不是 null」,而 value type 永遠不是 null),要讓布林值真正必填必須先讓它**能夠**是 null → `[Required] bool?`。這條線往下挖出一個更根本的事實:**實值型別無法分辨「沒傳」和「傳了預設值」** —— 精確的機制不是綁定器主動填 default,而是**它根本沒碰那個屬性**,物件誕生時 CLR 保證記憶體歸零,所以兩種情況的記憶體狀態完全相同、事後看不出差別;`bool?` 能分辨是因為 `Nullable<bool>` 是個 struct、多帶一個 `hasValue` 欄位(`bool` 1 byte、`bool?` 2 bytes)。這直接決定了 PUT 與 PATCH 的 DTO 設計:**同一個 `bool?`,兩個相反的理由** —— PUT 配 `[Required]` 是為了**偵測漏傳並拒絕**(不然客戶端只想改標題卻漏傳 `isDone`,完成狀態會被靜默重設成 false 還回 204 成功),PATCH 不加 required 是為了**用 null 表達「不修改」**。錯誤處理那題先故意把連線字串改錯,親眼看到兩個極端:Development 的開發者例外頁一行就交出「資料庫是 PostgreSQL」「使用者名稱是 postgres」「而且是預設超級使用者」,往下還有完整堆疊和**請求 headers**(使用者的認證 token 會原封不動顯示);Production 則是完全空白的 500,客戶端連個可回報的編號都沒有。正確答案是**同一個例外、兩個去處**:客戶端拿最少資訊 + `traceId`,server log 拿完整堆疊 + 同一個 `traceId`。過程中釐清了三組容易混淆的東西:(1) `ExceptionHandlerMiddleware`(真正的 middleware,`app.UseExceptionHandler()` 加入)vs `IExceptionHandler`(只是個被它呼叫的服務)—— 實測把 `UseExceptionHandler()` 註解掉之後 handler **完全不會被呼叫,連 log 都沒有**;(2) `IExceptionHandler` 的 `bool` 是個契約,`false` 是觀察者(只做副作用,回應交給預設行為,多個 handler 時繼續往下傳 —— 那是責任鏈不是缺點)、`true` 是接管者(**必須自己寫 body**,實測不寫就又變回空白 500);(3) `UseExceptionHandler` 只攔**丟出來的例外**,而 `UseStatusCodePages` 攔的是**狀態碼 4xx/5xx 但 body 空的**回應 —— 整理成「五種錯誤只有一種是例外」的表之後才明白,參數綁定失敗(`/todos/asdf`)、驗證失敗、路由不存在、資料找不到全都是「請求被正常處理、結論是個錯誤狀態碼」,只有資料庫連不上才真的丟例外。另一個推翻直覺的實測是 **`app.Map...` 不是 middleware** —— 它只把路由登記到一個集合,真正執行端點的 `EndpointMiddleware` 由 `WebApplication` 自動加在 pipeline **最後**,所以 `Use` 和 `Map` 的相對順序完全不影響行為,只有 `Use` 彼此之間的順序才重要(用一個會丟例外的 `/boom` middleware 做 A/B 驗證:`UseExceptionHandler` 在前→乾淨的 500,在後→例外穿過它沒人攔)。也確認了 `AddProblemDetails()` 本身不攔任何東西(只註冊 `IProblemDetailsService` 給別人用),以及 **`UseStatusCodePages` 能補格式但補不出內容**(`Results.NotFound()` 補出來的 ProblemDetails 沒有 `detail`,因為 middleware 不知道你想說什麼)—— 最後的分配是 `Invalid` 保留 `detail: result.Error`、`NotFound` 交給 `CustomizeProblemDetails` 的通用訊息,而 `??=` 讓它是**後備而不是取代**。另外學到 `launchSettings.json` 會覆蓋 shell 的環境變數(要 `--no-launch-profile` 才能真的切到 Production),而它**不會進 `dotnet publish` 的產出** —— 所以生產環境是 Production 不是因為有人設定,而是因為 `ASPNETCORE_ENVIRONMENT` 沒設定時預設就是它,Development 反而是本機刻意打開的,又一個失敗安全設計。結構化日誌的佔位符 `{Method}` **靠位置綁定不是靠名字**(調換 template 裡的名字會讓值錯位而編譯器不警告),不能用字串插值是因為 logger 收到的東西不同:插值給的是已壓平的字串,結構化給的是 template + values 分開保存,後者才能讓 `Method`/`Path` 變成可查詢的欄位並按 template 分群(analyzer 規則 `CA2254` 專門抓這件事)。練習 5 加了 `PATCH /todos/{id}/done`,過程中踩到一個跟 Day 10 完全同模式的 bug:`new Todo(id, existing.Title, existing.IsDone)` 把 `request.IsDone` 漏掉了,結果 PATCH 回 204 成功但資料沒變 —— 跟 Day 10 那個「`id` 參數沒用到導致 PUT 永遠 404」都是**建構子參數的來源拿錯**,編譯器無從判斷意圖。也因此體會到「建新物件」雖然比「直接改取出的物件」安全(後者在 EfCore/InMemory/File 三個實作下語意完全不同:靠 change tracking / 靠參考 / 完全不成立),但它把風險從「依賴隱含行為」換成了「每個參數的來源都要寫對」。**DTO 要不要為每個操作各拆一個的結論**:要,而論據不是「語意清楚」而是**四個 DTO 的驗證規則互不相容** —— `CreateTodoRequest` 根本沒有 `IsDone` 欄位(新建必然未完成)、`UpdateTodoRequest` 的 `IsDone` 是 `[Required] bool?`、`SetDoneRequest` 只有一個必填欄位、`TodoResponse` 完全不驗證;若共用一個 DTO,所有欄位都得是 nullable 且不能加 `[Required]`,驗證就會從宣告式**退回命令式的 if 判斷**,而那正是這一天在拆掉的東西(而且退回去的版本更糟:規則散落在 Service 裡、不會出現在 OpenAPI 文件上、客戶端只能用試的)。延伸題釐清 **FluentValidation 跟 EF Core 的 Fluent API 是兩個完全不同的東西**(只有「Fluent」兩字相同,那只是 method chaining 的風格),前者是第三方驗證套件、validator 是普通類別所以**可以注入服務**(而 attribute 是編譯期 metadata 無法注入)—— 但「能注入 Repository 做唯一性檢查」是能力不是建議,那會把資料完整性驗證塞進格式驗證那一層而且帶著 TOCTOU 問題;`[ApiController]` 真正負責的事之一是自動模型驗證(機制是 `ModelStateInvalidFilter` 這個 action filter,在 action **之前**執行,所以驗證失敗時 action 根本不跑、`ResultStatus.Invalid` 分支變成死碼,可用 `SuppressModelStateInvalidFilter` 關掉);OpenAPI 的 schema **直接從 DTO 型別產生而且 Data Annotations 會被寫進去**(`[StringLength(20)]` 會變成 schema 裡的 `maxLength: 20`),所以練習 1 寫的「驗證規則應該出現在 OpenAPI 文件裡讓客戶端送出前就知道」是可以實際驗證的;API 版本控制則是回到 DTO 解耦的價值 —— `TodoResponse` 改欄位名而資料庫的 `Title` 完全不動,那行三行的 `ToResponse()` 就是整個解耦的支點,而判斷破壞性變更的規律是「**放寬安全、收緊破壞;給得多安全、要得多破壞**」。
