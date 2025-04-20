using ProjectManagement.Domain.Models;

namespace ProjectManagement.Api.Interfaces
{
    public interface ICommentService
    {
        Task<CommentResponse> GetByTask(Guid taskId);
        Task<CommentResponse> Insert<TRequest>(TRequest request);
        Task<CommentResponse> Update<TRequest>(Guid commentId, TRequest request);
        Task<CommentResponse> Delete(Guid commentId);
    }
}
