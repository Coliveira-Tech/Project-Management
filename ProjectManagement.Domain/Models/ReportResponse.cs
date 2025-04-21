using ProjectManagement.Domain.Dtos;

namespace ProjectManagement.Domain.Models
{
    public class ReportResponse:BaseResponse<ReportDto>
    {
        public int TotalTasks { get; set; }
        public double AverageCompletedTasks { get; set; }
        public double AverageInProgressTasks { get; set; }
        public double AverageNotStartedTasks { get; set; }
        public double AverageOverdueTasks { get; set; }
        public double AverageCompletedOverdueTasks { get; set; }
        public double AverageDueDatePostponedTasks { get; set; }
    }
}
