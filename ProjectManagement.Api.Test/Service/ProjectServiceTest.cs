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
    public class ProjectServiceTest
    {
        private readonly ProjectService _service;
        private readonly Mock<ILogger<ProjectService>> _loggerMock;
        private readonly Mock<IRepository<Project>> _repositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        public ProjectServiceTest()
        {
            _loggerMock = new();
            _repositoryMock = new();
            _httpContextAccessorMock = new();
            _service = new ProjectService(_loggerMock.Object
                                         , _repositoryMock.Object
                                         , _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Tasks.Task GetById_ShouldReturnProjectResponse()
        {
            // Arrange
            Guid projectId = Guid.NewGuid();
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Project, bool>>>()))
                .ReturnsAsync([new()]);

            // Act
            ProjectResponse result = await _service.GetById(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProjectResponse>(result);
            Assert.True(result.IsSuccess);
            Assert.Single(result.ListResponse);
        }

        [Fact]
        public async Tasks.Task GetById_ShouldReturn404Error_WhenNoMatch()
        {
            // Arrange
            Guid projectId = Guid.NewGuid();
            string expectedMessage = "Project not found";

            // Act
            ProjectResponse result = await _service.GetById(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProjectResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
            Assert.Empty(result.ListResponse);
            Assert.Equal(StatusCodes.Status404NotFound, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task GetById_ShouldReturn400Error_WhenException()
        {
            // Arrange
            Guid projectId = Guid.NewGuid();
            string expectedMessage = "Error trying to retrieve Project";
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Project, bool>>>()))
                .ThrowsAsync(new());

            // Act
            ProjectResponse result = await _service.GetById(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProjectResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
            Assert.Empty(result.ListResponse);
            Assert.Equal(StatusCodes.Status400BadRequest, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task GetByUser_ShouldReturnProjectResponse()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Project, bool>>>()))
                .ReturnsAsync([new(), new(), new()]);

            // Act
            ProjectResponse result = await _service.GetByUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProjectResponse>(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.ListResponse.Count);
        }

        [Fact]
        public async Tasks.Task GetByUser_ShouldReturn404Error_WhenNoMatch()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string expectedMessage = "Project not found";

            // Act
            ProjectResponse result = await _service.GetByUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProjectResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
            Assert.Empty(result.ListResponse);
            Assert.Equal(StatusCodes.Status404NotFound, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task GetByUser_ShouldReturn400Error_WhenException()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string expectedMessage = "Error trying to retrieve Project";
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Project, bool>>>()))
                .ThrowsAsync(new());

            // Act
            ProjectResponse result = await _service.GetByUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProjectResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
            Assert.Empty(result.ListResponse);
            Assert.Equal(StatusCodes.Status400BadRequest, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task Insert_ShouldReturnProjectResponse()
        {
            // Arrange
            ProjectInsertRequest request = new()
            {
                Name = "Test project",
                Description = "Test Project for unit test",
                OwnerId = Guid.NewGuid()
            };

            _repositoryMock.Setup(repo => repo.Insert(It.IsAny<Project>()))
                                    .Returns(Tasks.Task.CompletedTask);

            // Act
            ProjectResponse result = await _service.Insert(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProjectResponse>(result);
            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(x => x.Insert(It.IsAny<Project>()), Times.Once);
        }

        [Fact]
        public async Tasks.Task Insert_ShouldReturnError400_WhenException()
        {
            // Arrange
            ProjectInsertRequest request = new()
            {
                Name = "Test project",
                Description = "Test Project for unit test",
                OwnerId = Guid.NewGuid()
            };

            string expectedMessage = "Error trying to insert Project";

            _repositoryMock.Setup(repo => repo.Insert(It.IsAny<Project>()))
                .ThrowsAsync(new());

            // Act
            ProjectResponse result = await _service.Insert(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProjectResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
            Assert.Empty(result.ListResponse);
            Assert.Equal(StatusCodes.Status400BadRequest, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task Delete_ShouldDeleteAndReturnProjectResponse()
        {
            // Arrange
            Guid projectId = Guid.NewGuid();
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Project, bool>>>()))
                .ReturnsAsync([new()]);

            // Act
            ProjectResponse result = await _service.Delete(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProjectResponse>(result);
            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(x => x.Delete(It.IsAny<Project>()), Times.Once);
        }

        [Fact]
        public async Tasks.Task Delete_ShouldReturnError422_WhenProjectHasActiveTasks()
        {
            // Arrange
            Guid projectId = Guid.NewGuid();
            List<Project> projects = [new()
                {
                    Tasks = [new(){
                        Id = Guid.NewGuid(),
                        Status = Domain.Enums.TaskStatus.InProgress,
                        ProjectId = projectId
                    }]
                }];

            string expectedMessage = "Project cannot be deleted while it has active tasks. Please finish or delete active tasks";

            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Project, bool>>>()))
                .ReturnsAsync(projects);

            // Act
            ProjectResponse result = await _service.Delete(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProjectResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
            Assert.Single(result.ListResponse);
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.ErrorCode);
            _repositoryMock.Verify(x => x.Delete(It.IsAny<Project>()), Times.Never);
        }

        [Fact]
        public async Tasks.Task Delete_ShouldReturnError404_WhenNoMatch()
        {
            // Arrange
            Guid projectId = Guid.NewGuid();
            string expectedMessage = "Project not found";

            // Act
            ProjectResponse result = await _service.Delete(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProjectResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
            Assert.Empty(result.ListResponse);
            Assert.Equal(StatusCodes.Status404NotFound, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task Delete_ShouldReturnError400_WhenException()
        {
            // Arrange
            Guid projectId = Guid.NewGuid();
            string expectedMessage = "Error trying to retrieve Project";
            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Project, bool>>>()))
                .ThrowsAsync(new());

            // Act
            ProjectResponse result = await _service.Delete(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProjectResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
            Assert.Empty(result.ListResponse);
            Assert.Equal(StatusCodes.Status400BadRequest, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task Update_ShouldUpdateAndReturnProjectResponse()
        {
            // Arrange
            Guid projectId = Guid.NewGuid();
            List<Project> projects = [new()
                {
                    Id = projectId,
                    Name = "Test project",
                    Description = "Test Project for unit test"
                }];

            ProjectUpdateRequest request = new()
            {
                Name = "Updated project",
                Description = "Updated Project for unit test"
            };

            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Project, bool>>>()))
                                    .ReturnsAsync(projects);

            // Act
            ProjectResponse result = await _service.Update(projectId, request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProjectResponse>(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(request.Name, result.ListResponse.First().Name);
            Assert.Equal(request.Description, result.ListResponse.First().Description);
            _repositoryMock.Verify(x => x.Update(It.IsAny<Project>()), Times.Once);
        }

        [Fact]
        public async Tasks.Task Update_ShouldReturnError404_WhenNoMatch()
        {
            // Arrange
            Guid projectId = Guid.NewGuid();
            string expectedMessage = "Project not found";
            ProjectUpdateRequest request = new()
            {
                Name = "Updated project",
                Description = "Updated Project for unit test"
            };

            // Act
            ProjectResponse result = await _service.Update(projectId, request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProjectResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
            Assert.Empty(result.ListResponse);
            Assert.Equal(StatusCodes.Status404NotFound, result.ErrorCode);
        }

        [Fact]
        public async Tasks.Task Update_ShouldReturnError400_WhenException()
        {
            // Arrange
            Guid projectId = Guid.NewGuid();
            string expectedMessage = "Error trying to update Project";
            ProjectUpdateRequest request = new()
            {
                Name = "Updated project",
                Description = "Updated Project for unit test"
            };

            _repositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Project, bool>>>()))
                .ThrowsAsync(new());

            // Act
            ProjectResponse result = await _service.Update(projectId, request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProjectResponse>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message.First());
            Assert.Empty(result.ListResponse);
            Assert.Equal(StatusCodes.Status400BadRequest, result.ErrorCode);
        }
    }
}
