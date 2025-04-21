namespace ProjectManagement.Domain.Dtos
{
    public class ReportDto
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int NotStartedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int CompletedOverdueTasks { get; set; }
        public int DueDatePostponedTasks { get; set; }
    }
}
