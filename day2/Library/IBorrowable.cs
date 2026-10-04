public interface IBorrowable
{
    bool Borrow(Member member);
    void Return();
}