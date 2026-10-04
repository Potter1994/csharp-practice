public abstract class LibraryItem : IBorrowable
{
    public string Title { get; }
    public Guid ItemId { get; } = Guid.NewGuid();
    public bool IsBorrowed { get; private set; }
    public Member? BorrowedBy { get; private set; }
    public DateTime? DueDate { get; protected set; }
    protected LibraryItem(string title)
    {
        Title = title;
    }

    public abstract int GetLoanPeriodDays();
    public bool Borrow(Member member)
    {
        if (IsBorrowed) return false;
        IsBorrowed = true;
        BorrowedBy = member;
        DueDate = DateTime.Today.AddDays(GetLoanPeriodDays());
        member.AddBorrowedItem(this);
        return true;
    }
    public void Return()
    {
        if (!IsBorrowed) return;
        BorrowedBy?.RemoveBorrowedItem(this);
        IsBorrowed = false;
        BorrowedBy = null;
        DueDate = null;
    }
}