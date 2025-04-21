using ProjectManagement.Domain.Models;

namespace ProjectManagement.Domain.Entities
{
    public class TaskHistory : BaseEntity
    {
        public TaskHistory() { }

        public TaskHistory(TaskHistoryInsertRequest request)
        {
            Field = request.Field;
            OldValue = request.OldValue;
            NewValue = request.NewValue;
            TaskId = request.TaskId;
            UserId = request.UserId;
        }

        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Field { get; set; } = string.Empty;
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;

        // Foreign key
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }

        // Navigation properties
        public virtual Task Task { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
