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
    public class ProjectController(IProjectService service) : Controller
    {
        private readonly IProjectService _service = service;

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUser([FromRoute] Guid userId)
        {
            ProjectResponse response = await _service.GetByUser(userId);
            return response.ToHttpResult();
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] ProjectInsertRequest request)
        {
            ProjectResponse response = await _service.Insert(request);
            return response.ToHttpResult();
        }

        [HttpPut("{projectId}")]
        public async Task<IActionResult> Update([FromRoute] Guid projectId, [FromBody] ProjectUpdateRequest request)
        {
            ProjectResponse response = await _service.Update(projectId, request);
            return response.ToHttpResult();
        }

        [HttpDelete("{projectId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid projectId)
        {
            ProjectResponse response = await _service.Delete(projectId);
            return response.ToHttpResult();
        }
    }
}
