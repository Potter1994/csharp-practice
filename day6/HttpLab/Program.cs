using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;


// static async Task DoWorkAsync(CancellationToken ct)
// {
//     for (int i = 0; i < 5; i++)
//     {
//         // 檢查是否收到取消請求, 若有則拋出例外
//         ct.ThrowIfCancellationRequested();

//         Console.WriteLine($"執行步驟: {i + 1}");
//         await Task.Delay(1000, ct); // 傳遞給支援取消的 API
//     }
// }

HttpClient client = new();

/* **練習 2:用 HttpClient 打公開 API**
用 `https://jsonplaceholder.typicode.com` 這個免費測試 API(不需金鑰):
- `GET /posts/1` 取得單筆資料,把 JSON 反序列化成一個 `record`。
- `GET /posts` 取得全部,反序列化成 `List<T>`。
- `POST /posts` 送出一筆新資料,觀察回應。
- 印出回應的 **狀態碼**、**部分 header**(例如 `Content-Type`)、**body**。
- 用 `System.Text.Json` 的 `JsonSerializer`,並設定 `PropertyNameCaseInsensitive = true` 觀察差異。

try
{

    HttpResponseMessage? response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts/1");
    response.EnsureSuccessStatusCode().WriteRequestAndResponseToConsole();
    Post? responseBody = await response.Content.ReadFromJsonAsync<Post>();

    // 這邊是為了讓你看到字串轉 json 的過程, 推薦直接像上面一樣使用 ReadFromJsonAsync 就好
    string itemString = await response.Content.ReadAsStringAsync();
    var itemJson = JsonSerializer.Deserialize<Post>(itemString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    var defaultItemJson = JsonSerializer.Deserialize<Post>(itemString);

    Console.WriteLine($"itemJson = {itemJson}");
    Console.WriteLine($"預設版 = {defaultItemJson}");
    Console.WriteLine($"responseBody = {responseBody}");
    // 下面這個是轉成 JsonNode, 必須使用 ["body"] 類似 Dictionary / JavaScript Object 的存取方式
    // C# 這裡使用 indexer, 使用 JsonNode 相當於「我不知道 / 不想定義這個 JSON 的 C# 型別，我直接操作 JSON 結構。」
    // string responseBody = await response.Content.ReadAsStringAsync();
    // JsonNode? jsonNode = JsonNode.Parse(responseBody);
    // Console.WriteLine(jsonNode?["body"]);

    HttpResponseMessage? responseList = await client.GetAsync("https://jsonplaceholder.typicode.com/posts");
    responseList.EnsureSuccessStatusCode();
    List<Post>? responseListBody = await responseList.Content.ReadFromJsonAsync<List<Post>>();

    if (responseListBody != null)
    {
        foreach (Post post in responseListBody)
        {
            // Console.WriteLine($"{post.Id}: {post.Title}");
        }
    }

    // using 是自動呼叫 Dispose(), StringContent 最終繼承來自 public abstract class HttpContent : IDisposable 有 IDisposable 可以呼叫
    using StringContent jsonData = new(JsonSerializer.Serialize(new
    {
        userId = 1,
        id = 1,
        title = "new title",
        body = "new body by Potter",
    }), Encoding.UTF8, "application/json");

    var responsePost = await client.PostAsync("https://jsonplaceholder.typicode.com/posts", jsonData);
    responsePost.EnsureSuccessStatusCode().WriteRequestAndResponseToConsole();
    Console.WriteLine($"成功: {await responsePost.Content.ReadAsStringAsync()}");
}
catch (HttpRequestException e)
{
    Console.WriteLine($"\nException Caught!");
    Console.WriteLine($"Message: {e.Message}");
}
*/

/* 實驗:用同一個 `HttpClient` 實例連續打 5 次請求,跟每次都 new 一個比較耗時。
Stopwatch sw1 = Stopwatch.StartNew();
for (int i = 0; i < 5; i++)
{
    try
    {
        var response1 = await client.GetAsync($"https://jsonplaceholder.typicode.com/todos/{i + 1}");
        response1.EnsureSuccessStatusCode();
        var json1 = await response1.Content.ReadAsStringAsync();
        Console.WriteLine(json1);

    }
    catch (HttpRequestException e)
    {
        Console.WriteLine("Request exception caught!");
        Console.WriteLine($"Message: {e.Message}");
    }
}
sw1.Stop();
Console.WriteLine(sw1.ElapsedMilliseconds);

Stopwatch sw2 = Stopwatch.StartNew();
for (int i = 0; i < 5; i++)
{
    try
    {
        HttpClient client1 = new();
        var response1 = await client1.GetAsync($"https://jsonplaceholder.typicode.com/todos/{i + 1}");
        response1.EnsureSuccessStatusCode();
        var json1 = await response1.Content.ReadAsStringAsync();
        Console.WriteLine(json1);

    }
    catch (HttpRequestException e)
    {
        Console.WriteLine("Request exception caught!");
        Console.WriteLine($"Message: {e.Message}");
    }
}
sw2.Stop();
Console.WriteLine(sw2.ElapsedMilliseconds - sw1.ElapsedMilliseconds); // 2164
*/

