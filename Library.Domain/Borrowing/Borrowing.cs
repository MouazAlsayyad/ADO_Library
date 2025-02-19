using Library.Domain.Entities;

namespace Library.Domain.Borrowing
{
    public sealed class Borrowing : Entity
    {
        public Borrowing(Guid id, Guid bookID, Guid userID, DateTime borrowDate, DateTime? returnDate, DateTime createdDate, DateTime updatedDate)
            : base(id, createdDate, updatedDate)
        {
            BookID = bookID;
            UserID = userID;
            BorrowDate = borrowDate;
            ReturnDate = returnDate;
        }

        public Guid BookID { get; set; }
        public Guid UserID { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
