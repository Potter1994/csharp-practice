# csharp-practice

從 JavaScript / TypeScript 轉 C# 後端的學習紀錄 —— 28 個單元,從語法與電腦原理,一路做到 AI 應用與 AWS 部署。

每個單元的產出是「一個跑得起來的小專案 + 一份把觀念講清楚的筆記」。筆記裡刻意保留了推論錯誤的段落(用 `<!-- 錯誤回答❌ -->` 標記),訂正的過程本身就是紀錄的一部分。

> 「Day N」是**單元編號,不是日期**。不排進度表,依序完成,做完一個打一個勾。

## 學習路徑

最終目標是跑完一輪完整的全端專案:**C# 後端 API → 加上 AI 功能 → 用 Terraform 部署到 AWS → React 前端串接**。先求廣度,跑完一輪後再回頭深化。

| 階段 | 主題 | 進度 |
|---|---|---|
| 一 | C# 基礎 + 電腦原理 | 6 / 7 |
| 二 | 後端架構設計(ASP.NET Core) | 0 / 7 |
| 三 | AI / LLM 應用開發 | 0 / 7 |
| 四 | AWS + Terraform + DevOps + 前端串接 | 0 / 7 |

完整課表見 [study-plan.md](study-plan.md)。

## 已完成的單元

| # | 主題 | 專案 | 重點 |
|---|---|---|---|
| 1 | C# 基本語法、型別系統 | [day1/Basics](day1/Basics) | 型別、方法多載、`out` 參數、溫度轉換 CLI |
| 2 | OOP:class / interface / 繼承 | [day2/Library](day2/Library) | 封裝、抽象類別、多型、介面選擇性能力(圖書館借閱系統) |
| 3 | 記憶體、CPU、Stack vs Heap | [day3/MemoryLab](day3/MemoryLab) | value / reference type、`ref`、boxing 成本、記憶體用量實測 |
| 4 | 作業系統、Process / Thread | [day4/AsyncLab](day4/AsyncLab) | race condition 與 `lock`、`Task` / `async`、`WhenAll` 並行查詢 |
| 5 | LINQ、集合、Generics | [day5/LinqLab](day5/LinqLab) | 集合效能實測、延遲執行、泛型約束 |
| 6 | 網路基礎(TCP/IP、HTTP) | [day6/HttpLab](day6/HttpLab) | `HttpClient` 生命週期、逾時與取消、並行抓取 + LINQ 處理 |

各單元的觀念筆記放在專案資料夾內的 `notes.md`。

### 幾個做下來覺得值得記的點

- **Stack 快不是因為「記憶體比較快」**,是因為配置只是移動一個指標,而且熱資料一直在 CPU cache 裡 — [day3](day3/MemoryLab/notes.md)
- **`await` 不等於開一條新執行緒**。I/O 等待期間執行緒會還回執行緒池,所以 `Task.WhenAll` 跑 10 個 HTTP 請求不需要 10 條執行緒 — [day4](day4/AsyncLab/notes.md)
- **`HttpClient` 的連線池在底層的 `SocketsHttpHandler` 上,不在 `HttpClient` 本身**。所以每次 `new` 都是一個新的空池子,連線無法共用 — [day6](day6/HttpLab/notes.md)
- **逾時丟的是 `TaskCanceledException` 不是 `TimeoutException`**,要靠 `InnerException` 才分得出「逾時」和「使用者主動取消」 — [day6](day6/HttpLab/notes.md)
- **冪等性決定請求失敗時能不能重試**。`GET`/`PUT`/`DELETE` 可以,`POST` 不行 — [day6](day6/HttpLab/notes.md)

## 怎麼跑

每個資料夾都是獨立的 console 專案,`cd` 進去直接跑:

```bash
cd day6/HttpLab
dotnet run
```

存檔自動重跑(類似 nodemon):

```bash
dotnet watch run
```

## 環境

- .NET 10 (`net10.0`)
- macOS + VS Code + C# Dev Kit

```bash
dotnet --version
```
