using Library.Domain.Entities;

namespace Library.Domain.Book
{
    public sealed class Book : Entity
    {
        public Book(Guid id, string title, string author, string isbn, DateOnly? publishedYear, bool availability, DateTime createdDate, DateTime updatedDate)
            : base(id, createdDate, updatedDate)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
            PublishedYear = publishedYear;
            Availability = availability;
        }

        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public DateOnly? PublishedYear { get; set; }
        public bool Availability { get; set; } = true;
    }
}
