using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Domain.Dtos
{
    public class CommentDto
    {
        public CommentDto() { }
        public CommentDto(Comment entity)
        {
            if (entity == null)
                return;

            Id = entity.Id;
            Content = entity.Content;
            CreatedAt = entity.CreatedAt;
            User = new UserDto(entity.User);
        }

        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public TaskDto Task { get; set; } = null!;
        public UserDto User { get; set; } = null!;
    }
}
