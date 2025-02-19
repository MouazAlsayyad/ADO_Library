using Library.Domain.Entities;

namespace Library.Domain.User
{
    public sealed class User : Entity
    {
        public User(Guid id, DateTime createdDate, DateTime updatedDate, string firstName, string lastName, string email, string password, string? phoneNumber) : base(id, createdDate, updatedDate)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            PhoneNumber = phoneNumber;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
