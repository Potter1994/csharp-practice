// - 寫一個 `Library` 類別管理一份 `List<LibraryItem>` 館藏和 `List<Member>` 會員。
// - 實作:借書(檢查是否已被借走)、還書、查詢某會員目前借了哪些書、查詢某本書是否可借。


public class Library(int delayMs, string name)
{
    private readonly List<Member> _members = [];
    private readonly List<LibraryItem> _libraryItems = [];
    private readonly int _delayMs = delayMs;
    public string Name { get; } = name;

    public void AddMember(Member member) => _members.Add(member);
    public void AddLibraryItem(LibraryItem item) => _libraryItems.Add(item);

    // - 實作:借書(檢查是否已被借走)
    public void Borrow(Member member, LibraryItem item)
    {
        if (item.IsBorrowed)
        {
            Console.WriteLine($"這本書已經被 {item.BorrowedBy?.Name} 借走了, 日期歸還在 {item.DueDate:yyyy-MM-dd}");
            return;
        }

        item.Borrow(member);
    }

    // - 實作:還書
    public void Return(LibraryItem item) => item.Return();

    // - 實作:查詢某會員目前借了哪些書
    public void SearchBorrowedListByMember(Member member)
    {
        string libraryItemString = "";

        if (member.BorrowedItems.Count == 0)
        {
            Console.WriteLine("目前沒有借書");
            return;
        }

        // foreach (var item in member.BorrowedItems)
        // {
        //     libraryItemString = libraryItemString.Length == 0 ? item.Title : $"{libraryItemString}, {item.Title}";
        // }

        // 改用 LINQ + string.Join 取代這邊 foreach 跟 string 的做法
        libraryItemString = string.Join(", ", member.BorrowedItems.Select(b => b.Title));

        Console.WriteLine($"{member.Name}目前借了以下這些書 {libraryItemString}");
    }

    // - 實作:查詢某本書是否可借
    public void CheckItem(LibraryItem item)
    {
        // 把這邊 Find 改成使用 LINQ 的方法, 主要是為了一致使用 LINQ 不然使用原本 List 的 Find 方法是更好的
        // LibraryItem? currentItem = _libraryItems.Find((i) => i.ItemId == item.ItemId);
        LibraryItem? currentItem = _libraryItems.FirstOrDefault(b => b.ItemId == item.ItemId);
        Console.WriteLine($"{(currentItem == null ? "這本書不在本圖書館裡" : currentItem.IsBorrowed ? "這本書已經被借走了" : "這本書可以借")}");
    }

    // - 用 LINQ 新增三個查詢:借最多書的會員、所有逾期未還的項目、按館藏類型(Book/Magazine)分組統計數量。
    public void GetMostBookMember()
    {
        var max = _members.MaxBy(m => m.BorrowedItems.Count);
        if (max != null)
        {
            var results = _members.Where(m => m.BorrowedItems.Count == max.BorrowedItems.Count);
            Console.WriteLine($"{string.Join("跟 ", results.Select(m => m.Name))} 借了最多本書");
        }
    }

    public List<LibraryItem> GetDueDateBook()
    {
        var result = _libraryItems.Where(b => b.IsBorrowed && b.DueDate < DateTime.Now).ToList();
        return result;
    }

    public void GetCountGroupByType()
    {
        var groups = _libraryItems.GroupBy(b => b.GetType().Name).ToList();
        foreach (var group in groups)
        {
            Console.WriteLine($"{group.Key} 數量: {group.Count()}");
        }
    }

    public async Task<(string name, bool found)> SearchByName(string bookName)
    {
        await Task.Delay(_delayMs);
        return (Name, _libraryItems.Find(item => item.Title == bookName) != null);
    }
}
