namespace ProjectManagement.Domain.Models
{
    public class ReportByUserRequest
    {
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.Now;
        public Guid AssignedUserId { get; set; }
    }
}
