using Library.Domain.Abstractions;
using Library.Domain.Services;
using Library.Domain.User;

namespace Library.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User> RegisterAsync(string firstName, string lastName, string email, string password, string? phoneNumber = null)
        {
            await _unitOfWork.BeginTransactionAsync(); // Start transaction

            try
            {
                // Check if the email is already in use
                var existingUser = await _unitOfWork.UserRepository.GetUserByEmail(email);
                if (existingUser != null)
                    throw new InvalidOperationException("Email is already in use.");

                // Hash password before storing it
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                User user = new(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    DateTime.UtcNow,
                    firstName,
                    lastName,
                    email,
                    hashedPassword,
                    phoneNumber
                );

                var registeredUser = await _unitOfWork.UserRepository.Register(user);

                // Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                return registeredUser;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmail(email)
                ?? throw new InvalidOperationException("Invalid email or password.");

            // Verify password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!isPasswordValid)
                throw new InvalidOperationException("Invalid email or password.");

            return user; // Authentication successful
        }

        public async Task<bool> ValidateEmailAsync(string email)
        {
            var existingUser = await _unitOfWork.UserRepository.GetUserByEmail(email);
            return existingUser != null;
        }
    }
}
