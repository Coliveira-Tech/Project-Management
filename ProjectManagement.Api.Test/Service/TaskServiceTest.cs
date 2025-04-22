using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectManagement.Api.Interfaces;
using ProjectManagement.Api.Services;
using ProjectManagement.Domain.Dtos;
using ProjectManagement.Domain.Models;
using System.Linq.Expressions;
using Entities = ProjectManagement.Domain.Entities;
using Enums = ProjectManagement.Domain.Enums;

namespace ProjectManagement.Api.Test.Service
{
    public class TaskServiceTest
    {
        private readonly TaskService _service;
        private readonly Mock<ILogger<TaskService>> _loggerMock;
        private readonly Mock<IProjectService> _projectServiceMock;
        private readonly Mock<ITaskHistoryService> _taskHistoryServiceMock;
        private readonly Mock<IRepository<Entities.Task>> _repositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        
        public TaskServiceTest()
        {
            _loggerMock = new();
            _repositoryMock = new();
            _projectServiceMock = new();
            _taskHistoryServiceMock = new();
            _httpContextAccessorMock = new();

            _service = new TaskService(_loggerMock.Object
                                         , _repositoryMock.Object
                                         , _projectServiceMock.Object
                                         , _taskHistoryServiceMock.Object
                                         , _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task GetByUser_ShouldReturnTaskResponse()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                .ReturnsAsync([new()]);

            // Act
            TaskResponse result = await _service.GetByUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.True(result.IsSuccess);
            Assert.Single(result.ListResponse);
        }

        [Fact]
        public async Task GetByUser_ShouldReturn404Error_WhenNoMatch()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string expectedMessage = "Task not found";

            // Act
            TaskResponse result = await _service.GetByUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedMessage, result.Message);
            Assert.Equal(StatusCodes.Status404NotFound, result.ErrorCode);
        }

        [Fact]
        public async Task GetByUser_ShouldReturn400Error_WhenException()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string expectedMessage = "Error trying to retrieve Task";
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                .ThrowsAsync(new Exception(expectedMessage));

