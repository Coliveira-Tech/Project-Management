namespace ProjectManagement.Domain.Models
{
    public class CommentInsertRequest
    {
        public string Content { get; set; } = string.Empty;
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
    }
}
