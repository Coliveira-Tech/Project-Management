using ProjectManagement.Domain.Models;

namespace ProjectManagement.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign key
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }

        // Navigation properties
        public virtual Task Task { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
