/* 練習 1:認識集合型別 
- 分別用 `List<T>`、`Dictionary<K,V>`、`HashSet<T>`、`Queue<T>`、`Stack<T>` 各寫一小段,體會它們各自解決什麼問題。
- 效能實驗:建立 10 萬筆資料,分別用 `List.Contains()` 和 `HashSet.Contains()` 查找 1 萬次,用 `Stopwatch` 量測並記錄差距。想一下為什麼(提示:一個是逐筆比對,一個是雜湊表)。
- 寫下判斷表:什麼情況該用哪一個。


Console.WriteLine("=== List ===");
List<int> list = new List<int> { 1, 2, 3 };
list.Add(2);
Console.WriteLine(list[3]);

Console.WriteLine("=== Dictionary ===");
Dictionary<string, string> dictionary = new Dictionary<string, string>() { ["First"] = "Hello", ["Second"] = "How are you?" };
Console.WriteLine(dictionary["First"]);
if (dictionary.TryGetValue("不存在的鍵", out var v))   // 不會丟例外
    Console.WriteLine(v);

dictionary["Third"] = "新增或覆蓋";   // 索引子:有就改,沒有就加
// dictionary.Add("Third", "只能新增");  // Add:已存在會丟例外 => Unhandled exception. System.ArgumentException: An item with the same key has already been added. 
// dictionary["不存在"] 會丟 KeyNotFoundException,所以不確定時用 TryGetValue —— 就是 Day 1 學的 Try-Pattern。

// HashSet: 去重 + 快速查找
Console.WriteLine("=== HashSet ===");
HashSet<int> hashSet = new HashSet<int>() { 66, 55, 50 };
Console.WriteLine(hashSet.Add(66)); // False: 已存在, 加不進去
Console.WriteLine(hashSet.Contains(66)); // True: 這個 HashSet 有 66 在裡面

// Queue: 先進先出(FIFO)
Console.WriteLine("=== Queue ===");
Queue<string> queue = new Queue<string>();
queue.Enqueue("客人A"); // 排隊
queue.Enqueue("客人B");
queue.Enqueue("客人C");
Console.WriteLine(queue.Dequeue()); // "客人A", 最先來的先服務
Console.WriteLine(queue.Peek()); // "客人B", 偷看下一個但不取出

// Stack: 後進先出(LIFO)
Console.WriteLine("=== Stack ===");
Stack<string> stack = new Stack<string>();
stack.Push("第一步");
stack.Push("第二步");
Console.WriteLine(stack.Pop()); // "第二步", 最後放的先拿
*/


/* 效能實驗
using System.Diagnostics;

List<string> list = new List<string>(100_000);
HashSet<string> hashSet = new HashSet<string>(100_000);

for (int i = 0; i < 100_000; i++)
{
    list.Add($"value: {i + 1}");
    hashSet.Add($"value: {i + 1}");
}

Stopwatch s1 = Stopwatch.StartNew();
for (int i = 0; i < 100_00; i++) list.Contains("value: 60000");
s1.Stop();

Stopwatch s2 = Stopwatch.StartNew();
for (int i = 0; i < 100_00; i++) hashSet.Contains("value: 60000");
s2.Stop();

Console.WriteLine($"使用 List.Contains(\"value: 10000\")的時間: {s1.Elapsed.TotalMilliseconds}");
Console.WriteLine($"使用 hashSet.Contains(\"value: 10000\")的時間: {s2.Elapsed.TotalMilliseconds}");
Console.WriteLine($"差距    : {s1.Elapsed.TotalMilliseconds / s2.Elapsed.TotalMilliseconds:F0} 倍");
*/

/* 判斷表
 集合   |      適用情境                 |   查找   |  有順序    |   允許重複
List<T>: 需要依索引(index)存取, 保持順序     O(n)        ✅           ✅
Dictionary<K,V>: 需要用「鍵」快速找「值」    O(1)      ❌ 不保證      鍵不可
HashSet<T>: 只在乎有沒有、去重,             O(1)      ❌ 不保證       ❌
Queue: 先進先出,                          -            ✅           ✅        
Stack: 後進先出,                          -            ✅           ✅
*/




List<string> sameReference = ["Andy", "Kevin"];

