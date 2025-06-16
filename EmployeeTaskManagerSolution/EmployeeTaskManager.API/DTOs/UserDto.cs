namespace EmployeeTaskManager.API.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public string? Password { get; set; }
        public string? PasswordHash { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
    }
}
