public class Book : LibraryItem, IRenewable
{
    public string Author { get; }
    public string ISBN { get; }
    public Book(string title, string author, string isbn) : base(title)
    {
        Author = author;
        ISBN = isbn;
    }

    public override int GetLoanPeriodDays() => 30;
    public void Renew()
    {
        DueDate = DueDate?.AddDays(GetLoanPeriodDays());
    }
}