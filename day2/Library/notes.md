### Day 2:OOP — class / interface / 繼承(圖書館借閱系統)

**驗收標準 1:為什麼用抽象類別 `LibraryItem`,而不是 `Book`、`Magazine` 各自獨立寫?**
(提示:想想 `Borrow()` / `Return()` 那段邏輯如果複製兩份會怎樣;還有 `Library` 裡的 `List<LibraryItem>` 為什麼裝得下兩種型別)
因為需要基本有 `Borrow()` 跟 `Return()` 的實作, 如果沒有用抽象類別就要在每個 class 都各自寫一次可能一模一樣的方法。
而就是因為他們都有書本最基本的借書跟還書的功能了, 又能有各自的書本例如: 雜誌, 漫畫書 等等不一樣的區別, 甚至去實作他們細微不同的例如借書時間等,
只在乎他們有繼承 LibraryItem 的抽象

程式碼重用 —— 共用 Borrow()/Return()(你答了)
多型 —— 一段程式碼能處理所有子類別,新增 Comic : LibraryItem 時 Library 一行都不用改(漏了)

**驗收標準 2:`IBorrowable` 這種 interface 跟繼承的差異是什麼?**
(提示:你讓 `Book` 實作 `IRenewable` 但 `Magazine` 沒有 —— 如果把 `Renew()` 放進 `LibraryItem` 會發生什麼問題?
 另外:一個類別可以繼承幾個父類別?可以實作幾個介面?)

繼承只能繼承一個, 而 interface 則是能好多個, 他主要表明他能夠實作這個 interface 的功能, 例如我範例寫的 IRenewable 有讓 Book 實作, Magazine 卻沒有, 所以 Book 就有這個方法能用, Magazine 就沒有

繼承表達是什麼, 介面表達能做什麼
「是什麼」只能有一個答案(所以單一繼承),「能做什麼」可以有很多個(所以多重實作)。

---

**從自己的設計長出來的問題**

- `DueDate { get; protected set; }` 為什麼是 `protected set` 而不是 `private set`?
  (提示:看 `Book.Renew()` 做了什麼。如果改成 `private set` 會編譯失敗嗎?)
  使用 `protected set` 是這個 class 本身跟子類別都能修改, 如果是 `private set` 就只有 class 自己本身能修改
  所以在 Book 就無法改到繼承自 LibraryItem 的屬性 DueDate 做更改了


- `Member` 裡為什麼是 `private readonly List<LibraryItem> _borrowedItems` 配上
  `public IReadOnlyList<LibraryItem> BorrowedItems => _borrowedItems;`?
  直接開一個 `public List<LibraryItem>` 會有什麼問題?
  (另外:`readonly` 擋得住 `_borrowedItems.Add(...)` 嗎?)

  如果直接配上 `public List<LibraryItem>` 能夠讓取得的人直接做 Clear(); 等方法操作, 特意加上 `IReadOnlyList` 來限制你能做什麼(只能讀取, 不能修改)
  加上 `readonly` 則是限制變數本身,不能重新指向另一個 List, 不是限制 List 裡面的內容不能更改
  


- `GetLoanPeriodDays()` 做成 `abstract` 方法讓子類別 override,
  而不是在建構函式傳入天數(`new Book(title, author, isbn, loanDays: 30)`),好處是什麼?
  好處是在定義子類別時就有統一的 Domain rule, 且強迫每個子類別要實作
  


- `Guid ItemId { get; } = Guid.NewGuid();` 為什麼用 `Guid` 而不是遞增的 `int`?
  (這個 Day 10 接資料庫時會再遇到一次)
  主要還是用 `Guid` 不會像 `int` 可能會再多個建立時造成重複, `Guid` 是產生一個幾乎不會撞的識別碼


- `public class Library(int delayMs, string name)` 這種把參數寫在類別名稱後面的語法叫什麼?
  跟傳統寫一個建構函式差在哪?
  這個語法叫做 Primary Constructor(主要建構函式), 是 C# 12 引入的語法

Primary Constructor Parameter
        ↓
可以被 class body 裡的 member 使用


Traditional Constructor Parameter
        ↓
只存在於 constructor 那個 scope


C# 12 的 Primary Constructor，主要目的是把「類別需要哪些初始化參數」直接寫在類別宣告上，減少傳統 constructor 的 boilerplate。
而且它跟 record 常一起出現，但 Primary Constructor 不只 record 能用，class / struct 也可以用。


---

**實作時踩到或想過的點**

- `LibraryItem.Borrow()` 裡呼叫了 `member.AddBorrowedItem(this)`,
  而 `Library.Borrow()` 又呼叫 `item.Borrow(member)` —— 這樣雙向關聯有什麼風險?
  (提示:如果有人跳過 `Library` 直接呼叫 `item.Borrow(member)` 會怎樣?)
  風險 1:狀態可以被改成不一致
  風險 2:責任重複,規則會走樣
  風險 3:序列化會無限遞迴
  風險 4:繞過 Library 就繞過它的職責


- `Library.SearchBorrowedListByMember` 原本用 foreach 組字串,後來改成
  `string.Join(", ", member.BorrowedItems.Select(b => b.Title))`。為什麼後者比較好?
  1. 去掉了需要判斷是不是第一個字的 3 元判斷式
  2. 比較直觀好看

  2000 筆: foreach 累加     2 ms | string.Join   0 ms
  8000 筆: foreach 累加    36 ms | string.Join   0 ms
 32000 筆: foreach 累加   744 ms | string.Join   0 ms

原因是 string 在 C# 是不可變的(immutable)。s1 + ", " + t 不是「把東西加到 s1 後面」,而是:
1. 配置一個全新的字串(長度 = 舊的 + 新的)
2. 把舊內容整個複製過去
3. 再把新內容複製進去
4. 舊的那個變成垃圾,等 GC 回收


---

**延伸(study-plan 有列,還沒做)**

- 幫 `Book` 加上逾期罰金計算:用 `DateTime` 算超過借期幾天、一天罰多少錢。
這個先不用做了沒關係