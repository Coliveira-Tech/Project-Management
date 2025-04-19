using ProjectManagement.Domain.Models;

namespace ProjectManagement.Domain.Entities
{
    public class Project : BaseEntity
    {
        public Project() { }

        public Project(ProjectInsertRequest request)
        {
            Name = request.Name;
            Description = request.Description;
            OwnerId = request.OwnerId;
        }

        public Project(ProjectUpdateRequest request)
        {
            Name = request.Name;
            Description = request.Description;
            OwnerId = request.OwnerId;
        }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
       
        // Navigation properties
        public List<Task> Tasks { get; set; } = [];
        public User Owner { get; set; } = null!;
    }
}
