namespace ProjectManagement.Domain.Models
{
    public class TaskUpdateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.None;
        public Guid AssignedUserId { get; set; }
    }
}
