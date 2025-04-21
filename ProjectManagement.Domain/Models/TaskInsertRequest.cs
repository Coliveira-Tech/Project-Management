namespace ProjectManagement.Domain.Models
{
    public class TaskInsertRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.None;
        public Enums.TaskPriority Priority { get; set; } = Enums.TaskPriority.None;
        public Guid ProjectId { get; set; }
        public Guid AssignedUserId { get; set; }
    }
}
