/*
**練習 2:拆出 Response DTO**
- 現在五個端點直接回傳領域模型 `Todo`。新增一個 `TodoResponse`,只包含前端真的需要的欄位。
- 問題來了:**轉換放在哪一層?** Endpoint / Service / Repository?先自己決定再往下看。
  (提示:`ITodoService` 現在回傳 `Result<Todo>`。如果改成 `Result<TodoResponse>`,Service 就知道了 API 的輸出格式 —— 那是好事還是壞事?回去看 Day 9 把 `CreateTodoRequest` 從 `ITodoRepository` 移走的理由。)
    轉換需放在 Endpoint: 處理 HTTP 層(請求、狀態碼、body 格式)

    如果 `ITodoService` 回傳改成 `Result<TodoResponse>`,Service 就知道了 API 的輸出格式
    1. ITodoService 再也不能服務別的傳輸方式 —— gRPC、CLI、背景作業拿到的都是 HTTP API 的回應格式
    2. Service 內部會資訊損失 —— 如果 CreateAsync 在 await repo.AddAsync() 之後還要做事(寄通知、寫 audit log,需要 CreatedAt),它手上已經只有 TodoResponse 了
    3. Day 13 寫單元測試時,你斷言的會是 API 的回應格式,而不是領域行為

- 做完回答:如果 `Todo` 加一個 `InternalNote` 欄位,要改幾個檔案?會不會意外洩漏給前端?
    不會意外泄露給前端是因為現在回傳 TodoResponse 根本沒有這個欄位
*/
public record TodoResponse(Guid Id, string Title, bool IsDone, DateTime CreatedAt);

public static class TodoMappingExtensions
{
    public static TodoResponse ToResponse(this Todo todo) => new(todo.Id, todo.Title, todo.IsDone, todo.CreatedAt);
}