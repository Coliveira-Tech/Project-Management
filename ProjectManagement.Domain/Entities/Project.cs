namespace ProjectManagement.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Foreign key
        public Guid OwnerId { get; set; }
       
        // Navigation properties
        public virtual ICollection<Task> Tasks { get; set; } = [];
        public virtual User Owner { get; set; } = new User();
    }
}
