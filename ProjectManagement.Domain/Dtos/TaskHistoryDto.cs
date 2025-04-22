using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Domain.Dtos
{
    public class TaskHistoryDto 
    {
        public TaskHistoryDto() { }
        public TaskHistoryDto(TaskHistory entity)
        {
            if (entity == null)
                return;

            Id = entity.Id;
            Timestamp = entity.Timestamp;
            Field = entity.Field;
            OldValue = entity.OldValue;
            NewValue = entity.NewValue;
            User = new UserDto(entity.User);
        }

        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Field { get; set; } = string.Empty;
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
    }
}
