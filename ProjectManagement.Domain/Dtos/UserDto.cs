using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Domain.Dtos
{
    public class UserDto
    {
        public UserDto() { }

        public UserDto(User entity)
        {
            if (entity == null)
                return;

            Id = entity.Id;
            Name = entity.Name;
            Email = entity.Email;
            Role = entity.Role;
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; } = null;
        public UserRole Role { get; set; } = UserRole.Guest;
        public string RoleDescription { get => Role.ToString(); }
    }
}