List<Book> list = [
    new("C# 入門", "王大明", 2012, 450, "程式", sameReference),
    new("深入 LINQ", "李曉華", 2020, 680, "程式", ["Judy", "Kane"]),
    new("料理入門", "陳美玲", 2019, 320, "生活", ["Potter", "Scott"]),
    new("阿基師的偷吃步", "阿基師", 2023, 480, "生活", ["Yumi", "Lee"]),
    new("火影忍者", "齊本", 2008, 250, "娛樂", ["Zeus"]),
    new("海賊王", "尾田", 2006, 250, "娛樂", ["Bruce", "Jany"]),
];


/*
**練習 2:LINQ 基礎運算子**
用一組測試資料(例如 Day 2 的書籍清單)練習以下方法,每個都寫一次:
- 篩選:`Where`
- 投影(轉換):`Select`、`SelectMany`
- 排序:`OrderBy`、`OrderByDescending`、`ThenBy`
- 取單一元素:`First`、`FirstOrDefault`、`Single`、`SingleOrDefault`(注意四者差異)
- 判斷:`Any`、`All`、`Contains`
- 彙總:`Count`、`Sum`、`Average`、`Max`、`Min`
- 分組:`GroupBy`
- 轉換:`ToList`、`ToArray`、`ToDictionary`

每一個都跟你熟悉的 JavaScript 陣列方法對照(`filter`/`map`/`sort`/`find`/`some`/`every`/`reduce`),記下哪些有對應、哪些沒有。

// - 篩選:`Where`
// 使用 Where 去篩選分類為程式的, 相當於 js array 的 filter
var programBook = list.Where(b => b.Category == "程式");

// - 投影(轉換):`Select`、`SelectMany`

// 使用 Select 去組成自己想要的格式(也可以用 b => new {b.Author, b.Title}), 可以用 js array 的 map 
var programBookAuthor = programBook.Select(b => b.Author);

// SelectMany (攤平組合)
var programBookBorrowedList = programBook.SelectMany(b => b.BorrowedList);

// 基於 Select 但能夠攤平 List, 相對於 js array 的 flatMap,
// 多載方法不只是攤平還能夠把外層元素和內層元素組合起來, 使用成 js array 的 flatMap + map 的用法

// var pairs = programBook.SelectMany(
//     b => b.BorrowedList,                          // ① 要攤平的集合
    // (book, borrower) => new { book.Title, 借閱者 = borrower });  // ② 怎麼組合父子

// → { Title = "C# 入門",   借閱者 = "Andy"  }
//   { Title = "C# 入門",   借閱者 = "Kevin" }
//   { Title = "深入 LINQ", 借閱者 = "Judy"  }
//   { Title = "深入 LINQ", 借閱者 = "Kane"  }




// - 排序:`OrderBy`、`OrderByDescending`、`ThenBy`

// list.OrderBy((a) => a.Price, Comparer<decimal>.Create((a, b) => a.CompareTo(b));
// 第二個參數使用 Comparer<TKey>.Create(); 可以建立排序的規則, 相當於 js list.sort((a, b) => a - b); 這種用法


// 預設升冪排序, 相當於 js array 的 sort
var orderByPrice = list.OrderBy((a) => a.Price);

// 降冪排序
var orderByDescending = list.OrderByDescending(a => a.Price);

// 在前一個相比一樣的時候, 會接續著 ThenBy 繼續比, 可以一直 ThenBy
var orderByPriceThenByYear = list.OrderBy((a) => a.Price).ThenBy(a => a.Year);
// foreach (var item in orderByDescending) Console.WriteLine($"{item.Title}: {item.Price}, year: {item.Year}");


// - 取單一元素:`First`、`FirstOrDefault`、`Single`、`SingleOrDefault`(注意四者差異)
// FirstOrDefault 相當於 js array 的 find, 而 First js 沒有對應(js 不會因為沒有找到而拋錯)
// Single 在 js array 就沒有差不多的 method 了

// 使用 First 取得第一個符合條件的, 但是當找不到時, 會跳 Unhandled exception
var first = list.First(a => a.Price < 400);

// 使用 FirstOrDefault, 與 First 一樣只是找不到時不會拋出 Unhandled exception
var firstOrDefault = list.FirstOrDefault(a => a.Price < 200);

// Single 找尋符合條件剛好是 1 個的, 當結果為 null 或者 2 個以上會拋出錯誤 Unhandled exception
var single = list.Single(a => a.Price == 450);

// 可以找不到結果或者符合條件剛好是 1 個的, 但是有 2 個以上會拋出錯誤 Unhandled exception
var singleOrDefault = list.SingleOrDefault(a => a.Price == 50);



// - 判斷:`Any`、`All`、`Contains`

// Any 檢查序列裡面有沒有元素或符合條件的某元素, 相當於 js array 的 some
list.Any(); // list.length > 0;
var anyListResult = list.Any(b => b.Price > 500); // arr.some(...)

// All 檢查有沒有全部符合你的 predicate function 判斷, 相當於 js array 的 every 方法
var allListResult = list.All(a => a.Price > 40);

// Contains 檢查元素有沒有在此序列中, 相當於 js array 的 includes
// 原本使用註解的寫法會是 False, 原因是因為 ["Andy", "Kevin"] 是不同的 reference
// var containsListResult = list.Contains(new("C# 入門", "王大明", 2012, 450, "程式", ["Andy", "Kevin"]));
var containsListResult = list.Contains(new("C# 入門", "王大明", 2012, 450, "程式", sameReference));


// - 彙總:`Count`、`Sum`、`Average`、`Max`、`Min`

// Count 是算 List 的長度, 相當於是 js array 的 .length
var count = list.Count();

// Sum 計算總和, 相當於 js array 使用 reduce 去做相加
var sum = list.Sum((b) => b.Price);

// Average 計算平均值, 相當於 js array 使用 reduce 去做相加再相除
var average = list.Average(b => b.Price);

// Max 找最大值, 相當於 js Math.max(); 去拿最大值
var max = list.Max(b => b.Price);

// Math.max(...arr.map(b => b.price))    // ≈ Max(b => b.Price)
list.Max(b => b.Price);      // → 680        (最大的「價格」)

// arr.reduce((a, b) => a.price > b.price ? a : b)   // ≈ MaxBy(b => b.Price)
list.MaxBy(b => b.Price);    // → Book 物件   (價格最大的「那本書」)


// Min 找最小值, 相當於 js Math.min(); 去拿最小值
var min = list.Min(b => b.Price);


// - 分組:`GroupBy`
// GroupBy 是將某個屬性或多個屬性拿來當 key 分組
var groupBy = list.GroupBy(b => b.Category);

// foreach (var group in groupBy)
// {
//     Console.WriteLine($"種類分組: {group.Key}");

//     foreach (var book in group)
//     {
//         Console.WriteLine($"書名: {book.Title}");
//     }
// }



// - 轉換:`ToList`、`ToArray`、`ToDictionary`

// 將 LINQ 的 IEnumerable<T> 的結果轉成 List<T>
var toList = list.Select(b => b.Author).ToList();

// 將 LINQ 的 IEnumerable<T> 的結果轉成 Array<T>
var toArray = list.Where(b => b.Price > 300).ToArray();

// 將 LINQ 的 IEnumerable<T> 的結果轉成 Dictionary<K,V>
var toDictionary = list.Select(b => new { b.Title, b.Author }).ToDictionary(b => b.Author);

*/

