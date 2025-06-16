namespace EmployeeTaskManager.API.Models
{
    public class ApiExceptionResponse
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
    }
}
