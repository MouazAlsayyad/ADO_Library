using Library.Domain.Book;
using Library.Domain.Borrowing;
using Library.Domain.User;

namespace Library.Domain.Abstractions
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IUserRepository UserRepository { get; }
        IBookRepository BookRepository { get; }
        IBorrowingRepository BorrowingRepository { get; }
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