/*
**練習 4:錯誤處理與逾時**
- 故意打一個不存在的路徑(例如 `/posts/99999`),觀察回傳什麼狀態碼、`EnsureSuccessStatusCode()` 會發生什麼。
    Message: Response status code does not indicate success: 404 (Not Found).

- 設定 `HttpClient.Timeout`,打一個會慢的端點,觀察逾時的例外類型。
    OperationCanceledException: The request was canceled due to the configured HttpClient.Timeout of 0.2 seconds elapsing.

- 用 `CancellationTokenSource` 實作「3 秒後取消請求」。
    TaskCanceledException: A task was canceled.

- 寫下:`HttpRequestException`、`TaskCanceledException`、`TimeoutException` 各在什麼情況出現?

    `HttpRequestException`: 
    連不上(DNS/TCP/TLS 失敗),或呼叫 EnsureSuccessStatusCode() 且狀態碼非 2xx。有 StatusCode 屬性可判斷 4xx/5xx

    `TaskCanceledException`: 
    逾時「和」主動取消都丟這個!兩者同型別靠 InnerException 分辨:是 TimeoutException → 逾時

    `TimeoutException`: 
    HttpClient 不會直接丟它,只會包在 InnerException 裡所以 catch (TimeoutException) 永遠抓不到
*/

/*
try
{
    // using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(200)); // 建構式直接給時間, 會自動取消
    client.Timeout = TimeSpan.FromMilliseconds(200); // 實作逾時
    HttpResponseMessage response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts/1");
    response.EnsureSuccessStatusCode();
    string json = await response.Content.ReadAsStringAsync();
    Console.WriteLine(json);
}
catch (TaskCanceledException e) when (e.InnerException is TimeoutException)
{
    Console.WriteLine("逾時 -> 可以重試");
}
catch (OperationCanceledException)
{
    Console.WriteLine("使用者取消 -> 安靜結束, 不算錯誤");
}
catch (HttpRequestException e)
{
    Console.WriteLine($"請求失敗, 狀態碼 {e.StatusCode} -> 4xx 改請求, 5xx 可重試");
}
*/

/*
catch 由上而下比對,第一個符合的就進去。所以具體的必須寫在前面:

catch (TaskCanceledException e) { }      // 具體
catch (OperationCanceledException e) { } // 一般
catch (HttpRequestException e) { }
catch (Exception e) { }                  // 最後的保底
*/

// using CancellationTokenSource cts = new(); // 實作取消請求

// // 啟動非同步任務並傳入 Token
// Task backgroundTask = DoWorkAsync(cts.Token);

// // 模擬 2 秒後取消任務
// await Task.Delay(2000);
// cts.Cancel();

// try
// {
//     await backgroundTask;
// }
// catch (TaskCanceledException e)
// {
//     Console.WriteLine($"TaskCanceledException: {e.Message}");
// }




/*
**練習 5(整合小專案):並行抓取 + LINQ 處理**
把 Day 4 和 Day 5 學的東西串起來:
- 用 `Task.WhenAll` **同時**抓取多筆資料(例如 `/posts/1` ~ `/posts/10`)。
    使用 Task.WhenAll 時間: 363 毫秒

- 跟「依序抓 10 次」比較總耗時,記下數字。
    依序抓 10 次耗時: 1216 毫秒

    但如果同時去做兩件事情時間卻是, 這是跟 cache 有關嗎? 不是 HTTP 內容 cache,是 TCP 連線重用
    同步順序取得時間: 1061
    使用 Task.WhenAll 時間: 88

    A 冷連線 WhenAll (posts 1-10)              : 641 ms
    B1 暖身單筆請求                             : 325 ms
    B2 熱連線 WhenAll (posts 11-20,全新網址)   : 341 ms
    B3 再抓一次   (posts 21-30,全新網址)       : 91 ms   ← 跟你看到的 88ms 吻合

    B3 抓的是全新網址,卻只花 91ms。所以快的原因是:
    TCP 三次交握 + TLS 交握已經做完了 —— 這是最貴的部分,HTTPS 大約佔 200~300ms。HttpClient 內部的連線池會把連線留著重用
    DNS 解析結果被快取 —— 不用再查一次 jsonplaceholder.typicode.com

    而 B2 還慢(341ms)的原因也很有意思:暖身只暖了一條連線,但 WhenAll 要同時發 10 個請求,連線池得再開 9 條新的 TCP 連線。等到 B3 時池子裡已經有 10 條熱連線,才真正快起來。
    這剛好反向證明了你練習 3 的結論為什麼成立 —— 重複 new HttpClient 慢,就是因為每個新實例都有自己的連線池,交握成本要重付一次。你那邊量到 2164ms 的差距,主要就是這個。

- 用 LINQ 處理回傳的資料:依 `userId` 分組、找出標題最長的一篇、統計每個使用者的文章數。
- 把結果用 `Dump`(JSON 序列化)印出來。
*/

