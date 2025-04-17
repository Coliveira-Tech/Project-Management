using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Domain.Dtos
{
    public class UserDto
    {
        public UserDto() { }

        public UserDto(User entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Email = entity.Email;
            Password = entity.Password;
            Role = entity.Role;
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
