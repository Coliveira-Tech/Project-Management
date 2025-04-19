using ProjectManagement.Api.Interfaces;
using ProjectManagement.Domain.Dtos;
using ProjectManagement.Domain.Models;

namespace ProjectManagement.Api.Services
{
    public class TaskService(ILogger<TaskService> logger
                           , IRepository<Domain.Entities.Task> repository
                           , IProjectService projectService)
        : BaseService<TaskService
        , Domain.Entities.Task
        , TaskDto
        , TaskResponse>(logger, repository)
        , ITaskService
    {
        private readonly IProjectService _projectService = projectService;

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
            }
            catch (Exception ex)
            {
                response.Message.Add($"Error trying to insert Task");
                response.Message.Add(ex.Message);
                response.ErrorCode = StatusCodes.Status400BadRequest;
            }

            return response;
        }
    }
}
