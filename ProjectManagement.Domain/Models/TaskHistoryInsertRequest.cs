namespace ProjectManagement.Domain.Models
{
    public class TaskHistoryInsertRequest
    {
        public string Field { get; set; } = string.Empty;
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
    }
}
