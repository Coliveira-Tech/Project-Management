namespace ProjectManagement.Domain.Models
{
    public class ReportRequest
    {
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.Now;
    }
}
