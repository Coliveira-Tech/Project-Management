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
    public class CommentServiceTest
    {
        private readonly CommentService _service;
        private readonly Mock<ILogger<CommentService>> _loggerMock;
        private readonly Mock<IRepository<Comment>> _repositoryMock;
        private readonly Mock<ITaskHistoryService> _taskHistoryServiceMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

        public CommentServiceTest()
        {
            _loggerMock = new();
            _repositoryMock = new();
            _taskHistoryServiceMock = new();
            _httpContextAccessorMock = new();
            _service = new CommentService(_loggerMock.Object
                                        , _repositoryMock.Object
                                        , _taskHistoryServiceMock.Object
                                        , _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Tasks.Task GetByTask_ShouldReturnCommentResponse()
        {
            // Arrange
            Guid taskId = Guid.NewGuid();
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Comment, bool>>>()))
                .ReturnsAsync([new()]);

            // Act
            CommentResponse result = await _service.GetByTask(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CommentResponse>(result);
            Assert.True(result.IsSuccess);
            Assert.Single(result.ListResponse);
        }

        [Fact]
        public async Tasks.Task GetByTask_ShouldReturn404Error_WhenNoMatch()
        {
            // Arrange
            Guid taskId = Guid.NewGuid();
            string expectedMessage = "Comment not found";
            // Act
            CommentResponse result = await _service.GetByTask(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CommentResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
            Assert.Empty(result.ListResponse);
            Assert.Equal(StatusCodes.Status404NotFound, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task GetByTask_ShouldReturn400Error_WhenException()
        {
            // Arrange
            Guid taskId = Guid.NewGuid();
            string expectedMessage = "Error trying to retrieve Comment";
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Comment, bool>>>()))
                .ThrowsAsync(new());

            // Act
            CommentResponse result = await _service.GetByTask(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CommentResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
            Assert.Empty(result.ListResponse);
            Assert.Equal(StatusCodes.Status400BadRequest, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task Insert_ShouldReturnCommentResponse()
        {
            // Arrange
            CommentInsertRequest request = new()
            {
                Content = "Test comment",
                UserId = Guid.NewGuid(),
                TaskId = Guid.NewGuid()
            };

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext)
                                    .Returns(new DefaultHttpContext());

            _httpContextAccessorMock.Setup(httpCA => httpCA.HttpContext.Request.Headers)
                                    .Returns(new HeaderDictionary
                                    {
                                        { "LoggedUserId", request.UserId.ToString() },
                                        { "LoggedUserRole", "3" }
                                    });

            // Act
            CommentResponse result = await _service.Insert(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CommentResponse>(result);
            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(x => x.Insert(It.IsAny<Comment>()), Times.Once);
            _taskHistoryServiceMock.Verify(x => x.Insert(It.IsAny<TaskHistoryInsertRequest>()), Times.Once);
        }

        [Fact]
        public async Tasks.Task Insert_ShouldReturnError400_WhenException()
        {
            // Arrange
            CommentInsertRequest request = new()
            {
                Content = "Test comment",
                UserId = Guid.NewGuid(),
                TaskId = Guid.NewGuid()
            };
            string expectedMessage = "Error trying to insert Comment";

            _repositoryMock.Setup(repo => repo.Insert(It.IsAny<Comment>()))
                .ThrowsAsync(new());

            // Act
            CommentResponse result = await _service.Insert(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CommentResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
            Assert.Empty(result.ListResponse);
            Assert.Equal(StatusCodes.Status400BadRequest, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task Delete_ShouldReturnCommentResponse()
        {
            // Arrange
            Guid commentId = Guid.NewGuid();

            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Comment, bool>>>()))
                .ReturnsAsync([new()]);

            // Act
            CommentResponse result = await _service.Delete(commentId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CommentResponse>(result);
            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(x => x.Delete(It.IsAny<Comment>()), Times.Once);
            _taskHistoryServiceMock.Verify(x => x.Insert(It.IsAny<TaskHistoryInsertRequest>()), Times.Once);
        }
        
        [Fact]
        public async Tasks.Task Delete_ShouldReturnError404_WhenNoMatch()
        {
            // Arrange
            Guid commentId = Guid.NewGuid();
            string expectedMessage = "Comment not found";

            // Act
            CommentResponse result = await _service.Delete(commentId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CommentResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
            Assert.Empty(result.ListResponse);
            Assert.Equal(StatusCodes.Status404NotFound, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task Delete_ShouldReturnError400_WhenException()
        {
            // Arrange
            Guid commentId = Guid.NewGuid();
            string expectedMessage = "Error trying to delete Comment";

            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Comment, bool>>>()))
                .ThrowsAsync(new());

            // Act
            CommentResponse result = await _service.Delete(commentId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CommentResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
            Assert.Empty(result.ListResponse);
            Assert.Equal(StatusCodes.Status400BadRequest, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task Delete_ShouldReturnDataAndLogError_WhenAfterDeleteError()
        {
            // Arrange
            Guid commentId = Guid.NewGuid();
            string expectedMessage = "Error trying to insert TaskHistory";
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Comment, bool>>>()))
                .ReturnsAsync([new()]);

            _taskHistoryServiceMock.Setup(taskHistoryService => taskHistoryService.Insert(It.IsAny<TaskHistoryInsertRequest>()))
                .ThrowsAsync(new Exception(expectedMessage));

            // Act
            CommentResponse result = await _service.Delete(commentId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CommentResponse>(result);
            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(x => x.Delete(It.IsAny<Comment>()), Times.Once);
            _taskHistoryServiceMock.Verify(x => x.Insert(It.IsAny<TaskHistoryInsertRequest>()), Times.Once);
        }
    }
}
