using Library.Domain.Book;
using Microsoft.Data.SqlClient;
using System.Text;

namespace Library.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction? _transaction;

        public BookRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        private Book MapBook(SqlDataReader reader) => new(
            reader.GetGuid(0), // Id
            reader.GetString(1), // Title
            reader.GetString(2), // Author
            reader.GetString(3), // ISBN
            reader.IsDBNull(4) ? (DateOnly?)null : DateOnly.FromDateTime(reader.GetDateTime(4)), // PublishedYear
            reader.GetBoolean(5), // Availability
            reader.IsDBNull(6) ? DateTime.MinValue : reader.GetDateTime(6), // BorrowDate
            reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7) // ReturnDate
        );

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            var books = new List<Book>();
            const string query = "SELECT Id, Title, Author, ISBN, PublishedYear, Availability, CreatedDate, UpdatedDate FROM Books";

            using var command = new SqlCommand(query, _connection, _transaction);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) books.Add(MapBook(reader));

            return books;
        }

        public async Task<Book?> GetBookByIdAsync(Guid bookId)
        {
            const string query = "SELECT Id, Title, Author, ISBN, PublishedYear, Availability, CreatedDate, UpdatedDate FROM Books WHERE Id = @Id";
            using var command = new SqlCommand(query, _connection, _transaction);
            command.Parameters.AddWithValue("@Id", bookId);

            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapBook(reader) : null;
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchText, string? title, string? author, string? isbn)
        {
            var queryBuilder = new StringBuilder("SELECT Id, Title, Author, ISBN, PublishedYear, Availability, CreatedDate, UpdatedDate FROM Books WHERE 1=1");
            var parameters = new List<SqlParameter>();

            AddSearchCondition(ref queryBuilder, ref parameters, searchText, "LOWER(Title) LIKE @SearchText OR LOWER(Author) LIKE @SearchText OR ISBN LIKE @SearchText", "@SearchText");
            AddSearchCondition(ref queryBuilder, ref parameters, title, "LOWER(Title) LIKE @Title", "@Title");
            AddSearchCondition(ref queryBuilder, ref parameters, author, "LOWER(Author) LIKE @Author", "@Author");
            AddSearchCondition(ref queryBuilder, ref parameters, isbn, "ISBN = @ISBN", "@ISBN");

            var books = new List<Book>();

            using var command = new SqlCommand(queryBuilder.ToString(), _connection, _transaction);
            command.Parameters.AddRange([.. parameters]);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) books.Add(MapBook(reader));

            return books;
        }

        private void AddSearchCondition(ref StringBuilder queryBuilder, ref List<SqlParameter> parameters, string? value, string condition, string paramName)
        {
            if (!string.IsNullOrEmpty(value))
            {
                queryBuilder.Append(" AND ").Append(condition);
                parameters.Add(new SqlParameter(paramName, $"%{value.ToLower()}%"));
            }
        }

        public async Task<IEnumerable<Book>> GetBorrowingsByUserIdAsync(Guid userId)
        {
            var books = new List<Book>();
            const string query = "SELECT b.Id, b.Title, b.Author, b.ISBN, b.PublishedYear, b.Availability, bor.BorrowDate, bor.ReturnDate " +
                                "FROM Borrowings bor " +
                                "INNER JOIN Books b ON bor.BookID = b.Id " +
                                "WHERE bor.UserID = @UserId AND bor.ReturnDate IS NULL";

            using var command = new SqlCommand(query, _connection, _transaction);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) books.Add(MapBook(reader));

            return books;
        }
    }
}
