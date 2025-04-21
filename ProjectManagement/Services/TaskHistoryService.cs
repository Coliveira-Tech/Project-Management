using ProjectManagement.Api.Interfaces;
using ProjectManagement.Domain.Dtos;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Models;

namespace ProjectManagement.Api.Services
{
    public class TaskHistoryService(ILogger<TaskHistoryService> logger
                                  , IRepository<TaskHistory> repository
                              , IHttpContextAccessor httpContextAccessor)
        : BaseService<TaskHistoryService
        , TaskHistory
        , TaskHistoryDto
        , TaskHistoryResponse>(logger, repository, httpContextAccessor)
        , ITaskHistoryService
    {
        public async Task<TaskHistoryResponse> GetByTask(Guid taskId)
        {
            return await GetBy(x => x.TaskId == taskId);
        }
    }
}
