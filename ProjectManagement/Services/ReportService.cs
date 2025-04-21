using ProjectManagement.Api.Interfaces;
using ProjectManagement.Domain.Dtos;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Models;
using Enums = ProjectManagement.Domain.Enums;

namespace ProjectManagement.Api.Services
{
    public class ReportService(ILogger<ReportService> logger
                             , IRepository<BaseEntity> repository
                             , IHttpContextAccessor httpContextAccessor
                             , ITaskService taskService 
                             , ITaskHistoryService taskHistoryService)
        : BaseService<ReportService
        , BaseEntity
        , ReportDto
        , ReportResponse>(logger, repository, httpContextAccessor)
        , IReportService
    {
        private readonly ITaskService _taskService = taskService;
        private readonly ITaskHistoryService _taskHistoryService = taskHistoryService;

        public async Task<ReportResponse> GetReport(ReportRequest request)
        {
            ReportResponse response = new();

            try
            {
                if (!IsManager())
                {
                    response.Message.Add("Only managers can access this resource");
                    response.IsSuccess = false;
                    response.ErrorCode = StatusCodes.Status401Unauthorized;

                    return response;
                }

                TaskResponse tasks = await _taskService.GetBy(task => task.CreatedAt >= request.StartDate
                                                                   && task.CreatedAt <= request.EndDate);

                if (!tasks.IsSuccess)
                {
                    response.Message.Add("No tasks found for the given date range.");
                    response.IsSuccess = false;
                    response.ErrorCode = StatusCodes.Status404NotFound;

                    return response;
                }

                IEnumerable<UserDto> assignedUsers = tasks.ListResponse
                                                          .Select(task => task.AssignedUser)
                                                          .DistinctBy(x => x.Id);

                if (assignedUsers.Any())
                {
                    assignedUsers.ToList().ForEach(user =>
                    {
                        ReportResponse userStats = GetReportByUser(new() 
                        { 
                            AssignedUserId = user.Id,
                            StartDate = request.StartDate,
                            EndDate = request.EndDate
                        }).Result;

                        response.ListResponse.AddRange(userStats.ListResponse);
                    });

                    response.TotalTasks = response.ListResponse.Sum(x => x.TotalTasks);
                    response.AverageCompletedTasks = response.ListResponse.Select(x => x.CompletedTasks).Average();
                    response.AverageInProgressTasks = response.ListResponse.Select(x => x.InProgressTasks).Average();
                    response.AverageNotStartedTasks = response.ListResponse.Select(x => x.NotStartedTasks).Average();
                    response.AverageOverdueTasks = response.ListResponse.Select(x => x.OverdueTasks).Average();
                    response.AverageCompletedOverdueTasks = response.ListResponse.Select(x => x.CompletedOverdueTasks).Average();
                    response.AverageDueDatePostponedTasks = response.ListResponse.Select(x => x.DueDatePostponedTasks).Average();
                    response.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                response.Message.Add($"Error trying to generate report");
                response.Message.Add(ex.Message);
                response.ErrorCode = StatusCodes.Status400BadRequest;
            }

            return response;
        }

        public async Task<ReportResponse> GetReportByUser(ReportByUserRequest request)
        {
            ReportResponse response = new();
            int completedOverdue = 0;
            int dueDatePostponed = 0;

            try
            {
                if (!IsManager())
                {
                    response.Message.Add("Only managers can access this resource");
                    response.IsSuccess = false;
                    response.ErrorCode = StatusCodes.Status401Unauthorized;

                    return response;
                }

                TaskResponse tasks = await _taskService.GetBy(task => task.AssignedUserId == request.AssignedUserId
                                                               && task.CreatedAt >= request.StartDate
                                                               && task.CreatedAt <= request.EndDate);

                if (!tasks.IsSuccess)
                {
                    response.Message.Add("No tasks found for the given user and date range.");
                    response.IsSuccess = false;
                    response.ErrorCode = StatusCodes.Status404NotFound;
                    return response;
                }

                tasks.ListResponse.ForEach(task =>
                {
                    TaskHistoryResponse taskHistory = _taskHistoryService.GetByTask(task.Id).Result;

                    if (taskHistory.IsSuccess)
                    {
                        completedOverdue = taskHistory.ListResponse
                                              .Where(th => th.Field == "Status"
                                                        && th.NewValue == Enums.TaskStatus.Completed.ToString()
                                                        && th.Timestamp > task.DueDate)
                                              .Count();

                        dueDatePostponed = taskHistory.ListResponse
                                              .Where(th => th.Field == "DueDate"
                                                        && DateTime.Parse(th.NewValue) > DateTime.Parse(th.OldValue))
                                              .Count();
                    }
                });

                ReportDto report = new()
                {
                    TotalTasks = tasks.ListResponse.Count,
                    CompletedTasks = tasks.ListResponse.Count(t => t.Status == Enums.TaskStatus.Completed),
                    InProgressTasks = tasks.ListResponse.Count(t => t.Status == Enums.TaskStatus.InProgress),
                    NotStartedTasks = tasks.ListResponse.Count(t => t.Status == Enums.TaskStatus.Pending),
                    OverdueTasks = tasks.ListResponse.Count(t => t.Status != Enums.TaskStatus.Completed && t.DueDate < DateTime.Now),
                    CompletedOverdueTasks = completedOverdue,
                    DueDatePostponedTasks = dueDatePostponed
                };

                response.ListResponse.Add(report);
                response.IsSuccess = true;
                response.Message.Add("Report generated successfully.");
            }
            catch (Exception ex)
            {
                response.Message.Add($"Error trying to generate report");
                response.Message.Add(ex.Message);
                response.ErrorCode = StatusCodes.Status400BadRequest;
            }

            return response;
        }

        private bool IsManager() 
        { 
            return GetLoggedUserRole() == Enums.UserRole.Manager;
        }
    }
}
