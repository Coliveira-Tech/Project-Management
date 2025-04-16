namespace ProjectManagement.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<Project> Projects { get; set; } = [];
        public virtual ICollection<Task> Tasks { get; set; } = [];
    }
}
