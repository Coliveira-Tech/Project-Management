using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.None;

        // Navigation properties
        public virtual ICollection<Project> Projects { get; set; } = [];
        public virtual ICollection<Task> Tasks { get; set; } = [];
        public virtual ICollection<Comment> Comments { get; set; } = [];
        public virtual ICollection<TaskHistory> TaskHistory { get; set; } = [];
    }
}
