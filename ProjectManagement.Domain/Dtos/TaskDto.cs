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
            AssignedUser = new UserDto(entity.AssignedUser);
        }

        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Enums.TaskStatus Status { get; set; }
        public string StatusDescription { get => Status.ToString(); }
        public Enums.TaskPriority Priority { get; set; }
        public string PriorityDescription { get => Priority.ToString(); }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ProjectDto Project { get; set; } = null!;
        public UserDto AssignedUser { get; set; } = null!;
    }
}
