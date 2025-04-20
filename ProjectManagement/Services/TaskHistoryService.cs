using ProjectManagement.Api.Interfaces;
using ProjectManagement.Domain.Dtos;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Models;

namespace ProjectManagement.Api.Services
{
    public class TaskHistoryService(ILogger<TaskHistoryService> logger
                                  , IRepository<TaskHistory> repository)
        : BaseService<TaskHistoryService
        , TaskHistory
        , TaskHistoryDto
        , TaskHistoryResponse>(logger, repository)
        , ITaskHistoryService
    {
        private readonly ILogger<TaskHistoryService> _logger = logger;
        private readonly IRepository<TaskHistory> _repository = repository;
    }
}
