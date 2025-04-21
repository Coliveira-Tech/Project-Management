using ProjectManagement.Api.Interfaces;
using ProjectManagement.Domain.Dtos;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Models;
using Tasks = System.Threading.Tasks;

namespace ProjectManagement.Api.Services
{
    public class CommentService(ILogger<CommentService> logger
                              , IRepository<Comment> repository
                              , ITaskHistoryService taskHistoryService
                              , IHttpContextAccessor httpContextAccessor)
        : BaseService<CommentService
        , Comment
        , CommentDto
        , CommentResponse>(logger, repository, httpContextAccessor)
        , ICommentService
    {
        private readonly ILogger<CommentService> _logger = logger;
        private readonly ITaskHistoryService _taskHistoryService = taskHistoryService;

        public async Task<CommentResponse> GetByTask(Guid taskId)
        {
            return await GetBy(x => x.TaskId == taskId);
        }

        protected override async Tasks.Task AfterInsert(Comment entity)
        {
            try
            {
                TaskHistoryInsertRequest request = new()
                {
                    TaskId = entity.TaskId,
                    Field = "Comment added",
                    OldValue = string.Empty,
                    NewValue = entity.Content,
                    UserId = GetLoggedUserId()
                };

                await _taskHistoryService.Insert(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to insert TaskHistory");
            }
        }

        protected override async Tasks.Task AfterDelete(Comment entity)
        {
            try
            {
                TaskHistoryInsertRequest request = new()
                {
                    TaskId = entity.TaskId,
                    Field = "Comment deleted",
                    OldValue = entity.Content,
                    NewValue = string.Empty,
                    UserId = GetLoggedUserId()
                };

                await _taskHistoryService.Insert(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to insert TaskHistory");
            }
        }
    }
}
