# Day 3 筆記:記憶體、CPU、Stack vs Heap

## 練習 5:核心觀念

### Stack 存什麼、Heap 存什麼

1. **Stack** 存實值型別的值,或是「參考型別變數所持有的位址」
2. **Heap** 存參考型別的物件本體

### Stack 存取為什麼比較快

**配置與釋放幾乎零成本**

Stack 配置就是移動堆疊指標(一個 CPU 指令),函式返回時全部一次釋放,不需要任何回收機制。
Heap 要找空閒空間、要記錄存活狀態、之後還要 GC 掃描。

**快取局部性**

- Stack 和實值型別陣列是連續資料,CPU 一次抓一條快取行(M5 是 128 bytes)就涵蓋好幾筆 → 命中率高
- 參考型別要多跳一次位址,而且物件一旦散落,存取就容易未命中

實測(1000 萬筆走訪):

| 情況 | 耗時 | 說明 |
|---|---|---|
| struct 陣列(連續) | 6 ms | |
| class 陣列(連續配置) | 6 ms | 連續時沒差,預取器擋掉了 |
| class 陣列(打亂順序) | 51 ms | 散落時的真實代價 |

> **結論:參考型別不是「一定慢」,是「容易變慢」。**

※ 這是**速度**的差異。練習 4 量到的 7.6 倍是**記憶體用量**,
成因是標頭 + 陣列參考 + GC 管理開銷,與快取無關。

### 實值型別什麼時候也會放到 Heap

1. **被裝箱** — `object n = 42;`
2. **身為 class 的欄位** — 跟著那個物件一起在 heap
3. **身為陣列的元素** — 跟著陣列一起在 heap

> **規則:實值型別存在「它被宣告的地方」。**
> 區域變數 → stack;class 的欄位 → 跟著那個物件走;陣列元素 → 跟著陣列走。
>
> 所以「struct 一定在 stack」是錯的 —— 練習 4 那 100 萬個 `PointStruct`
> 全部住在 heap 上(因為陣列是參考型別),只是連續內嵌、沒有各自的物件標頭。

### Boxing 為什麼有成本

| 成本 | 說明 |
|---|---|
| heap 配置 | 要找空間 |
| 複製資料 | 值要抄進箱子 |
| 物件標頭 | 16 bytes(型別指標 + 同步區塊)。`int` 本體才 4 bytes,箱子最小 24 bytes,**膨脹 6 倍** |
| 間接存取 | 讀值要多跳一次 |
| GC 壓力 | 每個箱子都是 GC 要追蹤、回收的對象(見練習 4 那 21 MB 管理費) |
| 拆箱要型別檢查 | 型別不符會丟 `InvalidCastException` |

---

## 練習 4:記憶體用量實測

| | `GetTotalMemory` | 實際資料量 |
|---|---|---|
| `PointStruct[1M]` | 7 MB | 7.6 MB |
| `PointClass[1M]` | 53 MB | 32 MB(8 MB 陣列 + 24 MB 物件) |

物件實際大小(用 `GC.GetAllocatedBytesForCurrentThread()` 量測):

```
16 bytes 標頭 + 欄位,最小 24 bytes,對齊到 8 的倍數
→ PointClass(8 bytes 欄位)= 24 bytes
```

**53 − 32 = 21 MB 的差額是 GC 堆的管理開銷:**
.NET 7+ 用 regions 管理記憶體,未填滿的區塊也整塊計入,加上 GC 簿記資料與對齊空隙。
這就是管理 100 萬個獨立物件的代價。

> 量測工具的差別:
> - `GC.GetTotalMemory(true)` — GC **堆的大小**,含未使用空間
> - `GC.GetAllocatedBytesForCurrentThread()` — **實際配置的位元組數**,較精確

---

## 附錄 A:「移動堆疊指標」是什麼意思

Stack 是一塊連續的記憶體,CPU 裡有一個暫存器叫**堆疊指標(SP)**,記著「已用到哪裡」。

```csharp
void Foo()
{
    int a = 1;       // 4 bytes
    int b = 2;       // 4 bytes
    PointStruct p;   // 8 bytes
}
```

```
        [已使用]                      [未使用]
 ┌──────────────────┬───┬───┬──────┬──────────────────┐
 │  呼叫 Foo 之前     │ a │ b │  p   │                  │
 └──────────────────┴───┴───┴──────┴──────────────────┘
                    ▲                ▲
                  進入 Foo 時       進入 Foo 前
                  SP 在這           SP 在這
```

- **進入 `Foo`:`SP -= 16`** — 一個 CPU 指令,三個變數的空間就配置好了
- **離開 `Foo`:`SP += 16`** — 一個 CPU 指令,三個變數全部釋放

沒有搜尋、沒有記帳、沒有回收器。

### 為什麼 Stack 可以這麼簡單

因為它有嚴格的**後進先出**紀律。函式呼叫天生就是巢狀的:

```
Main 呼叫 Foo 呼叫 Bar
→ Bar 一定比 Foo 先結束
→ 「最後配置的一定最先釋放」永遠成立
→ 只要一個指標就夠了
```

Heap 做不到這件事,因為物件的生命週期是任意的 —— 早建立的物件可能活得比晚建立的久。
所以 Heap 必須記錄「哪些區塊還在用」「哪裡有空洞」,並且需要 GC 定期判斷誰還活著。
**這就是成本的來源。**

