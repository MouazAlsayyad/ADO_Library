using Library.Domain.User;
using Microsoft.Data.SqlClient;

namespace Library.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction? _transaction;

        public UserRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<User> Register(User user)
        {
            const string query = "INSERT INTO Users (Id, CreatedDate, UpdatedDate, FirstName, LastName, Email, Password, PhoneNumber) " +
                                 "VALUES (@Id, @CreatedDate, @UpdatedDate, @FirstName, @LastName, @Email, @Password, @PhoneNumber)";

            using var command = new SqlCommand(query, _connection, _transaction);
            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@CreatedDate", user.CreatedDate);
            command.Parameters.AddWithValue("@UpdatedDate", user.UpdatedDate);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@PhoneNumber", (object?)user.PhoneNumber ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();

            return new User(user.Id, user.CreatedDate, user.UpdatedDate, user.FirstName, user.LastName, user.Email, user.Password, user.PhoneNumber);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            const string query = "SELECT Id, CreatedDate, UpdatedDate, FirstName, LastName, Email, Password, PhoneNumber FROM Users WHERE Email = @Email";
            using var command = new SqlCommand(query, _connection, _transaction);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User(
                    reader.GetGuid(0),
                    reader.GetDateTime(1),
                    reader.GetDateTime(2),
                    reader.GetString(3),
                    reader.GetString(4),
                    reader.GetString(5),
                    reader.GetString(6),
                    reader.IsDBNull(7) ? null : reader.GetString(7)
                );
            }
            return null;
        }
    }
}
