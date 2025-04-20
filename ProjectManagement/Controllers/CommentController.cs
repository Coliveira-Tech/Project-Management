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
    public class CommentController(ICommentService service) : Controller
    {
        private readonly ICommentService _service = service;

        [HttpGet("by-task/{taskId}")]
        public async Task<IActionResult> GetByTask([FromRoute] Guid taskId)
        {
            CommentResponse response = await _service.GetByTask(taskId);
            return response.ToHttpResult();
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] CommentInsertRequest request)
        {
            CommentResponse response = await _service.Insert(request);
            return response.ToHttpResult();
        }

        [HttpPut("{commentId}")]
        public async Task<IActionResult> Update([FromRoute] Guid commentId, [FromBody] CommentUpdateRequest request)
        {
            CommentResponse response = await _service.Update(commentId, request);
            return response.ToHttpResult();
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid commentId)
        {
            CommentResponse response = await _service.Delete(commentId);
            return response.ToHttpResult();
        }
    }
}