// try
// {
//     HttpResponseMessage? response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts/1");

//     // Enumerable.Range 可以想成是 js 的 Array.from
//     // Array.from({ length: 5 }, (_, i) => i + 1)
//     // 不過 Enumerable.Range(起始, 數量) 參數代表的是這樣
//     var tasks = Enumerable.Range(1, 10).Select(i => client.GetAsync($"https://jsonplaceholder.typicode.com/posts/{i}"));

//     Stopwatch sw1 = Stopwatch.StartNew();
//     for (int i = 0; i < 10; i++)
//     {
//         var currentResponse = await client.GetAsync($"https://jsonplaceholder.typicode.com/posts/{i + 1}");
//         currentResponse.EnsureSuccessStatusCode();
//         string currentString = await currentResponse.Content.ReadAsStringAsync();
//         // Console.WriteLine(currentString);
//     }
//     sw1.Stop();
//     Console.WriteLine($"同步順序取得時間: {sw1.ElapsedMilliseconds}");

//     Stopwatch sw2 = Stopwatch.StartNew();
//     var responseList = await Task.WhenAll(tasks);
//     sw2.Stop();
//     Console.WriteLine($"使用 Task.WhenAll 時間: {sw2.ElapsedMilliseconds}");

//     foreach (var item in responseList)
//     {
//         item.EnsureSuccessStatusCode();
//         string json = await item.Content.ReadAsStringAsync();
//     }
// }
// catch (TaskCanceledException e)
// {
//     Console.WriteLine($"TaskCanceledException: {e.Message}");
// }
// catch (OperationCanceledException e)
// {
//     Console.WriteLine($"OperationCanceledException: {e.Message}");
// }
// catch (HttpRequestException e)
// {
//     Console.WriteLine($"HttpRequestException: {e.Message}");
// }

// - 用 LINQ 處理回傳的資料:依 `userId` 分組、找出標題最長的一篇、統計每個使用者的文章數。
try
{
    // [1, 2, 3, 4, ..., 10] 起始 1, 數量 10 個
    var tasks = Enumerable.Range(1, 30).Select(i => client.GetFromJsonAsync<Post>($"https://jsonplaceholder.typicode.com/posts/{i}"));
    Post?[] posts = await Task.WhenAll(tasks);

    // OfType<T> 如果不是 T 型別就跳過, 也不會爆炸, (Cast 就是會去轉成 T 型別, 如果失敗就會爆炸)
    var groups = posts.OfType<Post>().GroupBy(p => p.UserId);
    // foreach (var group in groups) 
    // {
    //     int userId = group.Key;
    //     int totalPosts = group.Count();
    //     Post? longestTitlePost = group.MaxBy(p => p.Title?.Length ?? 0);
    //     Console.WriteLine($"userId: {userId}, 總文章數: {totalPosts}, 最長的標題: {longestTitlePost?.Title}");
    // }

    // 改成 LINQ 投影成物件, 回傳「統計結果」使用
    groups.Select(g => new
    {
        UserId = g.Key,
        Count = g.Count(),
        LongestTitle = g.MaxBy(p => p.Title?.Length ?? 0)?.Title
    }).Dump("每個使用者的統計");


}
catch (TaskCanceledException e)
{
    Console.WriteLine($"TaskCanceledException: {e.Message}");
}
catch (OperationCanceledException e)
{
    Console.WriteLine($"OperationCanceledException: {e.Message}");
}
catch (HttpRequestException e)
{
    Console.WriteLine($"HttpRequestException: {e.Message}");
}

public record Post(int UserId, int Id, string? Title, string? Body);

static class HttpResponseHeadersExtenstion
{
    internal static void WriteRequestAndResponseToConsole(this HttpResponseMessage response)
    {
        if (response is null)
        {
            return;
        }

        // - 印出回應的 **狀態碼**、**部分 header**(例如 `Content-Type`)、**body**。

        var request = response.RequestMessage;
        Console.Write("request 相關資訊: ");
        Console.Write($"{request?.Method} ");
        Console.Write($"{request?.RequestUri} ");
        Console.WriteLine($"HTTP/{request?.Version}");
        Console.Write("response 相關資訊: ");
        Console.Write($"狀態碼: {response.StatusCode} ");
        Console.WriteLine($"Content-Type: {response.Content.Headers.ContentType} ");
    }
}

static class DumpExtensions
{
    // 一定要快取成 static readonly, 不要每次 new
    static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    internal static T Dump<T>(this T obj, string? label = null)
    {
        if (label is not null) Console.WriteLine($"--- {label} ---");
        Console.WriteLine(JsonSerializer.Serialize(obj, Options));
        return obj; // 回傳自己, 可以串在運算式中間
    }
}