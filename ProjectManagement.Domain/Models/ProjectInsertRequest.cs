namespace ProjectManagement.Domain.Models
{
    public class ProjectInsertRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
    }
}
