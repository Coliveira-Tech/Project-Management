using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectManagement.Api.Interfaces;
using ProjectManagement.Api.Services;
using Entities = ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Models;
using System.Linq.Expressions;
using Enums = ProjectManagement.Domain.Enums;
using Tasks = System.Threading.Tasks;
using ProjectManagement.Domain.Dtos;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Api.Test.Service
{
    public class ReportServiceTest
    {
        private readonly ReportService _service;
        private readonly Mock<IReportService> _reportServiceMock;
        private readonly Mock<ILogger<ReportService>> _loggerMock;
        private readonly Mock<IRepository<Entities.BaseEntity>> _repositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<ITaskHistoryService> _taskHistoryServiceMock;
        private readonly Mock<ITaskService> _taskServiceMock;

        public ReportServiceTest()
        {
            _reportServiceMock = new();
            _loggerMock = new();
            _repositoryMock = new();
            _httpContextAccessorMock = new();
            _taskHistoryServiceMock = new();
            _taskServiceMock = new();
            _service = new ReportService(_loggerMock.Object
                                        , _repositoryMock.Object
                                        , _httpContextAccessorMock.Object
                                        , _taskServiceMock.Object
                                        , _taskHistoryServiceMock.Object);
        }

        [Fact]
        public async Tasks.Task GetReport_ShouldReturnReportResponse()
        {
            // Arrange
            var request = new ReportRequest();
            
            Guid userId1 = Guid.NewGuid();
            Guid userId2 = Guid.NewGuid();

            var requestbyUser1 = new ReportByUserRequest() { AssignedUserId = userId1 };
            var requestbyUser2 = new ReportByUserRequest() { AssignedUserId = userId2 };

            TaskDto task1 = new()
            {
                Id = Guid.NewGuid(),
                Status = Enums.TaskStatus.Pending,
                Priority = Enums.TaskPriority.High,
                CreatedAt = DateTime.Now.AddDays(-15),
                UpdatedAt = DateTime.Now.AddDays(-5),
                DueDate = DateTime.Now.AddDays(-1),
                AssignedUser = new UserDto
                {
                    Id = userId1,
                    Name = "Test User 1",
                    Role = Enums.UserRole.TeamMember
                }
            };

            TaskDto task2 = new()
            {
                Id = Guid.NewGuid(),
                Status = Enums.TaskStatus.InProgress,
                Priority = Enums.TaskPriority.Medium,
                CreatedAt = DateTime.Now.AddDays(-15),
                UpdatedAt = DateTime.Now.AddDays(-1),
                DueDate = DateTime.Now.AddDays(-1),
                AssignedUser = new UserDto
                {
                    Id = userId2,
                    Name = "Test User 2",
                    Role = Enums.UserRole.TeamMember
                },
            };

            TaskDto task3 = new()
            {
                Id = Guid.NewGuid(),
                Status = Enums.TaskStatus.Completed,
                Priority = Enums.TaskPriority.Medium,
                CreatedAt = DateTime.Now.AddDays(-10),
                UpdatedAt = DateTime.Now.AddDays(-5),
                DueDate = DateTime.Now.AddDays(-5),
                AssignedUser = new UserDto
                {
                    Id = userId2,
                    Name = "Test User 2",
                    Role = Enums.UserRole.TeamMember
                },
            };

            TaskResponse taskResponse = new()
            {
                IsSuccess = true,
                ListResponse = [task1, task2, task3],
            };

            _taskServiceMock.Setup(ts => ts.GetBy(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                            .ReturnsAsync(taskResponse);

            _taskServiceMock.Setup(ts => ts.GetBy(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                            .ReturnsAsync(new TaskResponse()
                            {
                                IsSuccess = true,
                                ListResponse = [task1]
                            });

            _taskServiceMock.Setup(ts => ts.GetBy(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                            .ReturnsAsync(new TaskResponse()
                            {
                                IsSuccess = true,
                                ListResponse = [task2, task3]
                            });

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext)
                                    .Returns(new DefaultHttpContext());

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext.Request.Headers)
                                    .Returns(new HeaderDictionary
                                    {
                                        { "LoggedUserId", Guid.NewGuid().ToString() },
                                        { "LoggedUserRole", "3" }
                                    });

            _taskHistoryServiceMock.Setup(ths => ths.GetByTask(It.IsAny<Guid>()))
                .ReturnsAsync(new TaskHistoryResponse
                {
                    IsSuccess = true,
                    ListResponse =
                    [
                        new TaskHistoryDto
                        {
                            Field = "DueDate",
                            OldValue = DateTime.Now.AddDays(-5).ToString(),
                            NewValue = DateTime.Now.AddDays(-3).ToString(),
                            Timestamp = DateTime.Now.AddDays(-4)
                        }
                    ]
                });

            _taskHistoryServiceMock.Setup(ths => ths.GetByTask(It.IsAny<Guid>()))
                .ReturnsAsync(new TaskHistoryResponse
                {
                    IsSuccess = true,
                    ListResponse =
                    [
                       new TaskHistoryDto
                        {
                            Field = "Status",
                            OldValue = Enums.TaskStatus.InProgress.ToString(),
                            NewValue = Enums.TaskStatus.Completed.ToString(),
                            Timestamp = DateTime.Now.AddDays(-2)
                        },
                    ]
                });

            // Act
            var result = await _service.GetReport(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ReportResponse>(result);
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(result.ListResponse);
        }

        [Fact]
        public async Tasks.Task GetReport_ShouldReturn404Error_WhenThereNoTasks()
        {
            // Arrange
            var request = new ReportRequest();
            string expectedMessage = "No tasks found for the given date range.";

            _taskServiceMock.Setup(ts => ts.GetBy(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                .ReturnsAsync(new TaskResponse { IsSuccess = false, Message = [expectedMessage] });

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext)
                                   .Returns(new DefaultHttpContext());

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext.Request.Headers)
                                    .Returns(new HeaderDictionary
                                    {
                                        { "LoggedUserId", Guid.NewGuid().ToString() },
                                        { "LoggedUserRole", "3" }
                                    });

            // Act
            var result = await _service.GetReport(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ReportResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedMessage, result.Message);
            Assert.Equal(StatusCodes.Status404NotFound, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task GetReport_ShouldReturn400Error_WhenException()
        {
            // Arrange
            var request = new ReportRequest();
            string expectedMessage = "Error trying to generate report";

            _taskServiceMock.Setup(ts => ts.GetBy(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                .Throws(new Exception(expectedMessage));

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext)
                                   .Returns(new DefaultHttpContext());

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext.Request.Headers)
                                    .Returns(new HeaderDictionary
                                    {
                                        { "LoggedUserId", Guid.NewGuid().ToString() },
                                        { "LoggedUserRole", "3" }
                                    });

            // Act
            var result = await _service.GetReport(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ReportResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedMessage, result.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task GetReport_ShouldReturn401Error_WhenUserIsNotManager()
        {
            // Arrange
            var request = new ReportRequest();
            string expectedMessage = "Only managers can access this resource";
            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext)
                                   .Returns(new DefaultHttpContext());
            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext.Request.Headers)
                                    .Returns(new HeaderDictionary
                                    {
                                        { "LoggedUserId", Guid.NewGuid().ToString() },
                                        { "LoggedUserRole", "2" }
                                    });

            // Act
            var result = await _service.GetReport(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ReportResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedMessage, result.Message);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task GetReportByUser_ShouldReturnReportResponse()
        {
            // Arrange
            Guid userId2 = Guid.NewGuid();
            ReportByUserRequest request = new () { AssignedUserId = userId2 };

            TaskDto task2 = new()
            {
                Id = Guid.NewGuid(),
                Status = Enums.TaskStatus.InProgress,
                Priority = Enums.TaskPriority.Medium,
                CreatedAt = DateTime.Now.AddDays(-15),
                UpdatedAt = DateTime.Now.AddDays(-1),
                DueDate = DateTime.Now.AddDays(-1),
                AssignedUser = new UserDto
                {
                    Id = userId2,
                    Name = "Test User 2",
                    Role = Enums.UserRole.TeamMember
                },
            };

            TaskDto task3 = new()
            {
                Id = Guid.NewGuid(),
                Status = Enums.TaskStatus.Completed,
                Priority = Enums.TaskPriority.Medium,
                CreatedAt = DateTime.Now.AddDays(-10),
                UpdatedAt = DateTime.Now.AddDays(-5),
                DueDate = DateTime.Now.AddDays(-5),
                AssignedUser = new UserDto
                {
                    Id = userId2,
                    Name = "Test User 2",
                    Role = Enums.UserRole.TeamMember
                },
            };

            TaskResponse taskResponse = new()
            {
                IsSuccess = true,
                ListResponse = [task2, task3],
            };

            _taskServiceMock.Setup(ts => ts.GetBy(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                            .ReturnsAsync(new TaskResponse()
                            {
                                IsSuccess = true,
                                ListResponse = [task2, task3]
                            });

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext)
                                    .Returns(new DefaultHttpContext());

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext.Request.Headers)
                                    .Returns(new HeaderDictionary
                                    {
                                        { "LoggedUserId", Guid.NewGuid().ToString() },
                                        { "LoggedUserRole", "3" }
                                    });

            _taskHistoryServiceMock.Setup(ths => ths.GetByTask(It.IsAny<Guid>()))
                .ReturnsAsync(new TaskHistoryResponse
                {
                    IsSuccess = true,
                    ListResponse =
                    [
                        new TaskHistoryDto
                        {
                            Field = "DueDate",
                            OldValue = DateTime.Now.AddDays(-5).ToString(),
                            NewValue = DateTime.Now.AddDays(-3).ToString(),
                            Timestamp = DateTime.Now.AddDays(-4)
                        }
                    ]
                });

            _taskHistoryServiceMock.Setup(ths => ths.GetByTask(It.IsAny<Guid>()))
                .ReturnsAsync(new TaskHistoryResponse
                {
                    IsSuccess = true,
                    ListResponse =
                    [
                       new TaskHistoryDto
                        {
                            Field = "Status",
                            OldValue = Enums.TaskStatus.InProgress.ToString(),
                            NewValue = Enums.TaskStatus.Completed.ToString(),
                            Timestamp = DateTime.Now.AddDays(-2)
                        },
                    ]
                });

            // Act
            var result = await _service.GetReportByUser(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ReportResponse>(result);
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(result.ListResponse);
        }

        [Fact]
        public async Tasks.Task GetReportByUser_ShouldReturn404Error_WhenThereNoTasks()
        {
            // Arrange
            var request = new ReportByUserRequest();
            string expectedMessage = "No tasks found for the given user and date range.";
            _taskServiceMock.Setup(ts => ts.GetBy(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                .ReturnsAsync(new TaskResponse { IsSuccess = false, Message = [expectedMessage] });

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext)
                                   .Returns(new DefaultHttpContext());

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext.Request.Headers)
                                    .Returns(new HeaderDictionary
                                    {
                                        { "LoggedUserId", Guid.NewGuid().ToString() },
                                        { "LoggedUserRole", "3" }
                                    });
            // Act
            var result = await _service.GetReportByUser(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ReportResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedMessage, result.Message);
            Assert.Equal(StatusCodes.Status404NotFound, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task GetReportByUser_ShouldReturn400Error_WhenException()
        {
            // Arrange
            var request = new ReportByUserRequest();
            string expectedMessage = "Error trying to generate report";

            _taskServiceMock.Setup(ts => ts.GetBy(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                .Throws(new Exception(expectedMessage));

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext)
                                   .Returns(new DefaultHttpContext());

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext.Request.Headers)
                                    .Returns(new HeaderDictionary
                                    {
                                        { "LoggedUserId", Guid.NewGuid().ToString() },
                                        { "LoggedUserRole", "3" }
                                    });
            // Act
            var result = await _service.GetReportByUser(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ReportResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedMessage, result.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task GetReportByUser_ShouldReturn401Error_WhenUserIsNotManager()
        {
            // Arrange
            var request = new ReportByUserRequest();
            string expectedMessage = "Only managers can access this resource";

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext)
                                   .Returns(new DefaultHttpContext());

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext.Request.Headers)
                                    .Returns(new HeaderDictionary
                                    {
                                        { "LoggedUserId", Guid.NewGuid().ToString() },
                                        { "LoggedUserRole", "2" }
                                    });
            // Act
            var result = await _service.GetReportByUser(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ReportResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedMessage, result.Message);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.ErrorCode);
        }

    }
}