            // Act
            TaskResponse result = await _service.GetByUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedMessage, result.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, result.ErrorCode);

        }

        [Fact]
        public async Task GetByProject_ShouldReturnTaskResponse()
        {
            // Arrange
            Guid projectId = Guid.NewGuid();
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                .ReturnsAsync([new()]);

            // Act
            TaskResponse result = await _service.GetByProject(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.True(result.IsSuccess);
            Assert.Single(result.ListResponse);
        }

        [Fact]
        public async Task GetByProject_ShouldReturn404Error_WhenNoMatch()
        {
            // Arrange
            Guid projectId = Guid.NewGuid();
            string expectedMessage = "Task not found";

            // Act
            TaskResponse result = await _service.GetByProject(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedMessage, result.Message);
            Assert.Equal(StatusCodes.Status404NotFound, result.ErrorCode);
        }

        [Fact]
        public async Task GetByProject_ShouldReturn400Error_WhenException()
        {
            // Arrange
            Guid projectId = Guid.NewGuid();
            string expectedMessage = "Error trying to retrieve Task";
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                .ThrowsAsync(new Exception(expectedMessage));

            // Act
            TaskResponse result = await _service.GetByProject(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedMessage, result.Message);
        }

        [Fact]
        public async Task Insert_ShouldInsertAndReturnTaskResponse()
        {
            // Arrange
            TaskInsertRequest request = new()
            {
                ProjectId = Guid.NewGuid(),
                AssignedUserId = Guid.NewGuid(),
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(7),
                Priority = Enums.TaskPriority.Medium,
                Status = Enums.TaskStatus.Pending,
            };

            ProjectResponse projReponse = new()
            {
                IsSuccess = true,
                ListResponse = [new ProjectDto()
                    {
                        Tasks = [new TaskDto()]
                    }]
            };

            Entities.Task task = new(request);

            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                .ReturnsAsync([task]);

            _projectServiceMock.Setup(projService => projService.GetById(It.IsAny<Guid>()))
                .ReturnsAsync(projReponse);

            // Act
            TaskResponse result = await _service.Insert(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(repo => repo.Insert(It.IsAny<Entities.Task>()), Times.Once);

        }

        [Fact]
        public async Task Insert_ShouldReturn422Error_WhenProjectHasMaxTasks()
        {
            // Arrange
            string expectedMessage = "Project cannot have more than 20 tasks";
            TaskInsertRequest request = new()
            {
                ProjectId = Guid.NewGuid(),
                AssignedUserId = Guid.NewGuid(),
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(7),
                Priority = Enums.TaskPriority.Medium,
                Status = Enums.TaskStatus.Pending,
            };

            ProjectResponse projReponse = new()
            {
                IsSuccess = true,
                ListResponse = [new ProjectDto()
                    {
                        Tasks = []
                    }]
            };

            for (int i = 0; i < 20; i++)
                projReponse.ListResponse[0].Tasks.Add(new TaskDto());

            _projectServiceMock.Setup(projService => projService.GetById(It.IsAny<Guid>()))
                .ReturnsAsync(projReponse);

            // Act
            TaskResponse result = await _service.Insert(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.ErrorCode);
            Assert.Contains(expectedMessage, result.Message);
        }

        [Fact]
        public async Task Insert_ShouldReturn400Error_WhenException()
        {
            // Arrange
            string expectedMessage = "Error trying to insert Task";

            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                .ThrowsAsync(new Exception(expectedMessage));

            // Act
            TaskResponse result = await _service.Insert(new TaskInsertRequest());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
        }

        [Fact]
        public async Task Insert_ShouldReturn404Error_WhenProjectNotFound()
        {
            // Arrange
            string expectedMessage = "Project not found";

            _projectServiceMock.Setup(projService => projService.GetById(It.IsAny<Guid>()))
                .ReturnsAsync(new ProjectResponse()
                {
                    IsSuccess = false,
                    Message = ["Project not found"],
                    ErrorCode = StatusCodes.Status404NotFound
                });

            // Act
            TaskResponse result = await _service.Insert(new TaskInsertRequest());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status404NotFound, result.ErrorCode);
            Assert.Contains(expectedMessage, result.Message);
        }

        [Fact]
        public async Task Update_ShouldUpdateAndReturnTaskResponse()
        {
            // Arrange
            Guid taskId = Guid.NewGuid();
            TaskUpdateRequest requestOld = new()
            {
                Title = "Task",
                Description = "Description",
                DueDate = DateTime.Now.AddDays(4),
                Status = Enums.TaskStatus.InProgress,
                AssignedUserId = Guid.NewGuid(),
            };

            TaskUpdateRequest requestUpdated = new()
            {
                Title = "Updated Task",
                Description = "Updated Description",
                DueDate = DateTime.Now.AddDays(7),
                Status = Enums.TaskStatus.Completed,
                AssignedUserId = requestOld.AssignedUserId,
            };

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext)
                                    .Returns(new DefaultHttpContext());

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext.Request.Headers)
                                    .Returns(new HeaderDictionary
                                    {
                                        { "LoggedUserId", requestUpdated.AssignedUserId.ToString() },
                                        { "LoggedUserRole", "2" }
                                    });

            Entities.Task task = new(requestOld);

            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                .ReturnsAsync([task]);

            // Act
            TaskResponse result = await _service.Update(taskId, requestUpdated);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(requestUpdated.Title, result.ListResponse.First().Title);
            Assert.Equal(requestUpdated.Description, result.ListResponse.First().Description);
            Assert.Equal(requestUpdated.DueDate, result.ListResponse.First().DueDate);
            Assert.Equal(requestUpdated.Status, result.ListResponse.First().Status);
            Assert.Equal(requestUpdated.UpdatedAt.Date, result.ListResponse.First().UpdatedAt.Date);
            _repositoryMock.Verify(repo => repo.Update(It.IsAny<Entities.Task>()), Times.Once);
            _taskHistoryServiceMock.Verify(taskHistoryService => taskHistoryService.InsertRange(It.IsAny<List<TaskHistoryInsertRequest>>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldUpdateOnlyStatusAndReturnTaskResponse()
        {
            // Arrange
            Guid taskId = Guid.NewGuid();
            TaskUpdateRequest requestOld = new()
            {
                Title = "Task",
                Description = "Description",
                DueDate = DateTime.Now.AddDays(4),
                Status = Enums.TaskStatus.InProgress,
                AssignedUserId = Guid.NewGuid(),
            };

            TaskUpdateRequest requestUpdated = new()
            {
                Title = "Task",
                Status = Enums.TaskStatus.Completed,
            };

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext)
                                    .Returns(new DefaultHttpContext());

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext.Request.Headers)
                                    .Returns(new HeaderDictionary
                                    {
                                        { "LoggedUserId", requestUpdated.AssignedUserId.ToString() },
                                        { "LoggedUserRole", "2" }
                                    });

            Entities.Task task = new(requestOld);

            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                .ReturnsAsync([task]);

            // Act
            TaskResponse result = await _service.Update(taskId, requestUpdated);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(requestOld.Title, result.ListResponse.First().Title);
            Assert.Equal(requestOld.Description, result.ListResponse.First().Description);
            Assert.Equal(requestOld.DueDate, result.ListResponse.First().DueDate);
            Assert.Equal(requestUpdated.Status, result.ListResponse.First().Status);
            Assert.Equal(requestOld.UpdatedAt, result.ListResponse.First().UpdatedAt);
            _repositoryMock.Verify(repo => repo.Update(It.IsAny<Entities.Task>()), Times.Once);
            _taskHistoryServiceMock.Verify(taskHistoryService => taskHistoryService.InsertRange(It.IsAny<List<TaskHistoryInsertRequest>>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturn404Error_WhenTaskNotFound()
        {
            // Arrange
            Guid taskId = Guid.NewGuid();
            string expectedMessage = "Task not found";

            // Act
            TaskResponse result = await _service.Update(taskId, new TaskUpdateRequest());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status404NotFound, result.ErrorCode);
            Assert.Contains(expectedMessage, result.Message);
        }

        [Fact]
        public async Task Update_ShouldReturn400Error_WhenException()
        {
            // Arrange
            Guid taskId = Guid.NewGuid();
            string expectedMessage = "Error trying to update Task";
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                .ThrowsAsync(new Exception(expectedMessage));

            // Act
            TaskResponse result = await _service.Update(taskId, new TaskUpdateRequest());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
        }

        [Fact]
        public async Task Delete_ShouldDeleteAndReturnTaskResponse()
        {
            // Arrange
            Guid taskId = Guid.NewGuid();
            Entities.Task task = new()
            {
                Id = taskId,
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(7),
                Priority = Enums.TaskPriority.Medium,
                Status = Enums.TaskStatus.Pending,
            };
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                .ReturnsAsync([task]);

            // Act
            TaskResponse result = await _service.Delete(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(repo => repo.Delete(It.IsAny<Entities.Task>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturn404Error_WhenTaskNotFound()
        {
            // Arrange
            Guid taskId = Guid.NewGuid();
            string expectedMessage = "Task not found";

            // Act
            TaskResponse result = await _service.Delete(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status404NotFound, result.ErrorCode);
            Assert.Contains(expectedMessage, result.Message);
        }

        [Fact]
        public async Task Delete_ShouldReturn400Error_WhenException()
        {
            // Arrange
            Guid taskId = Guid.NewGuid();
            string expectedMessage = "Error trying to delete Task";
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Entities.Task, bool>>>()))
                .ThrowsAsync(new Exception(expectedMessage));

            // Act
            TaskResponse result = await _service.Delete(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
        }
    }
}
