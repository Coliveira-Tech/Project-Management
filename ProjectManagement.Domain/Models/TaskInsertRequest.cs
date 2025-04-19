namespace ProjectManagement.Domain.Models
{
    public class TaskInsertRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Pending;
        public Enums.TaskPriority Priority { get; set; } = Enums.TaskPriority.Low;
        public Guid ProjectId { get; set; }
        public Guid AssignedUserId { get; set; }
    }
}
