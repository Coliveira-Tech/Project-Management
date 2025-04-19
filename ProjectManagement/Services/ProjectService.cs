using ProjectManagement.Api.Interfaces;
using ProjectManagement.Domain.Dtos;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Models;

namespace ProjectManagement.Api.Services
{
    public class ProjectService(ILogger<ProjectService> logger
                              , IRepository<Project> repository)
        : BaseService<ProjectService
        , Project
        , ProjectDto
        , ProjectResponse>(logger, repository)
        , IProjectService
    {

        public async Task<ProjectResponse> GetByUser(Guid userId)
        {
            return await GetBy(x => x.OwnerId == userId);
        }

        public async Task<ProjectResponse> GetById(Guid projectId)
        {
            return await GetBy(x => x.Id == projectId);
        }

        public override async Task<ProjectResponse> Delete(Guid projectId)
        {
            ProjectResponse response = new();

            try
            {
                response = await GetBy(x => x.Id == projectId);

                if (!response.IsSuccess)
                    return response;

                List<TaskDto>? projectTasks = response
                                                .ListResponse?
                                                .FirstOrDefault(new ProjectDto())
                                                .Tasks;

                if (projectTasks != null
                 && projectTasks.Any(x => x.Status != Domain.Enums.TaskStatus.Completed))
                {
                    response.Message.Add("Project cannot be deleted while it has active tasks. Please finish or delete active tasks");
                    response.IsSuccess = false;
                    response.ErrorCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                return await base.Delete(projectId);
            }
            catch (Exception ex)
            {
                response.Message.Add($"Error trying to delete Project");
                response.Message.Add(ex.Message);
                response.ErrorCode = StatusCodes.Status400BadRequest;
            }

            return response;
        }
    }
}
