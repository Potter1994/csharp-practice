/* 練習 2:Thread 基礎與 Race Condition
int counter = 0;

// 專屬的鎖物件
object gate = new object();

Thread threadA = new Thread(AddCounter);
Thread threadB = new Thread(AddCounter);

void AddCounter()
{
    for (int i = 0; i < 100000; i++)
    {
        // 鎖住鎖物件, 控制誰能夠進入這個區塊
        // 這叫原子性(atomicity): 要嘛整組完成, 要嘛還沒開始外人看不到中間狀態
        lock (gate)
        {
            counter++;
        }
    }
    Console.WriteLine(counter);
}

threadA.Start();
threadB.Start();
threadA.Join();
threadB.Join();
Console.WriteLine($"Process variable counter = {counter}");
*/


/* 練習 3:Task 與 async/await
- 寫一個模擬 I/O 的方法 `async Task<string> FetchDataAsync(string name, int delayMs)`,內部用 `await Task.Delay(delayMs)` 模擬等待,回傳一段字串。
- 依序(await 三次,一個接一個)呼叫三次不同 delay 的 `FetchDataAsync`,印出總花費時間。
- 改用 `Task.WhenAll` 同時發出三個呼叫,再印出總花費時間,比較跟依序執行的差異,理解 async 如何讓 I/O 等待時間重疊。

using System.Diagnostics;

async Task<string> FetchDataAsync(string name, int delayMs)
{
    await Task.Delay(delayMs);
    return name;
}


Stopwatch stopwatch = Stopwatch.StartNew();
Console.WriteLine(await FetchDataAsync("Potter", 500));
Console.WriteLine(await FetchDataAsync("Andy", 1500));
Console.WriteLine(await FetchDataAsync("James", 1000));
stopwatch.Stop();

Stopwatch stopwatch1 = Stopwatch.StartNew();
var results = await Task.WhenAll([FetchDataAsync("Potter", 500), FetchDataAsync("Andy", 1500), FetchDataAsync("James", 1000)]);
stopwatch1.Stop();

Console.WriteLine($"三個 await: {stopwatch.ElapsedMilliseconds}");
Console.WriteLine($"使用 Task WhenAll 方法: {stopwatch1.ElapsedMilliseconds}");
*/

/*練習 4(整合小專案):串接 Day 2 的圖書館系統
- 把 Day 2 的 `Library` 加一個模擬「查詢多個分館庫存」的情境:寫 3 個 `async` 方法模擬向 3 個分館查詢某本書庫存(各自用不同的 `Task.Delay` 模擬網路延遲)。
- 用 `Task.WhenAll` 同時查詢 3 個分館,彙整結果印出「哪個分館有現貨」。
*/

using System.Diagnostics;

// ① 建三個分館,延遲各不同
Library branchA = new Library(500, "A Library");
Library branchB = new Library(2000, "B Library");
Library branchC = new Library(1500, "C Library");


// ② 各自放書 —— 故意讓某些分館有、某些沒有
branchA.AddLibraryItem(new Book("Harry Potter", "Rowling", "isbn-1"));
branchC.AddLibraryItem(new Book("Harry Potter", "Rowling", "isbn-2"));
// branchB 故意不放,測試「沒現貨」的情況

// Console.WriteLine(await branchA.SearchByName("Harry Potter"));
// Console.WriteLine(await branchB.SearchByName("Harry Potter"));
// Console.WriteLine(await branchC.SearchByName("Harry Potter"));

// ③ 同時查三個分館
var sw = Stopwatch.StartNew();

// TODO: 用 Task.WhenAll 把三個 SearchByName 包起來
Task<(string, bool)>[] librarySearch = [branchA.SearchByName("Harry Potter"), branchB.SearchByName("Harry Potter"), branchC.SearchByName("Harry Potter")];
(string, bool)[] result = await Task.WhenAll(librarySearch);

sw.Stop();

// ④ 印出哪個分館有現貨 + 總耗時
for (int i = 0; i < result.Length; i++)
{
    (string name, bool found) = result[i];
    if (found) Console.WriteLine($"{name} 有");

}

Console.WriteLine(sw.ElapsedMilliseconds);