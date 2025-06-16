using EmployeeTaskManager.API.Data;
using EmployeeTaskManager.API.DTOs;
using EmployeeTaskManager.API.Models;
using EmployeeTaskManager.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTaskManager.API.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly EmployeeTaskManagerContext _context;

        public UserService(EmployeeTaskManagerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    Role = u.Role ?? string.Empty
                })
                .ToListAsync();
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found.");

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = user.Role ?? string.Empty
            };
        }

        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = userDto.PasswordHash ?? string.Empty, // Ensure non-null assignment
                FullName = userDto.FullName,
                Email = userDto.Email,
                Role = userDto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            userDto.UserId = user.UserId;
            return userDto;
        }
        public async Task<UserDto> UpdateUserAsync(int id, UserDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new Exception("User not found");

            user.Username = userDto.Username;
            user.FullName = userDto.FullName;
            user.Email = userDto.Email;
            user.Role = userDto.Role;

            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
