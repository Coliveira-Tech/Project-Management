using ProjectManagement.Api.Interfaces;
using ProjectManagement.Domain.Dtos;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Models;

namespace ProjectManagement.Api.Services
{
    public class UserService(ILogger<UserService> logger
                           , IRepository<User> repository)
        : BaseService<UserService
        , User
        , UserDto
        , UserResponse>(logger, repository)
        , IUserService
    {
    }
}
