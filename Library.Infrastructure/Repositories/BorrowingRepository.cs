using Library.Domain.Book;
using Library.Domain.Borrowing;
using Microsoft.Data.SqlClient;

namespace Library.Infrastructure.Repositories
{
    public class BorrowingRepository : IBorrowingRepository
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction? _transaction;

        public BorrowingRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task BorrowBookAsync(Guid userId, Guid bookId)
        {
            var book = await GetBookByIdAsync(bookId)
                ?? throw new InvalidOperationException($"The book with ID {bookId} does not exist.");

            if (!book.Availability)
                throw new InvalidOperationException($"The book with ID {bookId} is not available for borrowing.");

            await UpdateBookAvailabilityAsync(bookId, false);  // Set book as unavailable
            await InsertBorrowingRecordAsync(userId, bookId);
        }

        public async Task ReturnBookAsync(Guid userId, Guid bookId)
        {
            var book = await GetBookByIdAsync(bookId)
                 ?? throw new InvalidOperationException($"The book with ID {bookId} does not exist.");

            if (book.Availability)
                throw new InvalidOperationException($"The book with ID {bookId} is not borrowed.");

            await UpdateBookAvailabilityAsync(bookId, true);  // Set book as available
            await UpdateBorrowingRecordAsync(userId, bookId);
        }

        private async Task UpdateBookAvailabilityAsync(Guid bookId, bool availability)
        {
            var query = "UPDATE Books SET Availability = @Availability WHERE Id = @BookId AND Availability = @CurrentAvailability";
            using var command = new SqlCommand(query, _connection, _transaction);
            command.Parameters.AddWithValue("@BookId", bookId);
            command.Parameters.AddWithValue("@Availability", availability ? 1 : 0);
            command.Parameters.AddWithValue("@CurrentAvailability", availability ? 0 : 1);

            if (await command.ExecuteNonQueryAsync() == 0)
            {
                throw new InvalidOperationException($"Book {bookId} could not be updated.");
            }
        }

        private async Task<Book?> GetBookByIdAsync(Guid bookId)
        {
            const string query = "SELECT Id, Title, Author, ISBN, PublishedYear, Availability, CreatedDate, UpdatedDate FROM Books WHERE Id = @BookId";
            using var command = new SqlCommand(query, _connection, _transaction);
            command.Parameters.AddWithValue("@BookId", bookId);

            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync()
                ? new Book(
                    reader.GetGuid(0), reader.GetString(1), reader.GetString(2),
                    reader.GetString(3), DateOnly.FromDateTime(reader.GetDateTime(4)),
                    reader.GetBoolean(5), reader.GetDateTime(6), reader.GetDateTime(7))
                : null;
        }

        private async Task InsertBorrowingRecordAsync(Guid userId, Guid bookId)
        {
            const string query = "INSERT INTO Borrowings (UserID, BookID, BorrowDate) VALUES (@UserId, @BookId, @BorrowDate)";
            using var command = new SqlCommand(query, _connection, _transaction);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@BookId", bookId);
            command.Parameters.AddWithValue("@BorrowDate", DateTime.UtcNow);
            await command.ExecuteNonQueryAsync();
        }

        private async Task UpdateBorrowingRecordAsync(Guid userId, Guid bookId)
        {
            const string query = "UPDATE Borrowings SET ReturnDate = @ReturnDate WHERE UserID = @UserId AND BookID = @BookId AND ReturnDate IS NULL";
            using var command = new SqlCommand(query, _connection, _transaction);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@BookId", bookId);
            command.Parameters.AddWithValue("@ReturnDate", DateTime.UtcNow);

            if (await command.ExecuteNonQueryAsync() == 0)
            {
                throw new InvalidOperationException("No active borrowing record found for this user and book.");
            }
        }
    }
}
