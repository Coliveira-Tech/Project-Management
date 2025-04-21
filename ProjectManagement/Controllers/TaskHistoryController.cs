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
    public class TaskHistoryController(ITaskHistoryService service) : Controller
    {
        private readonly ITaskHistoryService _service = service;

        [HttpGet("by-task/{taskId}")]
        public async Task<IActionResult> GetByUser([FromRoute] Guid taskId)
        {
            TaskHistoryResponse response = await _service.GetByTask(taskId);
            return response.ToHttpResult();
        }
    }
}
