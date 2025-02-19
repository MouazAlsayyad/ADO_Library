namespace Library.Domain.User
{
    public interface IUserRepository
    {
        Task<User> Register(User user);
        Task<User?> GetUserByEmail(string email);
    }
}
