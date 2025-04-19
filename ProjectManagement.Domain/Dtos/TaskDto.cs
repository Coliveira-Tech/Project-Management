namespace ProjectManagement.Domain.Dtos
{
    public class TaskDto
    {
        public TaskDto() { }
        public TaskDto(Entities.Task entity)
        {
            if (entity == null)
                return;

            Id = entity.Id;
            Title = entity.Title;
            Description = entity.Description;
            Status = entity.Status;
            Priority = entity.Priority;
            CreatedAt = entity.CreatedAt;
            UpdatedAt = entity.UpdatedAt;
            Project = new ProjectDto(entity.Project);
            User = new UserDto(entity.AssignedUser);
        }

        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Enums.TaskStatus Status { get; set; }
        public Enums.TaskPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ProjectDto Project { get; set; } = null!;
        public UserDto User { get; set; } = null!;
    }
}
