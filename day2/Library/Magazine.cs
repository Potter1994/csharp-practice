public class Magazine : LibraryItem
{
    public int IssueNumber { get; }

    public Magazine(string title, int issueNumber) : base(title)
    {
        IssueNumber = issueNumber;
    }

    public override int GetLoanPeriodDays() => 7;
}