using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

HttpClient client = new();

/*
**練習 5(整合小專案):用 Day 6 的 HttpClient 測自己的 API**
- 另開一個 console 專案(或沿用 `day6/HttpLab`),用 `HttpClient` 對自己的 Todo API 跑一輪完整流程:
  `POST` 建立 → `GET` 確認 → `PUT` 更新 → `GET` 確認 → `DELETE` → `GET` 應得到 404。
- 每一步印出狀態碼,驗證跟練習 3 的表格一致。
- 這題同時複習 Day 6 的 `GetFromJsonAsync` / `PostAsJsonAsync` 與例外處理。
*/
try
{
    // GetAll();
    // HttpResponseMessage response = await client.GetAsync("http://localhost:5020/todos");
    // var json = await response.Content.ReadFromJsonAsync<List<Todo>>();
    // json.Dump();

    // `POST` 建立
    StringContent content = new(JsonSerializer.Serialize(new CreateTodoRequest("TodoClient 建立", false)), Encoding.UTF8, "application/json");
    HttpResponseMessage createResponse = await client.PostAsync("http://localhost:5020/todos", content);
    createResponse.EnsureSuccessStatusCode();
    Console.WriteLine($"POST 成功回傳的 StatusCode: {(int)createResponse.StatusCode}");
    Todo? createJson = await createResponse.Content.ReadFromJsonAsync<Todo>();

    if (createJson == null) return;

    // `GET` 確認
    HttpResponseMessage getByIdResponse = await client.GetAsync($"http://localhost:5020/todos/{createJson.Id}");
    getByIdResponse.EnsureSuccessStatusCode();
    Console.WriteLine($"GET 成功回傳的 StatusCode: {(int)getByIdResponse.StatusCode}");
    Todo? getByIdTodo = await getByIdResponse.Content.ReadFromJsonAsync<Todo>();

    // 練習 GetFromJsonAsync<> => 直接拿資料
    // 如果是使用 GetFromJsonAsync 就沒辦法取得 StatusCode 只能取得拋到 Exception 的例外中才能拿到
    // GetAsync 才能拿到 Http 細節
    Todo? result = await client.GetFromJsonAsync<Todo>($"http://localhost:5020/todos/{createJson.Id}");

    if (getByIdTodo == null) return;

    // `PUT` 更新
    // StringContent updatedContent = new(JsonSerializer.Serialize(new CreateTodoRequest("PUT 更新", true)), Encoding.UTF8, "application/json");
    HttpResponseMessage putResponse = await client.PutAsJsonAsync($"http://localhost:5020/todos/{createJson.Id}", new CreateTodoRequest("PUT 更新", true));
    putResponse.EnsureSuccessStatusCode();
    Console.WriteLine($"PUT 成功回傳的 StatusCode: {(int)putResponse.StatusCode}");

    //  `GET` 確認
    HttpResponseMessage getByUpdatedIdResponse = await client.GetAsync($"http://localhost:5020/todos/{createJson.Id}");
    getByUpdatedIdResponse.EnsureSuccessStatusCode();
    Todo? getByUpdatedIdTodo = await getByUpdatedIdResponse.Content.ReadFromJsonAsync<Todo>();
    Console.WriteLine($"GET 確認更新後: {getByUpdatedIdTodo?.Title} / IsDone={getByUpdatedIdTodo?.IsDone}");

    if (getByUpdatedIdTodo == null) return;

    //  → `DELETE` → `GET` 應得到 404。
    HttpResponseMessage deleteByIdResponse = await client.DeleteAsync($"http://localhost:5020/todos/{createJson.Id}");
    deleteByIdResponse.EnsureSuccessStatusCode();
    Console.WriteLine($"DELETE 成功回傳的 StatusCode: {(int)deleteByIdResponse.StatusCode}");

    //  `GET` 確認
    HttpResponseMessage getByDeletedIdResponse = await client.GetAsync($"http://localhost:5020/todos/{createJson.Id}");
    Console.WriteLine($"GET 已經刪除的 StatusCode: {(int)getByDeletedIdResponse.StatusCode}");

}
catch (TaskCanceledException e) when (e.InnerException is TimeoutException)
{
    Console.WriteLine("逾時 -> API 回應太慢");
}
// catch (OperationCanceledException) { }
catch (HttpRequestException e)
{
    Console.WriteLine($"請求失敗: {e.StatusCode} {e.Message}");
}

public record Todo(Guid Id, string Title, bool IsDone);
public record CreateTodoRequest(string Title, bool IsDone);

static class DumpExtensions
{
    static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    internal static T Dump<T>(this T obj, string? label = null)
    {
        if (label != null) Console.WriteLine($"Label: {label}");
        Console.WriteLine(JsonSerializer.Serialize(obj, Options));
        return obj;
    }
}