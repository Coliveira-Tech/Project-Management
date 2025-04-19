using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Api.Extensions;
using ProjectManagement.Api.Interfaces;
using ProjectManagement.Domain.Models;
using System.Diagnostics.CodeAnalysis;

namespace ProjectManagement.Api.Controllers
{
    [ExcludeFromCodeCoverage]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController(ITaskService service) : Controller
    {
        private readonly ITaskService _service = service;

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUser([FromRoute] Guid userId)
        {
            TaskResponse response = await _service.GetByUser(userId);
            return response.ToHttpResult();
        }

        [HttpGet("by-project/{projectId}")]
        public async Task<IActionResult> GetByProject([FromRoute] Guid projectId)
        {
            TaskResponse response = await _service.GetByProject(projectId);
            return response.ToHttpResult();
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] TaskInsertRequest request)
        {
            TaskResponse response = await _service.Insert(request);
            return response.ToHttpResult();
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> Update([FromRoute] Guid taskId, [FromBody] TaskUpdateRequest request)
        {
            TaskResponse response = await _service.Update(taskId, request);
            return response.ToHttpResult();
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid taskId)
        {
            TaskResponse response = await _service.Delete(taskId);
            return response.ToHttpResult();
        }
    }
}
