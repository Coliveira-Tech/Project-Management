using ProjectManagement.Domain.Models;

namespace ProjectManagement.Api.Interfaces
{
    public interface ITaskHistoryService
    {
        Task<TaskHistoryResponse> GetByTask(Guid taskId);
        Task<TaskHistoryResponse> Insert<TRequest>(TRequest request);
        Task<TaskHistoryResponse> InsertRange<TRequest>(List<TRequest> requests);
    }
}
