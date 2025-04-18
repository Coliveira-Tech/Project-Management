using ProjectManagement.Domain.Models;

namespace ProjectManagement.Domain.Entities
{
    public class TaskHistory : BaseEntity
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Field { get; set; } = string.Empty;
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;

        // Foreign key
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }

        // Navigation properties
        public virtual Task Task { get; set; } = new Task();
        public virtual User User { get; set; } = new User();
    }
}
