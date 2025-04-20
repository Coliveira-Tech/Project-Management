using ProjectManagement.Api.Interfaces;
using ProjectManagement.Domain.Dtos;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Models;

namespace ProjectManagement.Api.Services
{
    public class CommentService(ILogger<CommentService> logger
                              , IRepository<Comment> repository)
        : BaseService<CommentService
        , Comment
        , CommentDto
        , CommentResponse>(logger, repository)
        , ICommentService
    {
        public async Task<CommentResponse> GetByTask(Guid taskId)
        {
            return await GetBy(x => x.TaskId == taskId);
        }
    }
}