/*
**練習 3:延遲執行(這題最重要)**
- 寫一個 `Where` 查詢但**不要**呼叫 `ToList()`,在查詢後修改原始集合,再列舉查詢結果 —— 觀察印出來的是修改前還是修改後的資料。
- 在 `Where` 的條件裡放一行 `Console.WriteLine`,觀察它在哪一刻才被執行。
- 對同一個查詢變數 `foreach` 兩次,數一下條件被執行了幾次。
- 寫下結論:什麼時候查詢才真正執行?`ToList()` 改變了什麼?


// 如果加上 ToList(); 會馬上執行並將當時的結果儲存起來
var result = list.Where(b =>
{
    Console.WriteLine("執行 Where");
    return b.Price > 500;
});

// ToList(); 建立一個全新的 List, 把當下符合的元素複製進去。之後列舉的那個是新 List 與原始集合已經脫勾，原始集合怎麼改變已經無關了
var snapshot = list.Where(b =>
{
    Console.WriteLine("ToList 的 Where");
    return b.Price > 500;
}).ToList();
Console.WriteLine("只有 ToList(); 的程式碼已經執行完 Where 了");

list.Add(new("死神", "保人", 1889, 520, "生活", ["ssd"]));

// 現在跑 foreach 真的使用到 result 才會去執行, 且如果再跑一次 foreach 使用到 result 會再次執行一次
// 且執行的時候都是拿原本集合的 reference + 給的條件
foreach (var book in result)
{
    // 逐元素拉取(pull-based streaming)。foreach 每要一個,Where 才去處理一個。
    // hugeList.Where(...).First()      // 找到第一個就停,不會掃完全部
    Console.WriteLine($"--- {book.Title}: {book.Price}");
}
// 如果再 foreach 跑一次沒有 shapshot 的就會再次邊跑邊 Where 去處理
foreach (var book in result)
{
    Console.WriteLine($"--- {book.Title}: {book.Price}");
}
// 甚至在執行一次 Count(); 會再次去跑 Where 處理, 整個非常浪費
Console.WriteLine(result.Count());


foreach (var book in snapshot)
{
    Console.WriteLine($"ToList: {book.Title}: {book.Price}");
}
*/



