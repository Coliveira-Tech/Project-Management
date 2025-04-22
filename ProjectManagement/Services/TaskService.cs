using ProjectManagement.Api.Interfaces;
using ProjectManagement.Domain.Dtos;
using ProjectManagement.Domain.Models;
using System.Reflection;

namespace ProjectManagement.Api.Services
{
    public class TaskService(ILogger<TaskService> logger
                           , IRepository<Domain.Entities.Task> repository
                           , IProjectService projectService
                           , ITaskHistoryService taskHistoryService
                           , IHttpContextAccessor httpContextAccessor)
        : BaseService<TaskService
        , Domain.Entities.Task
        , TaskDto
        , TaskResponse>(logger, repository, httpContextAccessor)
        , ITaskService
    {
        private readonly ILogger<TaskService> _logger = logger;
        private readonly IProjectService _projectService = projectService;
        private readonly ITaskHistoryService _taskHistoryService = taskHistoryService;

        public async Task<TaskResponse> GetByUser(Guid userId)
        {
            return await GetBy(x => x.AssignedUserId == userId);
        }

        public async Task<TaskResponse> GetByProject(Guid projectId)
        {
            return await GetBy(x => x.ProjectId == projectId);
        }

        public async Task<TaskResponse> Insert(TaskInsertRequest request)
        {
            TaskResponse response = new();

            try
            {
                ProjectResponse projectResponse = await _projectService
                                                            .GetById(request.ProjectId);

                if (projectResponse.IsSuccess)
                {
                    ProjectDto project = projectResponse
                                            .ListResponse
                                            .FirstOrDefault(new ProjectDto());

                    if(project.Tasks.Count >= 20)
                    {
                        response.Message.Add("Project cannot have more than 20 tasks");
                        response.IsSuccess = false;
                        response.ErrorCode = StatusCodes.Status422UnprocessableEntity;
                        return response;
                    }

                    return await base.Insert(request);
                }

                response.Message.AddRange(projectResponse.Message);
                response.IsSuccess = projectResponse.IsSuccess;
                response.ErrorCode = projectResponse.ErrorCode;
            }
            catch (Exception ex)
            {
                response.Message.Add($"Error trying to insert Task");
                response.Message.Add(ex.Message);
                response.ErrorCode = StatusCodes.Status400BadRequest;
            }

            return response;
        }

        protected override async Task AfterUpdate(Domain.Entities.Task entity
                                                , List<Tuple<PropertyInfo?, object?, object?>> updatedProperties)
        {
            try
            {
                List<TaskHistoryInsertRequest> requests = [];

                updatedProperties.ForEach(updatedProperty =>
                {
                    PropertyInfo? propertyInfo = updatedProperty.Item1;
                    object? oldValue = updatedProperty.Item2;
                    object? newValue = updatedProperty.Item3;

                    TaskHistoryInsertRequest request = new()
                    {
                        TaskId = entity.Id,
                        Field = propertyInfo?.Name ?? string.Empty,
                        OldValue = oldValue?.ToString() ?? string.Empty,
                        NewValue = newValue?.ToString() ?? string.Empty,
                        UserId = GetLoggedUserId()
                    };

                    requests.Add(request);
                });

                if(requests.Count > 0)
                    await _taskHistoryService.InsertRange(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to insert TaskHistory");
            }
        }
    }
}
