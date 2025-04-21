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
    public class ReportController(IReportService service) : Controller
    {
        private readonly IReportService _service = service;

        [HttpGet()]
        public async Task<IActionResult> GetReport([FromQuery] ReportRequest request)
        {
            ReportResponse response = await _service.GetReport(request);
            return response.ToHttpResult();
        }

        [HttpGet("by-user")]
        public async Task<IActionResult> GetReportByUser([FromQuery] ReportByUserRequest request)
        {
            ReportResponse response = await _service.GetReportByUser(request);
            return response.ToHttpResult();
        }
    }
}
