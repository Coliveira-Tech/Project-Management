using ProjectManagement.Domain.Models;

namespace ProjectManagement.Api.Interfaces
{
    public interface IReportService
    {
        Task<ReportResponse> GetReport(ReportRequest request);
        Task<ReportResponse> GetReportByUser(ReportByUserRequest request);
    }
}
