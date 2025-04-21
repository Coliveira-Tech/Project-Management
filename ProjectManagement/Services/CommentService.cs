using ProjectManagement.Api.Interfaces;
using ProjectManagement.Domain.Dtos;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Models;
using System.Reflection;
using Tasks = System.Threading.Tasks;

namespace ProjectManagement.Api.Services
{
    public class CommentService(ILogger<CommentService> logger
                              , IRepository<Comment> repository
                              , IHttpContextAccessor httpContextAccessor)
        : BaseService<CommentService
        , Comment
        , CommentDto
        , CommentResponse>(logger, repository, httpContextAccessor)
        , ICommentService
    {
        public async Task<CommentResponse> GetByTask(Guid taskId)
        {
            return await GetBy(x => x.TaskId == taskId);
        }

        //protected override async Tasks.Task AfterInsert(Comment entity, PropertyInfo? propertyInfo, object? newValue)
        //{
        //    await base.AfterInsert(entity, propertyInfo, newValue);
        //}
    }
}
