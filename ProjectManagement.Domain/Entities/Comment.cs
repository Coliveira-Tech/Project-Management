namespace ProjectManagement.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign key
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }

        // Navigation properties
        public virtual Task Task { get; set; } = new Task();
        public virtual User User { get; set; } = new User();
    }
}
