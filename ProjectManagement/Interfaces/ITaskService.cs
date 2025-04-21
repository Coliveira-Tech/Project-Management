using ProjectManagement.Domain.Models;
using System.Linq.Expressions;
using Entities = ProjectManagement.Domain.Entities;

namespace ProjectManagement.Api.Interfaces
{
    public interface ITaskService
    {
        Task<TaskResponse> GetByUser(Guid userId);
        Task<TaskResponse> GetBy(Expression<Func<Entities.Task, bool>> predicate);
        Task<TaskResponse> GetByProject(Guid projectId);
        Task<TaskResponse> Insert(TaskInsertRequest request);
        Task<TaskResponse> Update<TRequest>(Guid id, TRequest request);
        Task<TaskResponse> Delete(Guid projectId);
    }
}
