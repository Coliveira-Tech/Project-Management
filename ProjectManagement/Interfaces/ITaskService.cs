using ProjectManagement.Domain.Models;

namespace ProjectManagement.Api.Interfaces
{
    public interface ITaskService
    {
        Task<TaskResponse> GetByUser(Guid userId);
        Task<TaskResponse> GetByProject(Guid projectId);
        Task<TaskResponse> Insert(TaskInsertRequest request);
        Task<TaskResponse> Update<TRequest>(Guid id, TRequest request);
        Task<TaskResponse> Delete(Guid projectId);
    }
}
