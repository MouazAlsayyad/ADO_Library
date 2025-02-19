namespace Library.Domain.Book
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(Guid bookId);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchText, string? title, string? author, string? isbn);
        Task<IEnumerable<Book>> GetBorrowingsByUserIdAsync(Guid userId);
    }
}
