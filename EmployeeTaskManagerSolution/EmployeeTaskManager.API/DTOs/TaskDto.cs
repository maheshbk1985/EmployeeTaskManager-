namespace EmployeeTaskManager.API.DTOs
{
    public class TaskDto
    {
        public int TaskId { get; set; }
        public int EmployeeId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Status { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
