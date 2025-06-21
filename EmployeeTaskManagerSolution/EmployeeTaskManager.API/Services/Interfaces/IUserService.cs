using EmployeeTaskManager.API.DTOs;

namespace EmployeeTaskManager.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> ValidateUserAsync(string email, string password);
        Task<UserDto> GetUserByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> CreateUserAsync(UserDto userDto);
        Task<UserDto> UpdateUserAsync(int id, UserDto userDto);
        Task<bool> DeleteUserAsync(int id);
    }
}
