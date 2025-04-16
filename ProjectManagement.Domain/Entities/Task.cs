namespace ProjectManagement.Domain.Entities
{
    public class Task
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Pending;
        public Enums.TaskPriority Priority { get; set; } = Enums.TaskPriority.Low;

        // Foreign key
        public Guid ProjectId { get; set; }
        public Guid AssignedUserId { get; set; }

        // Navigation property
        public virtual Project Project { get; set; } = new Project();
        public virtual User AssignedUser { get; set; } = new User();
        public virtual ICollection<Comment> Comments { get; set; } = [];
    }
}
