namespace Library.Domain.Services
{
    public interface IAuthService
    {
        Task<User.User> RegisterAsync(string firstName, string lastName, string email, string password, string? phoneNumber = null);

        Task<User.User> LoginAsync(string email, string password);
        Task<bool> ValidateEmailAsync(string email);
    }
}
