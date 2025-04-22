using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectManagement.Api.Interfaces;
using ProjectManagement.Api.Services;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Models;
using System.Linq.Expressions;
using Tasks = System.Threading.Tasks;

namespace ProjectManagement.Api.Test.Service
{
    public class TaskHistoryServiceTest
    {
        private readonly TaskHistoryService _service;
        private readonly Mock<ILogger<TaskHistoryService>> _loggerMock;
        private readonly Mock<IRepository<TaskHistory>> _repositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

        public TaskHistoryServiceTest()
        {
            _loggerMock = new();
            _repositoryMock = new();
            _httpContextAccessorMock = new();
            _service = new TaskHistoryService(_loggerMock.Object
                                            , _repositoryMock.Object
                                            , _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Tasks.Task GetByTask_ShouldReturnTaskHistoryResponse()
        {
            // Arrange
            Guid taskId = Guid.NewGuid();
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<TaskHistory, bool>>>()))
                .ReturnsAsync([new()]);

            // Act
            TaskHistoryResponse result = await _service.GetByTask(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskHistoryResponse>(result);
            Assert.True(result.IsSuccess);
            Assert.Single(result.ListResponse);

        }

        [Fact]
        public async Tasks.Task GetByTask_ShouldReturn404Error_WhenNoMatch()
        {
            // Arrange
            Guid taskId = Guid.NewGuid();
            string expectedMessage = "TaskHistory not found";

            // Act
            TaskHistoryResponse result = await _service.GetByTask(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskHistoryResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedMessage, result.Message);
            Assert.Equal(StatusCodes.Status404NotFound, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task GetByTask_ShouldReturn400Error_WhenException()
        {
            // Arrange
            Guid taskId = Guid.NewGuid();
            string expectedMessage = "Error trying to retrieve TaskHistory";
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<TaskHistory, bool>>>()))
                .Throws(new Exception(expectedMessage));

            // Act
            TaskHistoryResponse result = await _service.GetByTask(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskHistoryResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedMessage, result.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task Insert_ShouldInsertAndReturnTaskHistoryResponse()
        {
            // Arrange
            TaskHistoryInsertRequest request = new()
            {
                TaskId = Guid.NewGuid(),
                Field = "Test Field",
                OldValue = "Old Value",
                NewValue = "New Value",
                UserId = Guid.NewGuid()
            };

            TaskHistory taskHistory = new(request);

            // Act
            TaskHistoryResponse result = await _service.Insert(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskHistoryResponse>(result);
            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(repo => repo.Insert(It.IsAny<TaskHistory>()), Times.Once);
        }

        [Fact]
        public async Tasks.Task Insert_ShouldReturn400Error_WhenException()
        {
            // Arrange
            string expectedMessage = "Error trying to insert TaskHistory";
            _repositoryMock.Setup(repo => repo.Insert(It.IsAny<TaskHistory>()))
                .Throws(new Exception(expectedMessage));

            // Act
            TaskHistoryResponse result = await _service.Insert(new TaskHistoryInsertRequest());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskHistoryResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedMessage, result.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task InsertRange_ShouldInsertAndReturnTaskHistoryResponse()
        {
            // Arrange
            List<TaskHistoryInsertRequest> requests = new()
            {
                new TaskHistoryInsertRequest
                {
                    TaskId = Guid.NewGuid(),
                    Field = "Test Field 1",
                    OldValue = "Old Value 1",
                    NewValue = "New Value 1",
                    UserId = Guid.NewGuid()
                },
                new TaskHistoryInsertRequest
                {
                    TaskId = Guid.NewGuid(),
                    Field = "Test Field 2",
                    OldValue = "Old Value 2",
                    NewValue = "New Value 2",
                    UserId = Guid.NewGuid()
                }
            };

            // Act
            TaskHistoryResponse result = await _service.InsertRange(requests);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskHistoryResponse>(result);
            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(repo => repo.InsertRange(It.IsAny<List<TaskHistory>>()), Times.Once);
        }

        [Fact]
        public async Tasks.Task InsertRange_ShouldReturn400Error_WhenException()
        {
            // Arrange
            string expectedMessage = "Error trying to insert TaskHistory";
            _repositoryMock.Setup(repo => repo.InsertRange(It.IsAny<List<TaskHistory>>()))
                .Throws(new Exception(expectedMessage));
            List<TaskHistoryInsertRequest> requests = new();

            // Act
            TaskHistoryResponse result = await _service.InsertRange(requests);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TaskHistoryResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedMessage, result.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, result.ErrorCode);
        }
    }
}
