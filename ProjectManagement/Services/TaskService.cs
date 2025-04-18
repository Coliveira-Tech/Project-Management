using ProjectManagement.Api.Interfaces;
using ProjectManagement.Domain.Dtos;
using ProjectManagement.Domain.Models;

namespace ProjectManagement.Api.Services
{
    public class TaskService(ILogger<TaskService> logger
                           , IRepository<Domain.Entities.Task> repository)
        : BaseService<TaskService
    , Domain.Entities.Task
    , TaskDto
            , TaskResponse>(logger, repository)
        , ITaskService
    {
        private readonly ILogger<TaskService> _logger = logger;
        private readonly IRepository<Domain.Entities.Task> _repository = repository;
    }
}
