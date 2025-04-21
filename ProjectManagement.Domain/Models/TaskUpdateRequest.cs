namespace ProjectManagement.Domain.Models
{
    public class TaskUpdateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.None;
        public Guid AssignedUserId { get; set; }
    }
}
