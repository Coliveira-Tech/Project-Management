using ProjectManagement.Domain.Models;

namespace ProjectManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<Project> Projects { get; set; } = [];
        public virtual ICollection<Task> Tasks { get; set; } = [];
        public virtual ICollection<Comment> Comments { get; set; } = [];
        public virtual ICollection<TaskHistory> TaskHistory { get; set; } = [];
    }
}
