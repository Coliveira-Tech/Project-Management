using ProjectManagement.Domain.Models;

namespace ProjectManagement.Domain.Entities
{
    public class Task : BaseEntity
    {
        public Task() { }

        public Task(TaskInsertRequest request)
        {
            Title = request.Title;
            Description = request.Description;
            DueDate = request.DueDate;
            Status = request.Status;
            Priority = request.Priority;
            ProjectId = request.ProjectId;
            AssignedUserId = request.AssignedUserId;
        }

        public Task(TaskUpdateRequest request)
        {
            Title = request.Title;
            Description = request.Description;
            DueDate = request.DueDate;
            UpdatedAt = request.UpdatedAt;
            Status = request.Status;
            AssignedUserId = request.AssignedUserId;
        }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.None;
        public Enums.TaskPriority Priority { get; set; } = Enums.TaskPriority.None;

        // Foreign key
        public Guid ProjectId { get; set; }
        public Guid AssignedUserId { get; set; }

        // Navigation property
        public virtual Project Project { get; set; } = null!;
        public virtual User AssignedUser { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; } = [];
        public virtual ICollection<TaskHistory> TaskHistory { get; set; } = [];
    }
}
