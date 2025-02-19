using Microsoft.Data.SqlClient;

namespace Library.Infrastructure
{
    public class DatabaseSeeder
    {
        private readonly string _connectionString;

        public DatabaseSeeder(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SeedBooks()
        {
            // Check if the table already has data
            if (IsBooksTableEmpty())
            {
                var books = new[]
                {
                    new { Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", ISBN = "9780743273565", PublishedYear = new DateTime(1925, 4, 10) },
                    new { Title = "1984", Author = "George Orwell", ISBN = "9780451524935", PublishedYear = new DateTime(1949, 6, 8) },
                    new { Title = "To Kill a Mockingbird", Author = "Harper Lee", ISBN = "9780061120084", PublishedYear = new DateTime(1960, 7, 11) },
                };

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    foreach (var book in books)
                    {
                        var command = new SqlCommand("INSERT INTO Books (Title, Author, ISBN, PublishedYear, CreatedDate, UpdatedDate) " +
                                                     "VALUES (@Title, @Author, @ISBN, @PublishedYear, GETUTCDATE(), GETUTCDATE())", connection);

                        command.Parameters.AddWithValue("@Title", book.Title);
                        command.Parameters.AddWithValue("@Author", book.Author);
                        command.Parameters.AddWithValue("@ISBN", book.ISBN);
                        command.Parameters.AddWithValue("@PublishedYear", book.PublishedYear);

                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                Console.WriteLine("Books table already contains data. Skipping seeding.");
            }
        }

        private bool IsBooksTableEmpty()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var command = new SqlCommand("SELECT COUNT(1) FROM Books", connection);
                var result = (int)command.ExecuteScalar();

                return result == 0;
            }
        }
    }
}