/*
**練習 4:Generics**
- 寫一個泛型方法 `T? FindMax<T>(IEnumerable<T> items) where T : IComparable<T>`,回傳最大值。
- 寫一個泛型類別 `SimpleRepository<T>`,內含 `Add`、`GetAll`、`FindById` —— 為第二週的 Repository 層暖身。
- 試著加上不同的泛型約束(`where T : class`、`where T : struct`、`where T : new()`),觀察各自允許什麼。
*/

/*
T? FindMax<T>(IEnumerable<T> items) where T : IComparable<T>
{
    T? max = default;
    bool isValid = false;
    foreach (T item in items)
    {
        if (item is null) continue;
        if (!isValid || item.CompareTo(max) > 0)
        {
            max = item;
            isValid = true;
        }

    }

    // 丟例外, 因為要讓他知道他這個方法沒有真的拿到 Max
    if (!isValid) throw new InvalidOperationException("序列中沒有任何元素");

    return max;
}

var result = FindMax([-4, -2, -3]);
// Console.WriteLine(result);

var repo = new SimpleRepository<Book>();
repo.Add(list[0]);
repo.Add(list[1]);
Console.WriteLine(repo.GetAll().Count);
Console.WriteLine(repo.FindById(list[0].Id)?.Title);
Console.WriteLine(repo.FindById(Guid.NewGuid())?.Title ?? "找不到");
// repo.GetAll().Clear();  // 改成 IReadOnlyList 後這行編譯不過

// - 試著加上不同的泛型約束(`where T : class`、`where T : struct`、`where T : new()`),觀察各自允許什麼。
void TestClass<T>(T value) where T : class
{
    value = null;  // 有跳提示説把 Converting null literal or possible null value to non-nullable type.
}

void TestStruct<T>(T value) where T : struct
{
    // value = null; 這邊無法編譯成功 Cannot convert null to type parameter 'T' because it could be a non-nullable value type. Consider using 'default(T)' instead.
    Console.WriteLine(default(T));
}

T TestNew<T>() where T : new()
{
    return new T();
}

TestClass(new Book("Test", "Test Author", 2025, 100, "生活", []));
TestStruct(new TestStruct());
Console.WriteLine(TestNew<TestStruct>());

struct TestStruct { }

// - 寫一個泛型類別 `SimpleRepository<T>`,內含 `Add`、`GetAll`、`FindById` —— 為第二週的 Repository 層暖身。
class SimpleRepository<T> where T : IEntity
{
    private readonly List<T> _items = [];

    public void Add(T item) => _items.Add(item);

    // 使用 IReadOnlyList 才能防止外部直接修改
    public IReadOnlyList<T> GetAll() => _items;
    public T? FindById(Guid id) => _items.Find(item => id == item.Id);
}
*/


public interface IEntity
{
    // public 可以省略, 預設 interface 就是 public 的
    public Guid Id { get; }
}

record Book(string Title, string Author, int Year, decimal Price, string Category, List<string> BorrowedList) : IEntity
{
    public Guid Id { get; } = Guid.NewGuid();
}

