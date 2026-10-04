public class Member
{
    private readonly List<LibraryItem> _borrowedItems = new List<LibraryItem>();
    public string Name { get; set; }
    public Guid MemberId { get; } = Guid.NewGuid();
    public IReadOnlyList<LibraryItem> BorrowedItems => _borrowedItems;

    public Member(string name)
    {
        Name = name;
    }

    public void AddBorrowedItem(LibraryItem item)
    {
        _borrowedItems.Add(item);
    }

    public void RemoveBorrowedItem(LibraryItem item)
    {
        _borrowedItems.Remove(item);
    }
}