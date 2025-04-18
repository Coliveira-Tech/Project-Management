using ProjectManagement.Domain.Models;

namespace ProjectManagement.Api.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectResponse> GetByUser(Guid userId);
        Task<ProjectResponse> Insert<TRequest>(TRequest request);
        Task<ProjectResponse> Update<TRequest>(Guid id, TRequest request);
        Task<ProjectResponse> Delete(Guid projectId);
    }
}
