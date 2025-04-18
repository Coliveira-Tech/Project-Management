using ProjectManagement.Domain.Models;

namespace ProjectManagement.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Foreign key
        public Guid OwnerId { get; set; }
       
        // Navigation properties
        public virtual List<Task> Tasks { get; } = [];
        public virtual User Owner { get; set; } = new User();
    }
}
