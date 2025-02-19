namespace Library.Domain.Book
{
    public sealed record BookDto
    (
        Guid Id,
        string Title,
        string Author,
        string ISBN,
        DateOnly? PublishedYear,
        bool Availability
    );
}
