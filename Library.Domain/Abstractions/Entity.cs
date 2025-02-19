namespace Library.Domain.Entities
{
    public class Entity
    {
        protected Entity(Guid id, DateTime createdDate, DateTime updatedDate)
        {
            Id = id;
            CreatedDate = createdDate;
            UpdatedDate = updatedDate;
        }
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}
