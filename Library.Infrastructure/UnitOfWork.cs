using Library.Domain.Abstractions;
using Library.Domain.Book;
using Library.Domain.Borrowing;
using Library.Domain.User;
using Library.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;

namespace Library.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly string _connectionString;
        private readonly SqlConnection _connection;
        private SqlTransaction? _transaction;
        private IBookRepository? _bookRepository;
        private IBorrowingRepository? _borrowingRepository;
        private IUserRepository? _userRepository;

        public UnitOfWork(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
        }

        public IBookRepository BookRepository =>
        _bookRepository ??= new BookRepository(_connection, _transaction!);

        public IBorrowingRepository BorrowingRepository =>
            _borrowingRepository ??= new BorrowingRepository(_connection, _transaction!);

        public IUserRepository UserRepository =>
            _userRepository ??= new UserRepository(_connection, _transaction!);

        public async Task BeginTransactionAsync()
        {
            _transaction = _connection.BeginTransaction();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await Task.Run(_transaction.Commit);
                await _transaction.DisposeAsync();
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await Task.Run(_transaction.Rollback);
                await _transaction.DisposeAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
                await _transaction.DisposeAsync();

            if (_connection != null)
                await _connection.DisposeAsync();
        }

    }
}
