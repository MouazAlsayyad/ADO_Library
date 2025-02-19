namespace Library.Domain.Borrowing
{
    public interface IBorrowingRepository
    {
        Task BorrowBookAsync(Guid userId, Guid bookId);
        Task ReturnBookAsync(Guid userId, Guid bookId);
    }
}
