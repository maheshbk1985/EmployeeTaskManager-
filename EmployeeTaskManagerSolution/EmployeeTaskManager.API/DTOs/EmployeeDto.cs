namespace EmployeeTaskManager.API.DTOs
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Department { get; set; }
        public required string Designation { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