---

## 附錄 B:「Heap 散落各處」的準確意思

### 剛配置時其實是連續的

.NET 的 GC 配置物件時就是「配置指標往前推」,跟 stack 有點像。
所以在迴圈裡連續 `new` 100 萬個物件,它們在 heap 上大致是排在一起的。
(這也是為什麼上面實測中,連續配置的 class 陣列走訪速度跟 struct 一樣。)

### 長期執行的程式才會變散

```
時間點 1:配置 A B C D
  ┌───┬───┬───┬───┐
  │ A │ B │ C │ D │
  └───┴───┴───┴───┘

時間點 2:B 和 D 死了
  ┌───┬───┬───┬───┐
  │ A │ ░ │ C │ ░ │   ← 出現空洞
  └───┴───┴───┴───┘

時間點 3:配置 E F,GC 壓縮搬移
  ┌───┬───┬───┬───┬───┐
  │ A │ C │ E │ F │   │   ← A 和 C 現在相鄰了,但原本隔很遠
  └───┴───┴───┴───┴───┘
```

真實程式裡物件不斷生死、GC 不斷搬移,邏輯上相關的物件(例如一個清單裡的元素)
很容易散落在不同位置。**這才是「散落各處」的準確意思** —— 是長期執行的結果,不是配置當下的狀態。

---

## 延伸:`record struct` 和 `class` 的差異

這兩者同時在**兩個獨立的軸**上不同,拆開看才清楚:

|  | 非 record | record |
|---|---|---|
| **參考型別** | `class` | `record`(= `record class`) |
| **實值型別** | `struct` | `record struct` |

- 橫軸(record 與否)= 編譯器要不要幫忙產生「值語意」的成員
- 縱軸(struct / class)= 記憶體模型,也就是本篇前面整理的全部內容

### 軸一:`record` 自動產生什麼

```csharp
public record struct PointRec(int X, int Y);   // 就這一行
```

| 功能 | 效果 |
|---|---|
| 值相等 | `Equals` / `==` / `GetHashCode` 比較所有欄位 |
| 可讀的 `ToString()` | `PointRec { X = 1, Y = 2 }` 而不是型別名 |
| `with` 運算式 | `c with { Y = 99 }` → 複製一份再改其中一個 |
| 解構 | `var (x, y) = c;` |
| 主建構式 | 參數列直接變成屬性 |

手寫這五樣大概要 40 行,而且 `GetHashCode` 很容易寫錯。

實測對照:

```
class    相等? False   ToString: PointClass
record   相等? True    ToString: PointRec { X = 1, Y = 2 }
```

兩個 `X=1, Y=2` 的 class 物件**不相等**,因為 class 預設比的是**身分**(是不是同一個物件);
record 比的是**內容**。

### 軸二:記憶體(見本篇前面)

| | `class` | `record struct` |
|---|---|---|
| 存在哪 | Heap,變數存位址 | 宣告的地方(stack / 內嵌) |
| 物件標頭 | 16 bytes | 無 |
| 傳遞時複製 | 位址 | 整份資料 |
| 陣列 100 萬個 | 53 MB | 7.6 MB |
| 可以是 `null` | 可以 | 不行 |
| GC 負擔 | 每個都要追蹤 | 無 |

所以 `record struct` = **struct 的記憶體效率 + record 的語法便利**。

本日練習的 `PointStruct` 其實可以寫成一行:

```csharp
public readonly record struct PointStruct(int X, int Y);
```

### ⚠️ 坑:`record struct` 預設是可變的

```csharp
var m = new PointRec(1, 2);
m.X = 100;                      // 編譯得過!→ PointRec { X = 100, Y = 2 }
```

`record class` 的主建構式屬性是 `init`-only(建立後不可改),
但 **`record struct` 產生的是可讀可寫的屬性**。要不可變必須明確寫 `readonly`:

```csharp
public readonly record struct PointReadonly(int X, int Y);
// r.X = 100;   ← 這樣才會編譯錯誤
```

**建議一律加 `readonly`。** 可變的 struct 特別容易寫出「改到複本」的 bug ——
練習 1 的 `ModifyStruct(p)` 裡 `p.X = 999` 白改一場,就是這個問題。

### 怎麼選

關鍵問題:**兩個欄位值相同的東西,算不算「同一個」?**

```
new Point(3, 4) 和 new Point(3, 4)      → 是同一個點     → 值語意   → record struct
new Member("Amy") 和 new Member("Amy")  → 是兩個不同的人 → 身分語意 → class
```

Day 2 的 `Member` 特地加了 `MemberId`,就是因為「同名不同人」—— 那是**身分**,必須用 class。

選擇順序:

1. 有身分、會變動、體積大 → `class`
2. 有身分、但主要是資料容器(DTO、API 回傳)→ `record`(class 版)
3. 無身分、小(≤16 bytes)、不可變 → `readonly record struct`
4. 需要極致控制記憶體佈局 → `struct`(手寫)

> 第 2 項在寫 API 時會大量用到 —— DTO 幾乎都是 `record`,
> 因為它們就是「一包資料」,值相等和 `with` 都很好用。
